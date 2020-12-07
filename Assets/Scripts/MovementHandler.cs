using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public float jumpSpeed = 5f;
    public float acceleration = 10f;

    private Rigidbody rb;
    private InputHandler input;
    private Vector3 movInput;
    private bool jmpInput;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputHandler>();
        rb = GetComponent<Rigidbody>();
        movInput = new Vector3();
        jmpInput = false;
    }

    private void Update() {
        movInput.x = input.getMovement().x;
        movInput.z = input.getMovement().y;

        if (input.getJump()) {
            jmpInput = true;
        }

        Vector3 forward = Vector3.ProjectOnPlane(rb.velocity, Physics.gravity);
        if (forward.magnitude < 0.1f) forward = transform.forward;
        transform.rotation = Quaternion.LookRotation(forward, -Physics.gravity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(movInput * acceleration, ForceMode.Acceleration);
        if (jmpInput) {
            Jump();
            jmpInput = false;
        }
    }

    private void Jump() {
        Vector3 v = rb.velocity;
        v.y = jumpSpeed;
        rb.velocity = v;
    }

    public float getSpeed() {
        return Vector3.ProjectOnPlane(rb.velocity, Physics.gravity).magnitude;
    }
    public bool getJump() {
        return jmpInput;
    }
    public bool getGrounded() {
        float margin = 0.1f;
        Vector3 feetPos = transform.position - Physics.gravity.normalized * margin / 2;
        return Physics.Raycast(feetPos, Physics.gravity, margin);
    }
}
