using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBehaviour : GenericBehaviour
{
    public float fCoolTime;
    public float fDashSpeed;
    public float fDashTime;

    bool isDashing;

	// Use this for initialization
	void Start () {
        behaviourManager.SubscribeBehaviour(this);
        isDashing = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButton("Dash"))
        {
            isDashing = true;
        }

        if (isDashing)
        {
            transform.TransformDirection(Vector3.forward);
        }
	}
}
