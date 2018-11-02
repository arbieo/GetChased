public class DeathEffect : Effect
{
    public DeathEffect(float duration) : base(Type.DEATH, duration){}

    public override void OnRemoveEffect(Entity entity)
    {
        entity.Kill();
    }
}