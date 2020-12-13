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
    private GravityHandler gravity;
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
        gravity = GetComponent<GravityHandler>();
        contacts = new ContactPoint[10];
        for (int i = 0; i < contacts.Length; i++)
        {
            contacts[i] = new ContactPoint();
        }
        wallNormal = new Vector3();
        groundNormal = -gravity.get();
        movInput = new Vector3();
        jmpInput = false;
        planeVel = Vector3.zero;
        isGrounded = false;
    }

    void FixedUpdate()
    {
        /* get frame of reference */
        float fwdWeight = Vector3.Dot(pointOfView.forward, gravity.get());
        float rightWeight = Vector3.Dot(pointOfView.right, gravity.get());
        float upWeight = Vector3.Dot(pointOfView.up, gravity.get());

        Vector3 fwdDir;
        if (Mathf.Abs(fwdWeight) < Mathf.Abs(upWeight) || Mathf.Abs(fwdWeight) < Mathf.Abs(rightWeight)) {
            fwdDir = Vector3.ProjectOnPlane(pointOfView.forward, groundNormal);
        } else {
            fwdDir = Vector3.ProjectOnPlane(pointOfView.up, groundNormal);
        }
        Vector3 rightDir;
        if (Mathf.Abs(rightWeight) < Mathf.Abs(upWeight) || Mathf.Abs(rightWeight) < Mathf.Abs(fwdWeight)) {
            rightDir = Vector3.ProjectOnPlane(pointOfView.right, groundNormal);
        } else {
            rightDir = Vector3.ProjectOnPlane(pointOfView.up, groundNormal);
            rightDir *= Mathf.Sign(rightWeight);
        }

        /* get inputs */
        movInput = fwdDir * input.getMovement().y + rightDir * input.getMovement().x;
        jmpInput = input.getJump();

        /* movement */
        planeVel = Vector3.ProjectOnPlane(rb.velocity, -gravity.get());
        Vector3 groundVel = Vector3.ProjectOnPlane(rb.velocity, groundNormal);
        // adjust for slopes
        float verticalComponent = Vector3.Dot(movInput, -gravity.get().normalized);
        movInput -= movInput * verticalComponent;
        // difference between desired actual and velocity
        Vector3 velDiff = movInput * maxSpeed - groundVel;
        // clamp instead of normalize to handle standing still correctly
        Vector3 accelVect = Vector3.ClampMagnitude(velDiff, 1f) * acceleration;
        // Stop sticking to walls
        wallNormal.Normalize();
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
            Quaternion charRot = Quaternion.LookRotation(planeVel, -gravity.get());
            charRot = Quaternion.RotateTowards(transform.rotation, charRot, 360f * rotXSec * Time.fixedDeltaTime);
            rb.MoveRotation(charRot);
        }

        Debug.DrawRay(transform.TransformPoint(Vector3.up), transform.forward, Color.blue);
    }

    private void Jump()
    {
        Vector3 wall = getCloseWallNormal();
        if (wall.sqrMagnitude != 0) {
            Vector3 old = gravity.get();
            gravity.setDirection(-wall);
            Quaternion rot = Quaternion.FromToRotation(old, gravity.get());
            rb.MoveRotation(rot * rb.rotation);
        }
        else {
            float jumpSpeed = Mathf.Sqrt(jumpHeight * 2f * gravity.get().magnitude);
            rb.velocity = planeVel + -gravity.get().normalized * jumpSpeed;
        }
    }

    public float getSpeed()
    {
        return planeVel.magnitude;
    }
    public bool getJump()
    {
        return jmpInput && getGrounded() && getCloseWallNormal().sqrMagnitude != 0f;
    }
    public bool getGrounded()
    {
        return isGrounded;
    }
    private Vector3 getCloseWallNormal() {
        RaycastHit hit;
        if (Physics.Raycast(transform.TransformPoint(Vector3.up), transform.forward, out hit, 1f)) {
            Debug.DrawRay(transform.TransformPoint(Vector3.up), hit.normal, Color.red);
            return hit.normal;
        }
        return Vector3.zero;
    }

    private void OnCollisionEnter(Collision other)
    {
        int nCollisions = other.GetContacts(contacts);
        for (int i = 0; i < nCollisions; i++)
        {
            ContactPoint cp = contacts[i];
            /* Ground check */
            if (Vector3.Angle(cp.normal, -gravity.get()) < 90f)
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
            if (Vector3.Angle(cp.normal, -gravity.get()) < 90f)
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
        groundNormal = -gravity.get();
    }
}
