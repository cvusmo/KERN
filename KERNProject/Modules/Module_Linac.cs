using I2.Loc;
using KSP.Api;
using Newtonsoft.Json;
using KERN.Modules;
using KSP.Modules;
using KSP.OAB;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.Sim.State;
using KSP.Sim.impl;
using System;
using System.Collections;
using UnityEngine;

public class Module_Linac : Module_Deployable
{
    // Reference to the Data_Deployable object
    private Data_Deployable data_DeployableLinac;

    // Animation components to control
    private Animation PosMagnetAnimation;
    private Animation NegMagnetAnimation;

    // GameObjects to control
    public GameObject PosMagnet;
    public GameObject NegMagnet;

    // Called at the start
    public void OnStart()
    {
        base.OnStart(PartModuleStartState.Landed);

        // Get the Animation components
        PosMagnetAnimation = PosMagnet.GetComponent<Animation>();
        NegMagnetAnimation = NegMagnet.GetComponent<Animation>();
    }

    // A method to start the extension animation
    public void ExtendMagnet()
    {
        // Set deploy state
        data_DeployableLinac.CurrentDeployState.SetValue(Data_Deployable.DeployState.Extending);

        // Begin playing the animation from its current state
        PosMagnetAnimation.Play("PosMagnet");  // Replace with the actual animation clip name
        NegMagnetAnimation.Play("NegMagnet");  // Replace with the actual animation clip name
    }

    // A method to start the retraction animation
    private IEnumerator SlowDownAndStop(Animation animation)
    {
        while (animation.isPlaying)
        {
            animation[animation.clip.name].speed -= Time.deltaTime;
            yield return null;
        }
        animation.Stop();
    }

    // Create a context menu button for retracting the PosMagnet
    //[UIPartAction(guiName = "Retract PosMagnet")]
    public void RetractMagnet()
    {
        // Set deploy state
        data_DeployableLinac.CurrentDeployState.SetValue(Data_Deployable.DeployState.Retracting);

        // Start slowing down and stopping the animations
        StartCoroutine(SlowDownAndStop(PosMagnetAnimation));
        StartCoroutine(SlowDownAndStop(NegMagnetAnimation));
    }

    // Called every frame
    public void Update()
    {
        // If the animations are finished, update the deploy state
        if (!PosMagnetAnimation.isPlaying && !NegMagnetAnimation.isPlaying)
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
