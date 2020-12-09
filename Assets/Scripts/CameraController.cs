using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public InputHandler input;
    public Transform target;

    [Space]
    [Tooltip("In degrees per second")]
    public Vector2 angularVelocity = new Vector2(60f, 60f);
    private Camera cam;
    private Vector3 posBias;
    private Transform rigArm;

    // Start is called before the first frame update
    void Start()
    {
        posBias = transform.position;
        
        cam = GetComponentInChildren<Camera>();
        rigArm = transform.GetChild(0);
    }

    void Update()
    {
        // The camera doesn't need to be perfectly deterministic but needs to be
        // as smooth as possible so it is handled in Update, not in FixedUpdate

        // position
        transform.position = target.position + posBias;

        /* rotation */
        Vector2 rotInput = Vector2.Scale(input.getCameraMovement(), angularVelocity) * Time.deltaTime;
        /* yaw */
        Vector3 yAxis = -Physics.gravity.normalized;
        transform.RotateAround(transform.position, yAxis, rotInput.x);
        /* pitch */
        // TODO: FIX VERTICAL PITCH ISSUES (±90°)
        Vector3 xAxis = Vector3.Cross(rigArm.forward, yAxis);
        Vector3 projection = Vector3.ProjectOnPlane(rigArm.rotation * yAxis, xAxis);
        float pitch = Mathf.Acos(Vector3.Dot(projection, yAxis)) * Mathf.Rad2Deg;
        // 180° == -180°, 
        if (pitch > 180f) pitch -= 360f;
        if (pitch < -180f) pitch += 360f;
        // don't go over the vertical plane unless you are already over
        float constraint = 90f;
        if (pitch < -constraint || pitch > constraint) {
            constraint = 360f;
        }
        pitch = Mathf.Clamp(pitch + rotInput.y, -constraint, constraint) - pitch;
        rigArm.RotateAround(rigArm.position, xAxis, pitch);
    }
}
