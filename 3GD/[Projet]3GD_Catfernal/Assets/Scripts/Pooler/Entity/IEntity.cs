
namespace DesignPattern.ObjectPooling
{
    /// <summary>
    /// Basic interface for an entity.
    /// </summary>
    public interface IEntity
    {
        //Properties
        /// <summary>
        /// Is the entity enable ?
        /// </summary>
        bool IsEnable { get; }

        //Mehtods
        /// <summary>
        /// Initialize the entity.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Enable the entity.
        /// </summary>
        void Enable();
        /// <summary>
        /// Disable the entity.
        /// </summary>
        void Disable();
    }
}
