using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRClimbInteractable : XRBaseInteractable {
    protected override void OnSelectEntered(XRBaseInteractor interactor) {
        base.OnSelectEntering(interactor);
    }

    protected override void OnSelectExited(XRBaseInteractor interactor) {
        base.OnSelectExiting(interactor);
    }
}
