using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public InputHandler input;
    public Transform pointOfView;
    public float jumpHeight = 3f;
    public float maxSpeed = 6f;
    public float acceleration = 20f;
    public float rotXSec = 2f;

    private Rigidbody rb;
    private ContactPoint[] contacts;
    private Vector3 wallNormal;
    private Vector3 groundNormal;
    private Vector3 movInput;
    private bool jmpInput;
    private Vector3 planeVel;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        contacts = new ContactPoint[10];
        for (int i = 0; i < contacts.Length; i++)
        {
            contacts[i] = new ContactPoint();
        }
        wallNormal = new Vector3();
        groundNormal = -Physics.gravity;
        movInput = new Vector3();
        jmpInput = false;
        planeVel = Vector3.zero;
        isGrounded = false;
    }

    void FixedUpdate()
    {
        /* get inputs */
        movInput.x = input.getMovement().x;
        movInput.z = input.getMovement().y;
        jmpInput = input.getJump();

        /* movement */
        planeVel = Vector3.ProjectOnPlane(rb.velocity, -Physics.gravity);
        Vector3 groundVel = Vector3.ProjectOnPlane(rb.velocity, groundNormal);
        // align with the camera
        Vector3 movTarget = movInput;
        Quaternion camUpRot = Quaternion.FromToRotation(pointOfView.up, Vector3.up);
        Vector3 camFwd = camUpRot * pointOfView.forward;
        Quaternion fwdRot = Quaternion.FromToRotation(Vector3.forward, camFwd);
        Quaternion upRot = Quaternion.FromToRotation(Vector3.up, groundNormal);
        movTarget = upRot * fwdRot * movTarget;
        // adjust for slopes
        float verticalComponent = Vector3.Dot(movTarget, -Physics.gravity.normalized);
        Debug.Log(verticalComponent);
        movTarget -= movTarget * verticalComponent;
        // difference between desired actual and velocity
        Vector3 velDiff = movTarget * maxSpeed - groundVel;
        // clamp instead of normalize to handle standing still correctly
        Vector3 accelVect = Vector3.ClampMagnitude(velDiff, 1f) * acceleration;
        // Stop sticking to walls
        wallNormal.Normalize();
        Debug.DrawRay(transform.position, groundNormal, Color.red);
        float wallComponent = Vector3.Dot(accelVect, wallNormal);
        if (wallComponent < 0)
        {
            accelVect -= wallComponent * wallNormal;
        }
        wallNormal = Vector3.zero;
        // apply
        rb.AddForce(accelVect, ForceMode.Acceleration);

        /* jump */
        if (jmpInput && getGrounded())
        {
            Jump();
        }

        /* rotation */
        if (planeVel.magnitude > 0.1f)
        {
            // keep char facing forward when moving, smooth the rotation
            Quaternion charRot = Quaternion.LookRotation(planeVel, -Physics.gravity);
            charRot = Quaternion.RotateTowards(transform.rotation, charRot, 360f * rotXSec * Time.fixedDeltaTime);
            rb.MoveRotation(charRot);
        }
    }

    private void Jump()
    {
        float jumpSpeed = Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude);
        rb.velocity = planeVel + -Physics.gravity.normalized * jumpSpeed;
    }

    public float getSpeed()
    {
        return planeVel.magnitude;
    }
    public bool getJump()
    {
        return jmpInput && getGrounded();
    }
    public bool getGrounded()
    {
        return isGrounded;
    }

    private void OnCollisionEnter(Collision other)
    {
        int nCollisions = other.GetContacts(contacts);
        for (int i = 0; i < nCollisions; i++)
        {
            ContactPoint cp = contacts[i];
            /* Ground check */
            if (Vector3.Angle(cp.normal, -Physics.gravity) < 90f)
            {
                isGrounded = true;
                groundNormal = cp.normal;
            }
            /* Stop sticking to walls */
            else
            {
                wallNormal += cp.normal;
            }
        }
    }
    private void OnCollisionStay(Collision other)
    {
        int nCollisions = other.GetContacts(contacts);
        for (int i = 0; i < nCollisions; i++)
        {
            ContactPoint cp = contacts[i];
            /* Ground check */
            if (Vector3.Angle(cp.normal, -Physics.gravity) < 90f)
            {
                isGrounded = true;
                groundNormal = cp.normal;
            }
            /* Stop sticking to walls */
            else
            {
                wallNormal += cp.normal;
            }
        }
    }
    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
        groundNormal = -Physics.gravity;
    }
}
