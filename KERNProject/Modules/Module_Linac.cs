//controller class that manages the logic and interaction with the Linac. Here, you would define the methods to control the Linac, like extending or retracting the Linac, and controlling the animation.

using KERN.Modules;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.Definitions;
using System.Collections;
using UnityEngine;

public class Module_Linac : Module_Deployable
{
    // Reference to the Data_Deployable object
    internal Data_Deployable _dataDeployable;

    // Animation components to control
    public Animator animator;
    [SerializeField]

    internal Animation PosMagnetController;
    internal Animation NegMagnetController;

    // GameObjects to control
    internal GameObject PosMagnet;
    internal GameObject NegMagnet;

    internal bool IsAccelerating { get; private set; }

    internal Data_DeployableLinac dataDeployableLinac = new Data_DeployableLinac();
    public override Type PartComponentModuleType => typeof(PartComponentModule_DeployableLinac);

    // Called at the start
    public void InitializeLinac()
    {
        base.OnInitialize();
        this.AddActionGroupAction(new Action<bool>(ExtendMagnet), KSPActionGroup.Lights, "Toggle Control Surface", dataDeployableLinac.IsDeployed);
        this.AddActionGroupAction(new Action<bool>(ExtendMagnet), KSPActionGroup.None, "Activate Control Surface");
        this.AddActionGroupAction(new Action(RetractMagnet), KSPActionGroup.None, "Deactivate Control Surface");

        // Get the Animation components
        PosMagnetController = PosMagnet.GetComponent<Animation>();
        NegMagnetController = NegMagnet.GetComponent<Animation>();

        // Handle start states
        dataDeployableLinac.IsDeployed.SetValue(_dataDeployable.IsRetracted);
        
        // Make sure the magnets are not active in the beginning
        ToggleAccelerator(false);
    }

    public void EnableLinac()
    {
        base.OnEnable();

        RetractMagnet();
    }

    // A method to start the extension animation
    internal void ExtendMagnet(bool value)
    {
        // Set deploy state
        dataDeployableLinac.IsDeployed.SetValue(value);

        // Begin playing the animation from its current state
        PosMagnetController.Play("PosMagnet");  // Replace with the actual animation clip name
        NegMagnetController.Play("NegMagnet");  // Replace with the actual animation clip name
    }

    // A method to start the retraction animation
    internal IEnumerator SlowDownAndStop(Animation animation)
    {
        while (animation.isPlaying)
        {
            animation[animation.clip.name].speed -= Time.deltaTime;
            yield return null;
        }
        animation.Stop();
    }

    internal void RetractMagnet()
    {
        // Set deploy state
        dataDeployableLinac.IsDeployed.SetValue(_dataDeployable.IsRetracting);

        // Start slowing down and stopping the animations
        StartCoroutine(SlowDownAndStop(PosMagnetController));
        StartCoroutine(SlowDownAndStop(NegMagnetController));
    }

    internal void ToggleAccelerator(bool _accelerationState)
    {
        IsAccelerating = _accelerationState;
    }

    protected override void AddDataModules()
    {
        base.AddDataModules();
        if (this.DataModules.TryGetByType<Data_DeployableLinac>(out this.dataDeployableLinac))
            return;
        this.dataDeployableLinac = new Data_DeployableLinac();
        this.DataModules.TryAddUnique<Data_DeployableLinac>(this.dataDeployableLinac, out this.dataDeployableLinac);
    }

    // Called every frame
    public void Update()
    {
        // Update the state based on the animations.
        if (!PosMagnetController.isPlaying && !NegMagnetController.isPlaying)
        {
            if (dataDeployableLinac.IsDeployed.GetValue() == _dataDeployable.IsExtending)
            {
                dataDeployableLinac.IsDeployed.SetValue(_dataDeployable.IsExtended);
            }
            else if (dataDeployableLinac.IsDeployed.GetValue() == _dataDeployable.IsRetracting)
            {
                dataDeployableLinac.IsDeployed.SetValue(_dataDeployable.IsRetracted);
            }
        }
    }
}