using KSP.Modules;
using KSP.Sim.impl;

namespace KERN.Modules
{
    public class PartComponentModule_DeployableLinac : PartComponentModule_Deployable
    {
        public Type PartBehaviourModuleType => typeof(PartComponentModule_DeployableLinac);

        public override void OnStart(double universalTime)
        {
            base.OnStart(universalTime);

        }

        public override void OnUpdate(double universalTime, double deltaUniversalTime)
        {
            base.OnUpdate(universalTime, deltaUniversalTime);
            // Perform any necessary updates here
        }
    }
}