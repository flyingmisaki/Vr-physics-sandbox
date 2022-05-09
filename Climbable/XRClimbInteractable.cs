using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRClimbInteractable : XRBaseInteractable {
    protected override void OnSelectEntered(XRBaseInteractor interactor) {
        base.OnSelectEntering(interactor);
        Climber.climbingHand = interactor.GetComponent<XRController>();
    }

    protected override void OnSelectExited(XRBaseInteractor interactor) {
        base.OnSelectExiting(interactor);
        if (Climber.climbingHand && Climber.climbingHand.name == interactor.name) Climber.climbingHand = null;
    }
}
