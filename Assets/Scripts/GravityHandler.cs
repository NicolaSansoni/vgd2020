using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHandler : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 direction;
    private float magnitude;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        direction = Physics.gravity.normalized;
        magnitude = Physics.gravity.magnitude;
    }

    public Vector3 get() {
        return direction * magnitude;
    }
    public void setDirection(Vector3 direction) {
        this.direction = direction.normalized;
    }

    private void FixedUpdate() {
        rb.AddForce(get(), ForceMode.Acceleration);
    }
}
