
namespace Game.Enums
{
    /// <summary>
    /// The source from the damage taken.
    /// </summary>
    public enum DamageSource
    {
        // Entities
        Firefly,
        Orb,
        GreatOrb,

        // Player
        PlayerCollision,

        // World
        WallCollision,

        // Enemies
        BasicEnemy,
        HeavyEnemy,

        // Fire
        Fire,

        // Explosion
        Explosion,

        Undefined
    }
}
