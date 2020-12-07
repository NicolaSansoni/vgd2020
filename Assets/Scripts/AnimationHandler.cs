using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private MovementHandler mh;
    private Animator anim;

    private float speed;
    private bool grounded;
    private bool jump;

    // Start is called before the first frame update
    void Start()
    {
        mh = GetComponentInParent<MovementHandler>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = mh.getSpeed();
        jump = mh.getJump();
        grounded = mh.getGrounded();

        anim.SetFloat("Speed", speed);
        anim.SetBool("Jump", jump);
        anim.SetBool("Grounded", grounded);
    }
}
