public class ExcludeCollisionsEffect : Effect
{
    public Entity entity;

    public ExcludeCollisionsEffect(Entity entity, float duration) : base(Type.EXCLUDE_COLLISONS, duration)
    {
        this.entity = entity;
    }
}