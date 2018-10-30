using UnityEngine;

public class ShootingEffect : Effect
{
    public GameObject bulletPrefab;

    public float lastShotTime;
    public float shotRate;

    public ShootingEffect(GameObject bulletPrefab, float shotRate, float duration) : base(Type.SHOOTING, duration)
    {
        this.bulletPrefab = bulletPrefab;
        this.shotRate = shotRate;
        lastShotTime = Time.time - shotRate;
    }

    public override void OnEffectTick(Entity entity)
	{
        if (Time.time - lastShotTime > shotRate)
        {
            lastShotTime += shotRate;
		    GameObject bulletObject = GameObject.Instantiate(bulletPrefab, entity.transform.position, Quaternion.identity);
            PropelledEntity bulletEntity = bulletObject.GetComponent<PropelledEntity>();
            bulletEntity.AvoidCollisions(entity, 1);
            bulletEntity.moveVector = entity.moveVector;
            bulletEntity.speed = bulletEntity.maxSpeed;
            bulletEntity.DelayedKill(5);
        }
	}
}