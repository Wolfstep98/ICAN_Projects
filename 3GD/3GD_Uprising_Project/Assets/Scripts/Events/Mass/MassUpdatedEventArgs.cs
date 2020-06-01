using System;

namespace GameEvents
{
    public interface IMassUpdated
    {
        event EventHandler<MassUpdatedEventArgs> OnMassUpdated;
    }

    [Serializable]
    public class MassUpdatedEventArgs : EventArgs
    {
        #region Fields & Properties
        private float previousMass = 0.0f;
        public float PreviousMass { get { return this.previousMass; } }

        private float currentMass = 0.0f;
        public float CurrentMass { get { return this.currentMass; } }
        #endregion

        #region Constructor
        public MassUpdatedEventArgs(float previousMass, float currentMass)
        {
            this.previousMass = previousMass;
            this.currentMass = currentMass;
        }
        #endregion
    }
}
