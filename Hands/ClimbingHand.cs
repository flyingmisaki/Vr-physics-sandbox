using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class ClimbingHand : MonoBehaviour {
    [Header("Physics Hand")]
    public GameObject Hand;
    public ActionBasedController HandController;
    public InputActionReference ControllerSelect;

    private float ControllerSelected;
    private GameObject CollidingObject = null;
    private bool Climbing = false;

    void FixedUpdate() {
        GetInput();
        Climb();
    }

    void GetInput() {
        ControllerSelected = ControllerSelect.action.ReadValue<float>();
    }
    
    void Climb() {
        // if (ControllerSelected == 1 && collision.gameObject.tag == "Climbable") Attach();
        if (ControllerSelected == 1 && CollidingObject && !Climbing) Attach();
        if (ControllerSelected == 0 && Climbing) Release();
    }

    void Attach() {
        Hand.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        Physics.IgnoreCollision(Hand.GetComponent<Collider>(), CollidingObject.GetComponent<Collider>(), true);
        Climbing = true;
    }

    void Release() {
        Hand.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Physics.IgnoreCollision(Hand.GetComponent<Collider>(), CollidingObject.GetComponent<Collider>(), false);
        Climbing = false;
    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision) {
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (Climbing) return;
        if (collision.gameObject.tag == "Climbable") {
            CollidingObject = collision.gameObject;
            Debug.Log("Colliding!!");
        }
    }

    void OnCollisionExit(Collision collision) {
        if (Climbing) return;
        if (collision.gameObject.tag == "Climbable") {
            CollidingObject = null;
        }
    }
}