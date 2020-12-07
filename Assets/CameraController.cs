using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    private Camera cam;
    private Vector3 bias;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        bias = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + bias;
        transform.LookAt(target);
    }
}
