using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

	public GameObject display;
	public GameObject deathEffectPrefab;

	[HideInInspector]
	public Vector2 moveVector = new Vector2(0,1);
	[HideInInspector]
	public Vector2 targetVector = new Vector2(0,1);

	protected bool isInvincible = false;
	protected float invincibilityEndTime;
	protected bool isEtheral = false;
	protected float etheralEndTime;

	public float health = 100;
	public float collideDamage = 100;

	public List<Effect> effects = new List<Effects>();

	public static List<Entity> entities = new List<Entity>();

	void Start () {
		Initialize();
	}

	public virtual Vector2 PredictPosition(float time)
	{
		return transform.position;
	}

	public virtual void FixedUpdate()
	{
		if(isInvincible)
		{
			if (Time.time > invincibilityEndTime)
			{
				isInvincible = false;
			}
		}
		if(isEtheral)
		{
			if (Time.time > etheralEndTime)
			{
				isEtheral = false;
			}
		}
	}

	public void BecomeInvincible(float time)
	{
		isInvincible = true;
		invincibilityEndTime = Mathf.Max(Time.time + time, invincibilityEndTime);
	}

	public void BecomeEtheral(float time)
	{
		isEtheral = true;
		etheralEndTime = Mathf.Max(Time.time + time, etheralEndTime);
	}

	protected void Initialize()
	{
		entities.Add(this);
	}

	public void OnDestroy()
	{
		entities.Remove(this);
	}

	protected virtual void OnImpact(Entity entity)
	{
		health -= entity.collideDamage;
		if (health <= 0)
		{
			Kill();
		}
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		OnHitTriggered(c);
	}

	void OnTriggerStay2D(Collider2D c)
	{
		OnHitTriggered(c);
	}

	public virtual void Kill()
	{
		GameObject.Destroy(gameObject);
		if (deathEffectPrefab != null)
		{
			GameObject explosion = GameObject.Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
			var main = explosion.GetComponent<ParticleSystem>().main;
			main.startColor = display.GetComponent<SpriteRenderer>().color;
			GameObject.Destroy(explosion,1);
		}
	}

	void OnHitTriggered(Collider2D c)
	{
		Entity entity = c.attachedRigidbody.gameObject.GetComponent<Entity>();
		if (entity != null)
		{
			if (isEtheral || entity.isEtheral)
			{
				return;
			}

			if (!isInvincible)
			{
				OnImpact(entity);
			}
			if(!entity.isInvincible)
			{
				entity.OnImpact(entity);
			}
		}
	}
}
