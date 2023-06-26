//data model that represents the state of the Linear Accelerator (Linac). Here, you would define the properties that represent the state of the Linac, like whether it is deployed or not.

using KSP.Sim.Definitions;
using KSP.Modules;
using KSP.Game;
using KSP.Sim;
using System;

namespace KERN.Modules
{
    [Serializable]
    public class Data_DeployableLinac : ModuleData
    {
        public ModuleProperty<bool> IsDeployed = new ModuleProperty<bool>(false, true);

        public override Type ModuleType => typeof(Module_Linac);

        public override void OnStart(double universalTime)
        {
        }

        public override void OnUpdate(double universalTime, double deltaUniversalTime)
        {
        }
    }
}
