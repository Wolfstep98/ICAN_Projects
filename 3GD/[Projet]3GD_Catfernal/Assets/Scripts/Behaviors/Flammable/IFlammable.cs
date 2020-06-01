using System;

namespace Game.Behaviors
{

    public interface IFlammable
    {
        bool IsBurning { get; }

        void Burn(int amount);
    }
}