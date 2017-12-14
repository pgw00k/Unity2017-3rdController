using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBehaviour : GenericBehaviour
{

    private bool magic = false;

    private float speed, speedSeeker;               // Moving speed.

    public float walkSpeed = 0.15f;                 // Default walk speed.
    public float runSpeed = 1.0f;                   // Default run speed.
    public float sprintSpeed = 2.0f;                // Default sprint speed.
    public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.

    // Use this for initialization
    void Start () {
        // Subscribe this behaviour on the manager.
        behaviourManager.SubscribeBehaviour(this);
    }
	
	// Update is called once per frame
	void Update () {
        // Toggle fly by input.
        if (Input.GetButtonDown("Magic"))
        {
            magic = !magic;

            // Player is flying.
            if (magic)
            {
                // Register this behaviour.
                behaviourManager.RegisterBehaviour(this.behaviourCode);
            }
            else
            {
                // Set camera default offset.
                behaviourManager.GetCamScript.ResetTargetOffsets();

                // Unregister this behaviour and set current behaviour to the default one.
                behaviourManager.UnregisterBehaviour(this.behaviourCode);
            }
        }

        // Assert this is the active behaviour
        magic = magic && behaviourManager.IsCurrentBehaviour(this.behaviourCode);

        // Set fly related variables on the Animator Controller.
        behaviourManager.GetAnim.SetBool("Magic", magic);
    }

    // LocalFixedUpdate overrides the virtual function of the base class.
    public override void LocalFixedUpdate()
    {
        // Call the basic movement manager.
        MovementManagement(behaviourManager.GetH, behaviourManager.GetV);

    }

    void MovementManagement(float horizontal, float vertical)
    {
        // On ground, obey gravity.
        if (behaviourManager.IsGrounded())
            behaviourManager.GetRigidBody.useGravity = true;

        // Call function that deals with player orientation.
        //Rotating(horizontal, vertical);

        // Set proper speed.
        Vector2 dir = new Vector2(horizontal, vertical);
        speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
        // This is for PC only, gamepads control speed via analog stick.
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
        speed *= speedSeeker;
        if (behaviourManager.isSprinting())
        {
            speed = sprintSpeed;
        }

        behaviourManager.GetAnim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
    }
}
