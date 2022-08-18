using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    [Header("Controllers")]
    public ActionBasedController CameraController;
    public ActionBasedController RightHandController;
    public ActionBasedController LeftHandController;

    public InputActionReference RightTrackPadPress;
    public InputActionReference LeftTrackPadPress;

    public InputActionReference RightTrackPadTouch;
    public InputActionReference LeftTrackPadTouch;

    public InputActionReference RightPrimaryPress;
    public InputActionReference LeftPrimaryPress;

    public InputActionReference RightSecondaryPress;
    public InputActionReference LeftSecondaryPress;

    // Input fields
    private Vector3 cameraControllerPosition;
    private Vector3 rightHandControllerPosition;
    private Vector3 leftHandControllerPosition;

    private Quaternion cameraControllerRotation;
    private Quaternion rightHandControllerRotation;
    private Quaternion leftHandControllerRotation;

    private Vector2 rightTrackpadValue;
    private Vector2 leftTrackpadValue;

    private float rightTrackpadPressed;
    private float leftTrackpadPressed;

    private float rightTrackpadTouched;
    private float leftTrackpadTouched;

    private float rightPrimaryPressed;
    private float leftPrimaryPressed;

    private float rightSecondaryPressed;
    private float leftSecondaryPressed;

    void FixedUpdate() {
        GetControllerInputs();
    }

    // Gets controller inputs
    private void GetControllerInputs() {
        // Right controller position & rotation
        rightHandControllerPosition = RightHandController.positionAction.action.ReadValue<Vector3>();
        rightHandControllerRotation = RightHandController.rotationAction.action.ReadValue<Quaternion>();
        // Right trackpad value, press and touch
        rightTrackpadValue = RightHandController.translateAnchorAction.action.ReadValue<Vector2>();
        rightTrackpadPressed = RightTrackPadPress.action.ReadValue<float>();
        rightTrackpadTouched = RightTrackPadTouch.action.ReadValue<float>();
        // Right primary and secondary press
        rightPrimaryPressed = RightPrimaryPress.action.ReadValue<float>();
        rightSecondaryPressed = RightSecondaryPress.action.ReadValue<float>();

        // Left contoller position & rotation
        leftHandControllerPosition = LeftHandController.positionAction.action.ReadValue<Vector3>();
        leftHandControllerRotation = LeftHandController.rotationAction.action.ReadValue<Quaternion>();
        // Left trackpad value, press and touch
        leftTrackpadValue = LeftHandController.translateAnchorAction.action.ReadValue<Vector2>();
        leftTrackpadPressed = LeftTrackPadPress.action.ReadValue<float>();
        leftTrackpadTouched = LeftTrackPadTouch.action.ReadValue<float>();
        // Left primary and secondary press
        leftPrimaryPressed = LeftPrimaryPress.action.ReadValue<float>();
        leftSecondaryPressed = LeftSecondaryPress.action.ReadValue<float>();
        
        // Headset controller position & rotation
        cameraControllerPosition = CameraController.positionAction.action.ReadValue<Vector3>();
        cameraControllerRotation = CameraController.rotationAction.action.ReadValue<Quaternion>();
    }
}
