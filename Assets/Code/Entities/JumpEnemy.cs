using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEnemy : Entity {

	[HideInInspector]
	public Entity target { get; set; }

	public float timeToJump = 1;
    public float jumpDistance = 100;
    public float timeBetweenJumps = 3;

    float lastJumpTime;
    bool jumping = false;


	public override void FixedUpdate () 
	{
		base.FixedUpdate();

        if (jumping)
        {
            transform.position = (Vector2)transform.position + targetVector * jumpDistance/timeToJump * Time.fixedDeltaTime;
            if (Time.time - lastJumpTime > timeToJump)
            {
                jumping = false;
            }
        }
        if (Time.time - lastJumpTime > timeBetweenJumps)
        {
            jumping = true;
            lastJumpTime = Time.time;
        }

        if (!jumping)
        {
            targetVector = (target.transform.position - transform.position).normalized;
        }
	}
}
