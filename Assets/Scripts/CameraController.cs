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

    // Start is called before the first frame update
    void Start()
    {
        posBias = transform.position;
        
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // The camera doesn't need to be perfectly deterministic but needs to be
        // as smooth as possible so it is handled in Update, not in FixedUpdate

        // position
        transform.position = target.position + posBias;

        /* rotation */
        // TODO: ALIGN INPUT WITH CAMERA
        Vector2 rotInput = Vector2.Scale(input.getCameraMovement(), angularVelocity) * Time.deltaTime;
        /* yaw */
        Vector3 yAxis = -Physics.gravity.normalized;
        transform.RotateAround(transform.position, yAxis, rotInput.x);
        /* pitch */
        Vector3 xAxis = Vector3.Cross(transform.forward, yAxis);
        if (Vector3.Dot(transform.up, yAxis) < 0f) {
            xAxis *= -1;
        }
        if (xAxis.sqrMagnitude < 0.1f) {
            //TODO: THIS DOESN'T FIX THE MESSED UP ROTATION AS I'D LIKE
            xAxis = Vector3.Cross(yAxis, transform.up);
            if (Vector3.Dot(yAxis, transform.forward) < 0f) {
                xAxis *= -1;
            }
        }
        xAxis.Normalize();
        float pitch = Vector3.SignedAngle(transform.forward, yAxis, xAxis);
        Debug.Log(pitch);
        // don't go over the vertical plane unless you are already over
        if (pitch >= 0f && pitch <= 180f) {
            //TODO: THIS DOESN'T LOCK IF ROTATION IS MESSED UP
            rotInput.y = Mathf.Clamp(rotInput.y, pitch - 180f, pitch);
        }
        transform.RotateAround(transform.position, xAxis, rotInput.y);
    }
}
