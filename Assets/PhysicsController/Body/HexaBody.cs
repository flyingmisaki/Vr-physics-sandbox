using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class HexaBody : MonoBehaviour {
    // Public inspector fields
    [Header("XR Rig")]
    public GameObject PlayerController;
    public XROrigin XROrigin;
    public GameObject XRCamera;
    public GameObject CameraOffset;

    [Header("Controllers")]
    public ActionBasedController CameraController;
    public ActionBasedController RightHandController;
    public ActionBasedController LeftHandController;

    public InputActionReference RightTrackPadPress;
    public InputActionReference RightTrackPadTouch;
    public InputActionReference LeftTrackPadPress;
    public InputActionReference LeftTrackPadTouch;

    public InputActionReference RightPrimaryPress;
    public InputActionReference RightSecondaryPress;

    [Header("Hexabody")]
    public GameObject Body;
    public GameObject Head;
    public GameObject Chest;
    public GameObject Fender;
    public GameObject Sphere;
    public GameObject RightHand;
    public GameObject LeftHand;

    public ConfigurableJoint RightHandJoint;
    public ConfigurableJoint LeftHandJoint;
    public ConfigurableJoint Spine;

    [Header("Movement")]
    public float turnForce = 2.5f;
    public float moveForceCrouch = 5;
    public float moveForceWalk = 15;
    public float moveForceSprint = 20;

    [Header("Drag")]
    public float angularDragOnMove = 35;
    public float angularBreakDrag = 100;

    [Header("Crouch and Jump")]
    public float jumpPreloadForce = 1.3f;
    public float jumpReleaseForce = 1.3f;
    public float jumpMinCrouch = 0.125f;
    public float crouchForce = 0.005f;
    public float minCrouch = 0f;
    public float maxCrouch = 1.8f;
    public Vector3 crouchTarget;

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

    private float rightSecondaryPressed;

    // Body fields
    private bool jumping = false;
    private bool moving = false;
    // private bool crouching = false;
    private bool tiptoeing = false;
    
    private float originalHeight;
    private float additionalHeight;
    private Vector3 climbingInitialPosition;

    private Quaternion headYaw;
    private Vector3 moveDirection;
    private Vector3 sphereTorque;

    // On script start
    void Start() {
        GetControllerInputs();
        Initialize();
    }

    // On every physics update
    private void FixedUpdate() {
        GetControllerInputs();
        CalculateDirection();
        MoveAndRotateHands();
        MoveAndRotateBody();
        RigToBody();
        Jump();
        if (!jumping) Crouch();
        // Debugs();
    }

    private void Debugs() {
        Debug.Log("Jumping: " + jumping);
        Debug.Log("Moving: " + moving);
        Debug.Log("Tiptoeing: " + tiptoeing);
    }

    // Initialize player's height
    private void Initialize() {
        originalHeight = (0.5f * Sphere.transform.lossyScale.y) + (0.5f * Fender.transform.lossyScale.y) + (Head.transform.position.y - Chest.transform.position.y);
        additionalHeight = originalHeight;
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

        // Headset controller position & rotation
        cameraControllerPosition = CameraController.positionAction.action.ReadValue<Vector3>();
        cameraControllerRotation = CameraController.rotationAction.action.ReadValue<Quaternion>();
    }

    // Calculates body and movement values
    private void CalculateDirection() {
        // Values
        headYaw = Quaternion.Euler(0, XRCamera.transform.eulerAngles.y, 0);
        moveDirection = headYaw * new Vector3(leftTrackpadValue.x, 0, leftTrackpadValue.y);
        sphereTorque = new Vector3(moveDirection.z, 0, -moveDirection.x);
    }

    // Camera and Rig stuff
    private void RigToBody() {
        // Roomscale
        Body.transform.position = new Vector3(CameraController.transform.position.x, Body.transform.position.y, CameraController.transform.position.z);
        XRCamera.transform.rotation = CameraController.transform.rotation;
        
        // No roomscale
        // Body.transform.position = cameraControllerPosition;
        // XRCamera.transform.position = Head.transform.position;
        // XROrigin.transform.position = new Vector3(Fender.transform.position.x, Fender.transform.position.y - (0.5f * Fender.transform.localScale.y + 0.5f * Sphere.transform.localScale.y), Fender.transform.position.z);
    }

    // Movement
    private void MoveAndRotateBody() {
        RotateBody();
        MoveBody();
    }

    // Rotates Rig AND Body
    private void RotateBody() {
        Chest.transform.rotation = headYaw;
        Fender.transform.rotation = headYaw;
        if (rightTrackpadPressed == 1) return;
        if (rightTrackpadValue.x > 0.25f || rightTrackpadValue.x < -0.25f) {
            Head.transform.Rotate(0, rightTrackpadValue.x * turnForce, 0, Space.Self);
            XROrigin.transform.RotateAround(Body.transform.position, Vector3.up, rightTrackpadValue.x * turnForce);
        }
    }
    
    // Sphere control on input
    private void MoveBody() {
        if (leftTrackpadTouched == 0) StopSphere();
        if (leftTrackpadTouched == 1 && leftTrackpadPressed == 0) MoveSphere(moveForceWalk);
        if (leftTrackpadTouched == 1 && leftTrackpadPressed == 1) MoveSphere(moveForceSprint);
        if (jumping && leftTrackpadTouched == 1) MoveSphere(moveForceCrouch);
    }

    // Add torque to sphere for body movement
    private void MoveSphere(float force) {
        Sphere.GetComponent<Rigidbody>().freezeRotation = false;
        moving = true;
        Sphere.GetComponent<Rigidbody>().angularDrag = angularDragOnMove;
        Sphere.GetComponent<Rigidbody>().AddTorque(sphereTorque * (force * 2), ForceMode.Force);
    }

    // Stops sphere and freezes its rotation
    private void StopSphere() {
        Sphere.GetComponent<Rigidbody>().angularDrag = angularBreakDrag;
        if (Sphere.GetComponent<Rigidbody>().velocity == Vector3.zero) Sphere.GetComponent<Rigidbody>().freezeRotation = true;
        moving = false;
    }

    // Jump control on input
    private void Jump() {
        bool jumpButtonPressed = rightPrimaryPressed == 1 || rightTrackpadPressed == 1;
        if (jumpButtonPressed) JumpPreload();
        else if (jumping == true) JumpRelease();
    }

    // Virtual crouch for jump
    private void JumpPreload() {
        jumping = true;
        crouchTarget.y = Mathf.Clamp(crouchTarget.y -= jumpPreloadForce * Time.fixedDeltaTime, jumpMinCrouch, maxCrouch);
        Spine.targetPosition = new Vector3(0, crouchTarget.y, 0);
    }

    // Virtual crouch release for jump
    private void JumpRelease() {
        jumping = false;
        crouchTarget.y = Mathf.Clamp(crouchTarget.y += jumpReleaseForce * Time.fixedDeltaTime, jumpMinCrouch, maxCrouch);
        Spine.targetPosition = new Vector3(0, crouchTarget.y, 0);
    }

    // Crouch control
    private void Crouch() {
        VirtualCrouch();
        PhysicalCrouch();
        if (rightSecondaryPressed == 1) ResetCrouchHeight();
    }

    // Resets height to originalHeight calculated at Start()
    private void ResetCrouchHeight() {
        additionalHeight = originalHeight;
    }

    // Additional height on input for virtual crouch 
    private void VirtualCrouch() {
        if (rightTrackpadValue.y < -0.85f) additionalHeight += crouchForce;
        if (rightTrackpadValue.y > 0.85f) {
            tiptoeing = true;
            additionalHeight -= crouchForce;
        }
        if (tiptoeing == true && rightTrackpadValue.y < 0.85f && additionalHeight < originalHeight) {
            ResetCrouchHeight();
            tiptoeing = false;
        }
    }

    // Physical crouch dictated by head height and additional height based on virtual crouch
    private void PhysicalCrouch() {
        crouchTarget.y = Mathf.Clamp(cameraControllerPosition.y - additionalHeight, minCrouch, maxCrouch - originalHeight);
        Spine.targetPosition = new Vector3(0, crouchTarget.y, 0);
    }

    // Moves and rotates hands with a target
    private void MoveAndRotateHands() {
        RightHandJoint.targetPosition = rightHandControllerPosition - cameraControllerPosition;
        LeftHandJoint.targetPosition = leftHandControllerPosition - cameraControllerPosition;
        RightHandJoint.targetRotation = rightHandControllerRotation;
        LeftHandJoint.targetRotation = leftHandControllerRotation;
    }
}