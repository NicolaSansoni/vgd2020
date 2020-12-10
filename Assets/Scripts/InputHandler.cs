using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float mouseSens = 1f;
    public float joystickCameraSens = 1f;

    [Tooltip("seconds it takes to change from input 0 to input max")]
    public float joystickCameraDelay = 0f;
    private Vector2 mov;
    private Vector2 mCamMov;
    private Vector2 jCamMov;
    private bool jump;

    // Start is called before the first frame update
    void Start()
    {
        mov = Vector2.zero;
        mCamMov = Vector2.zero;
        jCamMov = Vector2.zero;
        jump = false;
    }

    // Update is called once per frame
    void Update()
    {
        /* character movement */
        mov.x = Input.GetAxis("Horizontal");
        mov.y = Input.GetAxis("Vertical");
        mov = Vector2.ClampMagnitude(mov, 1f);

        /* camera movement */
        // mouse
        mCamMov.x = Input.GetAxis("Mouse X") * mouseSens;
        mCamMov.y = Input.GetAxis("Mouse Y") * mouseSens;
        // joystick
        // get the difference between desired speed and the current one
        Vector2 jcMovTarget = new Vector2(
            Input.GetAxis("Joystick X"),
            Input.GetAxis("Joystick Y")
        );
        Vector2 jcMovDiff = jcMovTarget - jCamMov;
        // calculate the costraints for the velocity in this frame
        Vector2 jcClamp = new Vector2(1f,1f);
        if (joystickCameraDelay > 0) {
            jcClamp *= Time.deltaTime / joystickCameraDelay;
        }
        // if we are changing direction or stopping widen the constraints for better feel
        if (jcMovTarget.x * jCamMov.x <= 0) {
            jcClamp.x *= 2;
        }
        if (jcMovTarget.y * jCamMov.y <= 0) {
            jcClamp.y *= 2;
        }
        // clamp and apply sens
        jCamMov.x += Mathf.Clamp(jcMovDiff.x, -jcClamp.x, jcClamp.x);
        jCamMov.y += Mathf.Clamp(jcMovDiff.y, -jcClamp.y, jcClamp.y);
        /* jump */
        jump = Input.GetButtonDown("Jump");

    }

    public bool getJump() {
        return jump;
    }

    public Vector2 getMovement() {
        return mov;
    }

    public Vector2 getCameraMovement() {
        return mCamMov * mouseSens + jCamMov * joystickCameraSens;
    }
}
