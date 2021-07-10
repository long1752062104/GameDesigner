using Net.Component;
using System;

namespace Example2
{
    public class GameEvent : SingleCase<GameEvent>
    {
        public static Action OnPlayerDead;
    }
}