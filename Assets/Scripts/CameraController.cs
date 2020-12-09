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
        /* axes */
        Vector3 yAxis = -Physics.gravity.normalized;
        Vector3 xAxis = Vector3.Cross(yAxis, transform.forward);
        if (Vector3.Dot(transform.up, yAxis) < 0f) {
            xAxis *= -1;
        }
        xAxis.Normalize();
        
        /* draw axes for debug */
        Debug.DrawRay(transform.position, transform.up, Color.red);
        Debug.DrawRay(transform.position, transform.forward, Color.yellow);
        Debug.DrawRay(transform.position, yAxis, Color.blue);
        Debug.DrawRay(transform.position, xAxis, Color.green);
        Debug.DrawRay(transform.position, transform.right, Color.cyan);

        /* align input */
        float rotInputAngle = Vector3.SignedAngle(xAxis, transform.right, transform.forward) * Mathf.Deg2Rad;
        rotInput = new Vector2(
            rotInput.x * Mathf.Cos(rotInputAngle) - rotInput.y * Mathf.Sin(rotInputAngle),
            rotInput.x * Mathf.Sin(rotInputAngle) + rotInput.y * Mathf.Cos(rotInputAngle) 
        );

        /* pitch */
        float pitch = Vector3.SignedAngle(yAxis, transform.forward, xAxis);
        if (Mathf.Approximately(pitch % 180f, 0)) {
            xAxis = transform.right;
        }
        Debug.Log(pitch);
        // don't go over the vertical plane unless you are already over
        if (pitch >= 0f && pitch <= 180f) {
            rotInput.y = Mathf.Clamp(rotInput.y, 0f - pitch, 180f - pitch);
        } else {
            rotInput.y = Mathf.Clamp(rotInput.y, -180f - pitch, 0 - pitch);
        }
        transform.RotateAround(transform.position, xAxis, rotInput.y);

        /* yaw */
        transform.RotateAround(transform.position, yAxis, rotInput.x);
    }
}
