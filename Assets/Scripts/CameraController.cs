﻿using System.Collections;
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
    private GravityHandler gravity;
    private Vector3 posBias;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        gravity = target.GetComponent<GravityHandler>();

        posBias = transform.position;
    }

    private void FixedUpdate()
    {
        // The camera doesn't need to be perfectly deterministic but needs to be
        // as smooth as possible so it is handled in Update, not in FixedUpdate

        // position
        transform.position = target.position + Quaternion.FromToRotation(Vector3.down, gravity.get()) * posBias;

        /* rotation */
        Vector2 rotInput = Vector2.Scale(input.getCameraMovement(), angularVelocity) * Time.fixedDeltaTime;
        rotInput.y *= -1f;
        /* axes */
        Vector3 yAxis = -gravity.get().normalized;
        Vector3 xAxis = Vector3.Cross(yAxis, transform.forward);
        if (Vector3.Dot(transform.up, yAxis) < 0f) {
            xAxis *= -1;
        }
        xAxis.Normalize();

        /* pitch */
        float pitch = Vector3.SignedAngle(yAxis, transform.forward, xAxis);
        if (Mathf.Approximately(pitch % 180f, 0)) {
            xAxis = transform.right;
        }
        
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
