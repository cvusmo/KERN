//controller class that manages the logic and interaction with the Linac. Here, you would define the methods to control the Linac, like extending or retracting the Linac, and controlling the animation.

using KSP.Modules;
using KSP.Sim;
using KSP.Sim.Definitions;
using System.Collections;
using UnityEngine;

public class Module_Linac : Module_Deployable
{
    // Reference to the Data_Deployable object
    internal Data_Deployable data_DeployableLinac;

    // Animation components to control
    internal Animation PosMagnetController;
    internal Animation NegMagnetController;

    // GameObjects to control
    public GameObject PosMagnet;
    public GameObject NegMagnet;

    public bool IsAccelerating { get; private set; }

    // Called at the start
    public void InitializeLinac()
    {
        // Call base class's OnInitialize() to initialize Module_Deployable functionality
        base.OnInitialize();

        data_DeployableLinac = new Data_Deployable();

        // Assuming 'IsExtended' is a bool variable in Data_Deployable that indicates whether the Linac is extended or not.
        //this.AddActionGroupAction(new Action<bool>(ExtendMagnet), KSPActionGroup.Lights, "Toggle Control Surface", data_DeployableLinac.IsExtended);
        this.AddActionGroupAction(new Action(ExtendMagnet), KSPActionGroup.None, "Activate Control Surface");
        this.AddActionGroupAction(new Action(RetractMagnet), KSPActionGroup.None, "Deactivate Control Surface");

        // Get the Animation components
        PosMagnetController = PosMagnet.GetComponent<Animation>();
        NegMagnetController = NegMagnet.GetComponent<Animation>();

        data_DeployableLinac.CurrentDeployState.SetValue(Data_Deployable.DeployState.Retracted);

        // Make sure the magnets are not active in the beginning
        ToggleAccelerator(false);
    }

    public void EnableLinac()
    {
        base.OnEnable();

        RetractMagnet();
    }

    // A method to start the extension animation
    public void ExtendMagnet()
    {
        // Set deploy state
        data_DeployableLinac.CurrentDeployState.SetValue(Data_Deployable.DeployState.Extending);

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

    public void RetractMagnet()
    {
        // Set deploy state
        data_DeployableLinac.CurrentDeployState.SetValue(Data_Deployable.DeployState.Retracting);

        // Start slowing down and stopping the animations
        StartCoroutine(SlowDownAndStop(PosMagnetController));
        StartCoroutine(SlowDownAndStop(NegMagnetController));
    }

    public void ToggleAccelerator(bool _accelerationState)
    {
        IsAccelerating = _accelerationState;
    }

    // Called every frame
    public void Update()
    {
        // Update the state based on the animations.
        if (!PosMagnetController.isPlaying && !NegMagnetController.isPlaying)
        {
            if (data_DeployableLinac.CurrentDeployState.GetValue() == Data_Deployable.DeployState.Extending)
            {
                data_DeployableLinac.CurrentDeployState.SetValue(Data_Deployable.DeployState.Extended);
            }
            else if (data_DeployableLinac.CurrentDeployState.GetValue() == Data_Deployable.DeployState.Retracting)
            {
                data_DeployableLinac.CurrentDeployState.SetValue(Data_Deployable.DeployState.Retracted);
            }
        }
    }
}
