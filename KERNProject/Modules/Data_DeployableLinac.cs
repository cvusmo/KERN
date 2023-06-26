//data model represents state of the Linear Accelerator (Linac) module.
//(IsDeployed) stores the state of the Linac, along with various attributes to control its behavior and appearance.



using KSP.Sim;
using KSP.Sim.Definitions;
using UnityEngine;
using UnityEngine.Serialization;

namespace KERN.Modules
{
    [Serializable]
    public class Data_DeployableLinac : ModuleData
    {
        [LocalizedField("KERN/DCS/DEBUG/IsDeployed")]
        [KSPState(CopyToSymmetrySet = true)]
        [FormerlySerializedAs("isDeployed")]
        [Tooltip("Current Linac State")]
        public ModuleProperty<bool> IsDeployed = new ModuleProperty<bool>(false, true);
        public override Type ModuleType => typeof(Module_Linac);

    }
}