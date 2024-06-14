using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventsScore
{
    public static Action<int> OnModifyCurrentScore;
    public static Action<int> OnModifyHigherScore;
}
