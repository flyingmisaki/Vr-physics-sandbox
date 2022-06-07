using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class ClimbingHand : MonoBehaviour {
    [Header("Physics Hand")]
    public GameObject Hand;
    [Header("Action")]
    public InputActionReference ControllerSelect;
    [Header("Body")]
    public GameObject Body;

    private float ControllerSelected;
    private GameObject CollidingObject = null;
    public bool climbing = false;

    // On every physics tick
    void FixedUpdate() {
        GetInput();
        Climb();
    }

    // Get grip action from controller
    void GetInput() {
        ControllerSelected = ControllerSelect.action.ReadValue<float>();
    }
    
    // Climb control on input
    void Climb() {
        // if (ControllerSelected == 1 && collision.gameObject.tag == "Climbable") Attach();
        if (ControllerSelected == 1 && CollidingObject && !climbing) Attach();
        if (ControllerSelected == 0 && climbing) Release();
    }

    // Freeze hand position & rotation + disable collisions
    void Attach() {
        Hand.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Physics.IgnoreCollision(Hand.GetComponent<Collider>(), CollidingObject.GetComponent<Collider>(), true);
        climbing = true;
    }

    // Unfreeze
    void Release() {
        Hand.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Physics.IgnoreCollision(Hand.GetComponent<Collider>(), CollidingObject.GetComponent<Collider>(), false);
        climbing = false;
    }

    // Detect gameobject on collision
    void OnCollisionEnter(Collision collision) {
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (climbing) return;
        if (collision.gameObject.tag == "Climbable") {
            CollidingObject = collision.gameObject;
            // Debug.Log("Colliding!!");
        }
    }

    // Sets it null
    void OnCollisionExit(Collision collision) {
        if (climbing) return;
        if (collision.gameObject.tag == "Climbable") {
            CollidingObject = null;
        }
    }
}