using KERN.Modules;
using KSP.Sim.impl;
using System;

namespace KERN.Modules
{
    public class PartComponentModule_DeployableControlSurface : PartComponentModule_ControlSurface
    {
        public override Type PartBehaviourModuleType => typeof(Module_DeployableControlSurface);

        public override void OnStart(double universalTime)
        {
        }

        public override void OnUpdate(double universalTime, double deltaUniversalTime)
        {
        }
    }
}
