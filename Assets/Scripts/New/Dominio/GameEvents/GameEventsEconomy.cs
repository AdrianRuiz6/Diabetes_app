using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Master.Domain.Events
{
    public static class GameEventsEconomy
    {
        public static Action<int> OnCoinsUpdated;
        public static Action<Color> OnProductEquiped;
        public static Action<bool> OnProductBought;
    }
}

