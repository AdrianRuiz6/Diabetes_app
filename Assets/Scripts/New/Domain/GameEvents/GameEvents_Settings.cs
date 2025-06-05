using System;

namespace Master.Domain.GameEvents
{
    public class GameEvents_Settings
    {
        public static Action<int> OnInitialTimeInitialized;
        public static Action<int> OnInitialTimeModified;

        public static Action<int> OnFinishTimeInitialized;
        public static Action<int> OnFinishTimeModified;

        public static Action<float> OnSoundEffectsInitialized;
        public static Action<float> OnSoundEffectsModified;
    }
}