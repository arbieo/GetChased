using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

	public LineRenderer line;
	public Collider2D laserCollider;

	public int damage = 10;

	void OnTriggerEnter2D(Collider2D c)
	{
		OnHitTriggered(c);
	}

	void OnTriggerStay2D(Collider2D c)
	{
		OnHitTriggered(c);
	}
	
	void OnHitTriggered(Collider2D c)
	{
		Entity entity = c.attachedRigidbody.gameObject.GetComponent<Entity>();
		if (entity != null)
		{
			if (entity.HasEffect(Effect.Type.ETHERAL))
			{
				return;
			}

			if(!entity.HasEffect(Effect.Type.INVULN))
			{
				entity.OnDamage(damage);
			}
		}
	}
}
