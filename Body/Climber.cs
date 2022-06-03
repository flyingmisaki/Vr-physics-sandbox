using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Climber : MonoBehaviour {
    public GameObject character;
    public static XRController climbingHand;
    
    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void FixedUpdate() {
        if (climbingHand) Climb();
    }

    // Climbing positions, ect...
    void Climb() {
        InputDevices.GetDeviceAtXRNode(climbingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
        character.transform.Translate(-velocity * Time.fixedDeltaTime);
    }
}
