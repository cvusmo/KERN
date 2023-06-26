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
        // This property will store whether the linear accelerator is deployed or not
        public ModuleProperty<bool> IsDeployed = new ModuleProperty<bool>(false, true);

        public override Type ModuleType => typeof(Module_Linac);

        public override void OnStart(double universalTime)
        {
            // Here you could set the initial state of IsDeployed based on game state
        }

        public override void OnUpdate(double universalTime, double deltaUniversalTime)
        {
            // Here you could update the state of IsDeployed based on game state
        }
    }
}
