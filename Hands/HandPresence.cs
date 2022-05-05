using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour {
    public InputDeviceCharacteristics controllerCharacteristics;    
    private InputDevice targetDevice;
    public Animator handAnimator;

    private Collider[] handColliders;

    void Start() {
        TryInitialize();
        handColliders = GetComponentsInChildren<Collider>();
    }

    void TryInitialize() {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }

    void UpdateHandAnimation() {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    public void EnableHandColliderDelay(float delay) {
        Invoke("EnableHandCollider", delay);
    }

    public void EnableHandCollider() {
        foreach (var item in handColliders) item.enabled = true;
    }

    public void DisableHandCollider() {
        foreach (var item in handColliders) item.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (!targetDevice.isValid) {
            TryInitialize();
        }
        else {
            UpdateHandAnimation();
        }
    }
}
