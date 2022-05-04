using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class HexaBody : MonoBehaviour {
    [Header("XR Rig")]
    public XRRig XRRig;
    public GameObject XRCamera;

    [Header("Controllers")]
    public ActionBasedController CameraController;
    public ActionBasedController RightHandController;
    public ActionBasedController LeftHandController;

    public InputActionReference RightTrackPadPress;
    public InputActionReference RightTrackPadTouch;
    public InputActionReference LeftTrackPadPress;
    public InputActionReference LeftTrackPadTouch;

    [Header("Hexabody")]
    public GameObject Body;
    public GameObject Head;
    public GameObject Chest;
    public GameObject Fender;
    public GameObject Sphere;

    public ConfigurableJoint RightHandJoint;
    public ConfigurableJoint LeftHandJoint;
    public ConfigurableJoint Spine;

    [Header("Movement")]
    public float turnSpeed = 3;
    public float moveForceCrouch = 15;
    public float moveForceWalk = 30;
    public float moveForceSprint = 45;

    [Header("Drag")]
    public float angularDragOnMove = 40;
    public float angularBreakDrag = 100;

    [Header("Crouch and Jump")]
    public float crouchSpeed = 1.2f;
    public float minCrouch = 0.1f;
    public float maxCrouch = 1.8f;
    private float additionalHeight;
    public Vector3 crouchTarget;

    bool jumping = false;

    // Input
    private Quaternion headYaw;
    private Vector3 moveDirection;
    private Vector3 sphereTorque;

    private Vector3 cameraControllerPosition;
    private Vector3 rightHandControllerPosition;
    private Vector3 leftHandControllerPosition;

    private Quaternion rightHandControllerRotation;
    private Quaternion leftHandControllerRotation;

    private Vector2 RightTrackpad;
    private Vector2 LeftTrackpad;

    private float RightTrackpadPressed;
    private float LeftTrackpadPressed;

    private float RightTrackpadTouched;
    private float LeftTrackpadTouched;

    void Start() {
        additionalHeight = (0.5f * Sphere.transform.lossyScale.y) + (0.5f * Fender.transform.lossyScale.y) + (Head.transform.position.y - Chest.transform.position.y);
    }

    void Update() {}

    private void FixedUpdate() {
        GetControllerInputs();
        RigToBody();
        MoveAndRotateBody();
        MoveAndRotateHands();
        Ajust();
        Jump();
        if (!jumping) PhysicalCrouch();
    }

    private void GetControllerInputs() {
        // Right Controller Position & Rotation
        rightHandControllerPosition = RightHandController.positionAction.action.ReadValue<Vector3>();
        rightHandControllerRotation = RightHandController.rotationAction.action.ReadValue<Quaternion>();
        // Trackpad
        RightTrackpad = RightHandController.translateAnchorAction.action.ReadValue<Vector2>();
        RightTrackpadPressed = RightTrackPadPress.action.ReadValue<float>();
        RightTrackpadTouched = RightTrackPadTouch.action.ReadValue<float>();

        // Left Contoller Position & Rotation
        leftHandControllerPosition = LeftHandController.positionAction.action.ReadValue<Vector3>();
        leftHandControllerRotation = LeftHandController.rotationAction.action.ReadValue<Quaternion>();
        // Trackpad
        LeftTrackpad = LeftHandController.translateAnchorAction.action.ReadValue<Vector2>();
        LeftTrackpadPressed = LeftTrackPadPress.action.ReadValue<float>();
        LeftTrackpadTouched = LeftTrackPadTouch.action.ReadValue<float>();

        // Camera Inputs
        cameraControllerPosition = CameraController.positionAction.action.ReadValue<Vector3>();

        // Values
        headYaw = Quaternion.Euler(0, XRRig.cameraGameObject.transform.eulerAngles.y, 0);
        moveDirection = headYaw * new Vector3(LeftTrackpad.x, 0, LeftTrackpad.y);
        sphereTorque = new Vector3(moveDirection.z, 0, -moveDirection.x);
    }

    // Camera and Rig stuff
    private void RigToBody() {
        // Move Camera to Body
        XRCamera.transform.position = Head.transform.position;
        // Move Rig to Body
        XRRig.transform.position = new Vector3(Fender.transform.position.x, Fender.transform.position.y - (0.5f * Fender.transform.localScale.y + 0.5f * Sphere.transform.localScale.y), Fender.transform.position.z);
    }

    // Movement
    private void MoveAndRotateBody() {
        if (!jumping) {
            if (LeftTrackpadTouched == 0) StopSphere();
            if (LeftTrackpadPressed == 0 && LeftTrackpadTouched == 1) MoveSphere(moveForceWalk);
            if (LeftTrackpadPressed == 1 && LeftTrackpadTouched == 1) MoveSphere(moveForceSprint);
        }
        if (jumping) {
            if (LeftTrackpadTouched == 0) StopSphere();
            if (LeftTrackpadTouched == 1) MoveSphere(moveForceCrouch);
        }

        RotateBody();
    }

    private void RotateBody() {
        if (RightTrackpadPressed == 1) return;
        Head.transform.Rotate(0, RightTrackpad.x * turnSpeed, 0, Space.Self);
        Chest.transform.rotation = headYaw;
    }

    private void MoveSphere(float force) {
        Sphere.GetComponent<Rigidbody>().freezeRotation = false;
        Sphere.GetComponent<Rigidbody>().angularDrag = angularDragOnMove;
        Sphere.GetComponent<Rigidbody>().AddTorque(sphereTorque.normalized * (force * 2), ForceMode.Force);
    }

    private void StopSphere() {
        Sphere.GetComponent<Rigidbody>().angularDrag = angularBreakDrag;
        if (Sphere.GetComponent<Rigidbody>().velocity == Vector3.zero) Sphere.GetComponent<Rigidbody>().freezeRotation = true;
    }

    // Jump
    private void Jump() {
        if (RightTrackpadPressed == 1 && RightTrackpad.y < 0) JumpSitDown();
        if (RightTrackpadPressed == 0 && jumping == true) JumpSitUp();
    }

    private void Ajust() {
        if (RightTrackpad.y > 0) {
            crouchTarget.y += crouchSpeed * Time.fixedDeltaTime;
            Spine.targetPosition = new Vector3(0, crouchTarget.y, 0);
        }
        if (RightTrackpad.y < 0) {
            crouchTarget.y -= crouchSpeed * Time.fixedDeltaTime;
            Spine.targetPosition = new Vector3(0, crouchTarget.y, 0);
        }
    }

    private void JumpSitDown() {
        if (crouchTarget.y >= minCrouch) {
            jumping = true;
            crouchTarget.y -= crouchSpeed * Time.fixedDeltaTime;
            Spine.targetPosition = new Vector3(0, crouchTarget.y, 0);
        }
    }

    private void JumpSitUp() {
        jumping = false;
        crouchTarget = new Vector3(0, maxCrouch - additionalHeight, 0);
        Spine.targetPosition = crouchTarget;
    }

    // Joints
    private void PhysicalCrouch() {
        crouchTarget.y = Mathf.Clamp(cameraControllerPosition.y - additionalHeight, minCrouch, maxCrouch - additionalHeight);
        Spine.targetPosition = new Vector3(0, crouchTarget.y, 0);
    }

    private void MoveAndRotateHands() {
        RightHandJoint.targetPosition = rightHandControllerPosition - cameraControllerPosition;
        LeftHandJoint.targetPosition = leftHandControllerPosition - cameraControllerPosition;

        RightHandJoint.targetRotation = rightHandControllerRotation;
        LeftHandJoint.targetRotation = leftHandControllerRotation;
    }
}