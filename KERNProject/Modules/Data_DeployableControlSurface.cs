using KSP.Sim;
using KSP.Sim.Definitions;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace KERN.Modules
{
    [Serializable]
    public class Data_DeployableControlSurface : ModuleData
    {
        [LocalizedField("KERN/DCS/DEBUG/IsDeployed")]
        [KSPState(CopyToSymmetrySet = true)]
        [FormerlySerializedAs("isDeployed")]
        [Tooltip("Current Control Surface State")]
        public ModuleProperty<bool> IsDeployed = new ModuleProperty<bool>(false, true);

        public override Type ModuleType => typeof(Module_DeployableControlSurface);
    }
}
