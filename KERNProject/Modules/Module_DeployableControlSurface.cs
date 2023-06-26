using I2.Loc;
using KERN.Modules;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.Definitions;
using System;
using UnityEngine;

namespace KERN.Modules
{
    [DisallowMultipleComponent]
    public class Module_DeployableControlSurface : Module_ControlSurface
    {
        public Animator animator;
        [SerializeField]
        protected Data_DeployableControlSurface dataDeployableControlSurface = new Data_DeployableControlSurface();

        public override Type PartComponentModuleType => typeof(PartComponentModule_DeployableControlSurface);

        public override void OnInitialize()
        {
            base.OnInitialize();
            this.AddActionGroupAction(new Action<bool>(this.SetControlSurfaceActiveState), KSPActionGroup.Brakes, "Toggle Control Surface", this.dataDeployableControlSurface.IsDeployed);
            this.AddActionGroupAction(new Action(this.SetControlSurfaceActiveStateOn), KSPActionGroup.None, "Activate Control Surface");
            this.AddActionGroupAction(new Action(this.SetControlSurfaceActiveStateOff), KSPActionGroup.None, "Deactivate Control Surface");
            this.UpdatePAMControlVisibility();
        }

        public override void AddDataModules()
        {
            base.AddDataModules();
            if (this.DataModules.TryGetByType<Data_DeployableControlSurface>(out this.dataDeployableControlSurface))
                return;
            this.dataDeployableControlSurface = new Data_DeployableControlSurface();
            this.DataModules.TryAddUnique<Data_DeployableControlSurface>(this.dataDeployableControlSurface, out this.dataDeployableControlSurface);
        }

        public override void CtrlSurfaceUpdate(Vector3 vel, float deltaTime)
        {
            if (!this.dataDeployableControlSurface.IsDeployed.GetValue())
                return;
            base.CtrlSurfaceUpdate(vel, deltaTime);
        }

        private void SetDragCubes(bool deployed)
        {
            this._dataDrag.SetCubeWeight("Deployed", deployed ? 1f : 0.0f);
            this._dataDrag.SetCubeWeight("Retracted", deployed ? 0.0f : 1f);
        }

        public override string GetModuleDisplayName() => LocalizationManager.GetTranslation("PartModules/ControlSurface/Name", true, 0, true, false, (GameObject)null, (string)null, true);

        private void SetControlSurfaceActiveState(bool newState)
        {
            this.animator.SetBool("Deployed", newState);
            this.dataDeployableControlSurface.IsDeployed.SetValue(newState);
            this.SetDragCubes(newState);
        }

        private void SetControlSurfaceActiveStateOn() => this.SetControlSurfaceActiveState(true);

        private void SetControlSurfaceActiveStateOff() => this.SetControlSurfaceActiveState(false);

        public override void UpdatePAMControlVisibility()
        {
            base.UpdatePAMControlVisibility();
            this.dataDeployableControlSurface.SetLabel((IModuleProperty)this.dataDeployableControlSurface.IsDeployed, "Debug/IsDeployed");
        }
    }
}
