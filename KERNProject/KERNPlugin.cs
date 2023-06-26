using BepInEx;
using HarmonyLib;
using KSP.Game;
using KSP.Messages;
using SpaceWarp;
using SpaceWarp.API.Mods;
using UnityEngine;

namespace KERN
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class KERNPlugin : BaseSpaceWarpPlugin
    {
        public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
        public const string ModName = MyPluginInfo.PLUGIN_NAME;
        public const string ModVer = MyPluginInfo.PLUGIN_VERSION;
        public static KERNPlugin Instance { get; set; }
        public static string Path { get; private set; }

        public override void OnPreInitialized()
        {
            KERNPlugin.Path = this.PluginFolderPath;
        }
        public override void OnInitialized()
        {
            base.OnInitialized();


            ColorsPatch.DeclareParts("KERN", (IEnumerable<string>)new List<string>()
            {
                "waveguide",
                "collector",
                "electrongun"
            });
            KERN.KERNPlugin.Instance = this;
            Harmony.CreateAndPatchAll(typeof(KERNPlugin).Assembly, (string)null);
        }
        public override void OnPostInitialized() => base.OnPostInitialized();

    }
}