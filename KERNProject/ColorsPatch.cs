using BepInEx.Logging;
using HarmonyLib;
using KSP.Game;
using KSP.Modules;
using SpaceWarp.API.Assets;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[HarmonyPatch]
internal class ColorsPatch
{
    private static bool LoadOnInit = true;
    private static string[] allParts;
    private static Dictionary<string, Texture[]> partHash;
    private static int[] propertyIds;
    private static string[] textureSuffixes = new string[6]
    {
    "d.png",
    "m.png",
    "n.png",
    "ao.png",
    "e.png",
    "pm.png"
    };
    private static string[] textureNames = new string[6]
    {
    "diffuse",
    "mettalic",
    "normal",
    "ambient occlusion",
    "emission",
    "paint map"
    };
    private const int DIFFUSE = 0;
    private const int METTALLIC = 1;
    private const int BUMP = 2;
    private const int OCCLUSION = 3;
    private const int EMISSION = 4;
    private const int PAINT_MAP = 5;
    private static Shader ksp2Opaque;
    private static Shader ksp2Transparent;
    private static Shader unityStandard;
    private const string KSP2_OPAQUE_PATH = "KSP2/Scenery/Standard (Opaque)";
    private const string KSP2_TRANSPARENT_PATH = "KSP2/Scenery/Standard (Transparent)";
    private const string UNITY_STANDARD = "Standard";
    private const string displayName = "KHC";
    public static ManualLogSource Logger;

    public static Dictionary<string, string[]> DeclaredParts { get; private set; } = new Dictionary<string, string[]>();

    [HarmonyPrepare]
    private static bool Init(MethodBase original)
    {
        if ((object)original == null)
            return true;
        ColorsPatch.partHash = new Dictionary<string, Texture[]>();
        ColorsPatch.propertyIds = new int[6]
        {
      Shader.PropertyToID("_MainTex"),
      Shader.PropertyToID("_MetallicGlossMap"),
      Shader.PropertyToID("_BumpMap"),
      Shader.PropertyToID("_OcclusionMap"),
      Shader.PropertyToID("_EmissionMap"),
      Shader.PropertyToID("_PaintMaskGlossMap")
        };
        ColorsPatch.ksp2Opaque = Shader.Find("KSP2/Scenery/Standard (Opaque)");
        ColorsPatch.ksp2Transparent = Shader.Find("KSP2/Scenery/Standard (Transparent)");
        ColorsPatch.unityStandard = Shader.Find("Standard");
        ColorsPatch.Logger = BepInEx.Logging.Logger.CreateLogSource("KHC");
        return true;
    }

    public static void DeclareParts(string modGUID, params string[] partNameList) => ColorsPatch.DeclareParts(modGUID, (IEnumerable<string>)((IEnumerable<string>)partNameList).ToList<string>());

    public static void DeclareParts(string modGUID, IEnumerable<string> partNameList)
    {
        if (ColorsPatch.DeclaredParts.ContainsKey(modGUID))
            ColorsPatch.LogWarning((object)(modGUID + " tried to declare their parts twice. Ignoring second call."));
        else if (partNameList.Count<string>() == 0)
            ColorsPatch.LogWarning((object)(modGUID + " tried to declare no parts. Ignoring this call."));
        else
            ColorsPatch.DeclaredParts.Add(modGUID, partNameList.ToArray<string>());
    }

    public static void ReloadTextures()
    {
        foreach (string key in ColorsPatch.DeclaredParts.Keys)
            ColorsPatch.LoadTextures(key);
    }

    public static Texture[] GetTextures(string partName) => ColorsPatch.partHash[partName];

    private static void LoadDeclaredParts()
    {
        List<string> stringList = new List<string>();
        if (ColorsPatch.DeclaredParts.Count == 0)
        {
            ColorsPatch.LogWarning((object)"No parts were declared before load.");
        }
        else
        {
            if (ColorsPatch.LoadOnInit)
            {
                foreach (string key in ColorsPatch.DeclaredParts.Keys)
                {
                    ColorsPatch.LoadTextures(key);
                    foreach (string partName in ColorsPatch.DeclaredParts[key])
                        stringList.Add(ColorsPatch.TrimPartName(partName));
                }
            }
            ColorsPatch.allParts = stringList.ToArray();
        }
    }

    private static bool TryAddUnique(string partName)
    {
        if (ColorsPatch.partHash.ContainsKey(partName))
            return false;
        ColorsPatch.partHash.Add(partName, new Texture[6]);
        return true;
    }

    private static void LoadTextures(string modGUID)
    {
        ColorsPatch.LogMessage((object)(">Loading parts from " + modGUID + "<"));
        foreach (string partName in ColorsPatch.DeclaredParts[modGUID])
        {
            ColorsPatch.LogMessage((object)(">Loading " + partName));
            if (!ColorsPatch.TryAddUnique(partName))
            {
                ColorsPatch.LogWarning((object)(partName + " already exists in hash map. Probably it already exists in another mod. Ignoring this part."));
            }
            else
            {
                string key = ColorsPatch.TrimPartName(partName);
                string str = modGUID.ToLower() + "/images/" + key.ToLower() + "/" + key.ToLower();
                int num1 = 0;
                Texture2D asset1;
                if (AssetManager.TryGetAsset<Texture2D>(str + "_" + ColorsPatch.textureSuffixes[0], out asset1))
                {
                    ColorsPatch.partHash[key][0] = (Texture)asset1;
                    int num2 = num1 + 1;
                    ColorsPatch.LogMessage((object)string.Format("\t({0}/6) Loaded {1} texture", (object)num2, (object)ColorsPatch.textureNames[0]));
                    for (int index = 1; index < ColorsPatch.propertyIds.Length; ++index)
                    {
                        Texture2D asset2;
                        if (AssetManager.TryGetAsset<Texture2D>(str + "_" + ColorsPatch.textureSuffixes[index], out asset2))
                        {
                            ++num2;
                            ColorsPatch.partHash[key][index] = (Texture)asset2;
                            ColorsPatch.LogMessage((object)string.Format("\t({0}/6) Loaded {1} texture", (object)num2, (object)ColorsPatch.textureNames[index]));
                        }
                    }
                }
                else
                {
                    ColorsPatch.LogWarning((object)(partName + " doesn't have a diffuse texture. Skipping this part."));
                    break;
                }
            }
        }
    }

    private static void SetTexturesToMaterial(string partName, ref Material material)
    {
        string key = ColorsPatch.TrimPartName(partName);
        for (int index = 0; index < ColorsPatch.propertyIds.Length; ++index)
        {
            Texture texture = ColorsPatch.partHash[key][index];
            if (texture != null)
                material.SetTexture(ColorsPatch.propertyIds[index], texture);
        }
        material.SetFloat("_Metallic", 1f);
        material.SetFloat("_GlossMapScale", 0.85f);
    }

    private static string TrimPartName(string partName)
    {
        if (partName.Length >= 3)
        {
            if (partName.EndsWith("XS") || partName.EndsWith("XL"))
                return partName.Remove(partName.Length - 2, 2);
            if (partName.EndsWith("S") || partName.EndsWith("M") || partName.EndsWith("L"))
                return partName.Remove(partName.Length - 1);
        }
        return partName;
    }

    [HarmonyPatch(typeof(GameManager), "OnLoadingFinished")]
    public static void Prefix() => ColorsPatch.LoadDeclaredParts();

    [HarmonyPatch(typeof(Module_Color), "OnInitialize")]
    public static void Postfix(Module_Color __instance)
    {
        if (ColorsPatch.DeclaredParts.Count == 0)
            return;
        string partName = ColorsPatch.TrimPartName(__instance.OABPart == null ? __instance.part.Name : __instance.OABPart.PartName);
        if (!((IEnumerable<string>)ColorsPatch.allParts).Contains<string>(partName))
            return;
        foreach (MeshRenderer componentsInChild in __instance.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (!(componentsInChild.material.shader.name != ColorsPatch.unityStandard.name))
            {
                Material material = new Material(ColorsPatch.ksp2Opaque);
                ColorsPatch.SetTexturesToMaterial(partName, ref material);
                componentsInChild.material = material;
                if (componentsInChild.material.shader.name != ColorsPatch.ksp2Opaque.name)
                    componentsInChild.SetMaterial(material);
            }
        }
       // __instance.SomeColorUpdated();
    }

    private static void LogMessage(object data) => ColorsPatch.Logger.LogMessage((object)string.Format("{0}", data));

    private static void LogWarning(object data) => ColorsPatch.Logger.LogWarning((object)string.Format("{0}", data));

    private static void LogError(object data) => ColorsPatch.Logger.LogError((object)string.Format("{0}", data));
}
