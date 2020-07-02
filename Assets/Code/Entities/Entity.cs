using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

	public enum Team
	{
		PLAYER,
		ENEMY,
		NEUTRAL
	}

	public Team team;
	
	public int score = 0;

	public Color entityColor = Color.red;

	public GameObject display;

	[HideInInspector]
	public Vector2 moveVector = new Vector2(0,1);
	[HideInInspector]
	public Vector2 targetVector = new Vector2(0,1);

	public int health = 1;
	public int collideDamage = 1;

	public bool collideImmunity = false;
	public bool damageImmunity = false;

	public List<Effect> effects = new List<Effect>();

	public Dictionary<Effect.Type, List<Effect>> effectsByType = new Dictionary<Effect.Type, List<Effect>>();

	public static List<Entity> entities = new List<Entity>();

	public virtual void Awake () {
		Initialize();
	}

	public virtual Vector2 PredictPosition(float time)
	{
		return transform.position;
	}

	public void AddEffect(Effect effect)
	{
		effect.OnAddEffect(this);
		effects.Add(effect);
		if (!effectsByType.ContainsKey(effect.type))
		{
			effectsByType.Add(effect.type, new List<Effect>());
		}
		effectsByType[effect.type].Add(effect);
	}

	public void RemoveEffect(Effect effect)
	{
		effect.OnRemoveEffect(this);
		effects.Remove(effect);
		effectsByType[effect.type].Remove(effect);
	}

	public bool HasEffect(Effect.Type effectType)
	{
		return effectsByType.ContainsKey(effectType) && effectsByType[effectType].Count > 0;
	}

	public virtual void FixedUpdate()
	{
		List<Effect> effectsToRemove = new List<Effect>();
		foreach (Effect effect in effects)
		{
			effect.OnEffectTick(this);
			if (Time.time - effect.startTime > effect.duration)
			{
				effectsToRemove.Add(effect);
			}
		}
		foreach (Effect effect in effectsToRemove)
		{
			RemoveEffect(effect);
		}
	}

	public void BecomeInvincible(float time)
	{
		Effect invuln = new Effect(Effect.Type.INVULN, time);
		AddEffect(invuln);
	}

	public void BecomeEtheral(float time)
	{
		Effect etheral = new Effect(Effect.Type.ETHERAL, time);
		AddEffect(etheral);
	}

	public void AvoidCollisions(Entity otherEntity, float duration)
	{
		Effect avoidEffect = new ExcludeCollisionsEffect(otherEntity, duration);
		AddEffect(avoidEffect);
	}

	public void DelayedKill(float duration)
	{
		DeathEffect deathEffect = new DeathEffect(duration);
		AddEffect(deathEffect);
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
		if (collideImmunity)
		{
			return;
		}
		health -= entity.collideDamage;
		if (health <= 0)
		{
			Kill();
		}
	}

	public virtual void OnDamage(int damage)
	{
		if (damageImmunity)
		{
			return;
		}
		health -= damage;
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

	public virtual void Kill(bool giveScore = true)
	{
		GameObject.Destroy(gameObject);

		GameObject deathPrefab = Resources.Load<GameObject>("Prefabs/Explosion");
		if (team == Team.PLAYER)
		{
			deathPrefab = Resources.Load<GameObject>("Prefabs/PlayerExplosion");
		}
		GameObject explosion = GameObject.Instantiate(deathPrefab, transform.position, Quaternion.identity);
		var main = explosion.GetComponent<ParticleSystem>().main;
		explosion.GetComponent<HomingParticles>().target = GameController.instance.player.gameObject;
		main.startColor = entityColor;
		GameObject.Destroy(explosion,5);

		if (score > 0 && giveScore)
		{
			GameObject scorePrefab = (GameObject)Resources.Load("Prefabs/ScorePrefab");
			GameObject scoreObject = GameObject.Instantiate(scorePrefab, transform.position, Quaternion.identity);
			scoreObject.GetComponent<ScoreController>().text.text = score.ToString();
			GameController.instance.AddScore(score);
		}
	}

	void OnHitTriggered(Collider2D c)
	{
		if (c.attachedRigidbody == null)
		{
			return;
		}
		Entity entity = c.attachedRigidbody.gameObject.GetComponent<Entity>();
		if (entity != null)
		{
			if (HasEffect(Effect.Type.ETHERAL) || entity.HasEffect(Effect.Type.ETHERAL))
			{
				return;
			}

			if (HasEffect(Effect.Type.EXCLUDE_COLLISONS))
			{
				foreach (Effect effect in effectsByType[Effect.Type.EXCLUDE_COLLISONS])
				{
					if (((ExcludeCollisionsEffect)effect).entity == entity)
					{
						return;
					}
				}
			}
			if (entity.HasEffect(Effect.Type.EXCLUDE_COLLISONS))
			{
				foreach (Effect effect in entity.effectsByType[Effect.Type.EXCLUDE_COLLISONS])
				{
					if (((ExcludeCollisionsEffect)effect).entity == this)
					{
						return;
					}
				}
			}

			if(!entity.HasEffect(Effect.Type.INVULN))
			{
				entity.OnImpact(this);
			}
		}
	}
}
