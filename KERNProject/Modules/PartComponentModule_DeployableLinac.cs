//managing the interaction between your Linac part and the game's component system. Here, you would define methods to interact with the game's part system, like adding your Linac to the part system.

using KSP.Modules;
using KSP.Sim.impl;

namespace KERN.Modules
{
    public class PartComponentModule_DeployableLinac : PartComponentModule_Deployable
    {
        Module_Linac module_Linac;
        public Type PartBehaviourModuleType => typeof(PartComponentModule_DeployableLinac);

        public override void OnStart(double universalTime)
        {
            base.OnStart(universalTime);

            module_Linac = new Module_Linac();
            module_Linac.dataDeployableLinac.IsDeployed.SetValue(module_Linac._dataDeployable.IsRetracted);
        }

        public override void OnUpdate(double universalTime, double deltaUniversalTime)
        {
            base.OnUpdate(universalTime, deltaUniversalTime);

            module_Linac.Update();
        }
    }
}