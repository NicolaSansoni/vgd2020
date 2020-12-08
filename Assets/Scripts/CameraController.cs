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
    private Transform rig2;

    // Start is called before the first frame update
    void Start()
    {
        posBias = transform.position;
        
        cam = GetComponentInChildren<Camera>();
        rig2 = transform.GetChild(0);
    }

    void Update()
    {
        // The camera doesn't need to be perfectly deterministic but needs to be
        // as smooth as possible so it is handled in Update, not in FixedUpdate

        // position
        transform.position = target.position + posBias;

        /* rotation */
        Vector2 rotInput = Vector2.Scale(input.getCameraMovement(), angularVelocity) * Time.deltaTime;
        /* x */
        transform.RotateAround(transform.position, transform.up, rotInput.x);
        /* y */
        float pitch = rig2.localEulerAngles.x;
        // 180° == -180°, 
        if (pitch > 180f) pitch -= 360f;
        // don't go over the vertical plane
        pitch = Mathf.Clamp(pitch + rotInput.y, -90f, 90f);
        rig2.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}
