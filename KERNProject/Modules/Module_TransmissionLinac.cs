using KSP.Modules;
using KSP.Sim.Definitions;
using KSP.Sim.impl;
using UnityEngine;

namespace KERN.Modules
{
    internal class Module_TransmissionLinac : PartBehaviourModule
    {
        [SerializeField]
        protected Data_Transmitter dataTransmitter;

        [SerializeField]
        protected Data_DeployableLinac dataLinac; // New data module for linear accelerator

        public override System.Type PartComponentModuleType => typeof(PartComponentModule_DataTransmitter);

        protected override void AddDataModules()
        {
            base.AddDataModules();
            this.DataModules.TryAddUnique<Data_Transmitter>(this.dataTransmitter, out this.dataTransmitter);
            this.DataModules.TryAddUnique<Data_DeployableLinac>(this.dataLinac, out this.dataLinac); // Add linear accelerator data module
        }
    }
}
