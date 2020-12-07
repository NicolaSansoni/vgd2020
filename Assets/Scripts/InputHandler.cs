using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Vector2 mov;
    private bool jump;

    // Start is called before the first frame update
    void Start()
    {
        mov = new Vector2();
        jump = false;
    }

    // Update is called once per frame
    void Update()
    {
        mov.x = Input.GetAxis("Horizontal");
        mov.y = Input.GetAxis("Vertical");
        mov = Vector2.ClampMagnitude(mov, 1f);

        jump = Input.GetButtonDown("Jump");
    }

    public bool getJump() {
        return jump;
    }

    public Vector2 getMovement() {
        return mov;
    }
}
