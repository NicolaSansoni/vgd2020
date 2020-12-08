using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public InputHandler input;
    public Transform pointOfView;
    public float jumpSpeed = 5f;
    public float maxSpeed = 6f;
    public float acceleration = 10f;
    public float rotXSec = 2f;

    private Rigidbody rb;
    private Vector3 movInput;
    private bool jmpInput;
    private Vector3 planeVel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movInput = new Vector3();
        jmpInput = false;
        planeVel = Vector3.zero;
    }

    private void Update() {
        movInput.x = input.getMovement().x;
        movInput.z = input.getMovement().y;

        if (input.getJump()) {
            jmpInput = true;
        }

        // keep char facing forward when moving, smooth the rotation
        if (planeVel.magnitude > 0.1f) {
            Quaternion fwdRot = Quaternion.LookRotation(planeVel, -Physics.gravity);
            fwdRot = Quaternion.RotateTowards(transform.rotation, fwdRot, 360f * rotXSec * Time.deltaTime);
            transform.rotation = fwdRot;
        }
    }

    void FixedUpdate()
    {
        planeVel = Vector3.ProjectOnPlane(rb.velocity, Physics.gravity);
        Vector3 velTarget = movInput * maxSpeed;
        // align with the camera
        Vector3 fwd = Vector3.ProjectOnPlane(pointOfView.forward, Physics.gravity);
        if (fwd.Equals(Vector3.zero)) {
            fwd = Vector3.ProjectOnPlane(pointOfView.up, Physics.gravity);
        }
        velTarget = Quaternion.FromToRotation(Vector3.forward, fwd) * velTarget;
        
        Vector3 velDiff = velTarget - planeVel;
        // clamp instead of normalize to handle standing still correctly
        Vector3 accelVect = Vector3.ClampMagnitude(velDiff, 1f) * acceleration;
        rb.AddForce(accelVect, ForceMode.Acceleration);

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
        return planeVel.magnitude;
    }
    public bool getJump() {
        return jmpInput;
    }
    public bool getGrounded() {
        float margin = 0.1f;
        // ray needs to start above the ground because if it starts inside no hit is registered
        Vector3 feetPos = transform.position - Physics.gravity.normalized * margin / 2;
        return Physics.Raycast(feetPos, Physics.gravity, margin);
    }
}
