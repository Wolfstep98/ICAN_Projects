namespace Game.Behaviors
{
    /// <summary>
    /// Interface for behaviors that can be activable.
    /// </summary>
    public interface IActivable
    {
        #region Properties
        /// <summary>
        /// Is the behavior activated.
        /// </summary>
        bool IsActivated { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Activate the behavior.
        /// </summary>
        void Activate();

        /// <summary>
        /// Desactivate the behavior.
        /// </summary>
        void Desactivate();
        #endregion
    }
}
