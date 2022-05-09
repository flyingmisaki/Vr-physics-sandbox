using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Climber : MonoBehaviour {
    private GameObject player;
    public static XRController climbingHand;

    void Start() {
        player = GetComponent<GameObject>();
    }

    void FixedUpdate() {
        if (climbingHand) Climb();
    }

    void Climb() {
        InputDevices.GetDeviceAtXRNode(climbingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
        player.transform.Translate(-velocity * Time.fixedDeltaTime);
    }
}
