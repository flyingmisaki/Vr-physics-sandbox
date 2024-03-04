using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerBodyShrink : MonoBehaviour {
    [Header("Lower Body Parts")]
    public GameObject Fender;
    public GameObject Sphere;

    void OnCollisionStay(Collision collision) {
        Sphere.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        Fender.transform.localScale = new Vector3(0.375f, 0.375f, 0.375f);
    }

    void OnCollisionExit(Collision collision) {
        Sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Fender.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
}
