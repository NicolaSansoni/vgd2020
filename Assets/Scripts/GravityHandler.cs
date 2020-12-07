using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHandler : MonoBehaviour
{
    private Vector3 gravity;
    // Start is called before the first frame update
    void Start()
    {
        gravity = Physics.gravity;
    }

    public Vector3 getGravity() {
        return gravity;
    }
}
