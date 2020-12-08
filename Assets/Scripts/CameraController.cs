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
        cam = GetComponentInChildren<Camera>();
        posBias = transform.position;
    }

    void Update()
    {
        // The camera doesn't need to be perfectly deterministic but needs to be
        // as smooth as possible so it is handled in Update, not in FixedUpdate

        // position
        transform.position = target.position + posBias;

        // rotation
        Vector2 rotation = Vector2.Scale(input.getCameraMovement(), angularVelocity) * Time.deltaTime;
        transform.Rotate(0f, rotation.x, 0f);
    }
}
