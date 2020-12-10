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
    public float maxSlope = 50f;

    private Rigidbody rb;
    private Collider feet;
    private ContactPoint[] contacts;
    private Vector3 movInput;
    private bool jmpInput;
    private Vector3 planeVel;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        contacts = new ContactPoint[10];
        for (int i = 0; i < contacts.Length; i++) {
            contacts[i] = new ContactPoint();
        }
        movInput = new Vector3();
        jmpInput = false;
        planeVel = Vector3.zero;
        feet = transform.Find("Base").gameObject.GetComponent<Collider>();
        isGrounded = false;
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
        Quaternion camUpRot = Quaternion.FromToRotation(pointOfView.up, Vector3.up);
        Vector3 camFwd = camUpRot * pointOfView.forward;
        Quaternion fwdRot = Quaternion.FromToRotation(Vector3.forward, camFwd);
        Quaternion upRot = Quaternion.FromToRotation(Vector3.up, -Physics.gravity);
        velTarget = upRot * fwdRot * velTarget;
        
        Vector3 velDiff = velTarget - planeVel;
        // clamp instead of normalize to handle standing still correctly
        Vector3 accelVect = Vector3.ClampMagnitude(velDiff, 1f) * acceleration;
        rb.AddForce(accelVect, ForceMode.Acceleration);

        if (jmpInput) {
            if (getGrounded()){
                Jump();
            }
            jmpInput = false;
        }
    }

    private void Jump() {
        float jumpSpeed = Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude);
        rb.velocity = planeVel + -Physics.gravity.normalized * jumpSpeed;
    }

    public float getSpeed() {
        return planeVel.magnitude;
    }
    public bool getJump() {
        return jmpInput && getGrounded();
    }
    public bool getGrounded() {
        return isGrounded;
    }

    private void OnCollisionEnter(Collision other) {
        int nCollisions = other.GetContacts(contacts);
        for (int i = 0; i < nCollisions; i++) {
            if (contacts[i].thisCollider.Equals(feet) &&
                !contacts[i].otherCollider.CompareTag("Character") &&
                Vector3.Angle(contacts[i].normal, -Physics.gravity) < maxSlope) {

                Debug.Log(contacts[i].otherCollider);
                isGrounded = true;
            }
        }
    }
    private void OnCollisionStay(Collision other) {
        int nCollisions = other.GetContacts(contacts);
        for (int i = 0; i < nCollisions; i++) {
            if (contacts[i].thisCollider.Equals(feet) &&
                !(contacts[i].otherCollider.CompareTag("Character")) &&
                Vector3.Angle(contacts[i].normal, -Physics.gravity) < maxSlope) {

                Debug.Log(contacts[i].otherCollider);
                isGrounded = true;
            }
        }
    }
    private void OnCollisionExit(Collision other) {
        isGrounded = false;
    }
}
