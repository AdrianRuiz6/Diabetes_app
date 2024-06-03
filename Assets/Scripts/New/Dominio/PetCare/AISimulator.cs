using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class AISimulator : MonoBehaviour
{
    private DateTime _lastShutDownTime; //TODO: guardar en playerPrefs
    private int _countAttributes;

    private Dictionary<string, DateTime?> buttonsCD;
    private DateTime? _buttonInsulinTimeCD, _buttonExerciseTimeCD, _buttonFoodTimeCD;
    [SerializeField] private string _IDInsulinCD = "ButtonInsulin";
    [SerializeField] private string _IDExerciseCD = "ButtonExercise";
    [SerializeField] private string _IDFoodCD = "ButtonFood";

    private int _iterationAmountInsulin;
    private int _iterationAmountExercise;
    private int _iterationAmountFood;

    void Awake() 
    {
        GameEventsPetCare.OnExecutedAttribute += UpdateInterval;

        // TODO: cargar los CD de los botones en el DataStorage.
        buttonsCD = new Dictionary<string, DateTime?>
        {
            { _IDInsulinCD, _buttonInsulinTimeCD },
            { _IDExerciseCD, _buttonExerciseTimeCD },
            { _IDFoodCD, _buttonFoodTimeCD }
        };

        foreach(KeyValuePair<string, DateTime?> button in buttonsCD)
        {
            string savedValue;

            savedValue = PlayerPrefs.GetString(button.Key, "null");
            if (savedValue == "null")
                buttonsCD[button.Key] = null;
            else
                buttonsCD[button.Key] = DateTime.Parse(savedValue, null, System.Globalization.DateTimeStyles.RoundtripKind);

        }
    }

    private void Start()
    {
        _iterationAmountInsulin = 0;
        _iterationAmountExercise = 0;
        _iterationAmountFood = 0;

        _countAttributes = 0;
        OnApplicationStart();
    }
    void OnDestroy()
    {
        GameEventsPetCare.OnExecutedAttribute += UpdateInterval;
        //TODO: guardar lastShutDownTime en playerPrefs
    }
    private void OnApplicationStart()
    {
        //TODO: cargar lastShutDownTime en playerPrefs
        SimulateTime();
    }

    private void SimulateTime()
    {
        UpdateTimers();

        TimeSpan timePassed = DateTime.Now - _lastShutDownTime;
        int intervalsPassed = (int)(timePassed.TotalSeconds / 300);

        while (intervalsPassed != 0)
        {
            GameEventsPetCare.OnExecutingAttributes?.Invoke();
            while (_countAttributes != 3)
            {
                continue;
            }
            if(_iterationAmountInsulin > 0)
                _iterationAmountInsulin--;
            if(_iterationAmountExercise > 0)
                _iterationAmountExercise--;
            if(_iterationAmountFood > 0)
                _iterationAmountFood--;
            _countAttributes = 0;
        }
    }

    private void UpdateInterval()
    {
        _countAttributes++;
    }

    private void UpdateTimers()
    {
        // Updates timer of the attributes AI.
        TimeSpan timePassed = DateTime.Now - _lastShutDownTime;
        float currentTime = 300 - (float)(timePassed.TotalSeconds % 300);

        AttributeSchedule.Instance.UpdateTimer(currentTime);

        // Updates buttons.
        foreach(KeyValuePair<string, DateTime?> button in buttonsCD)
        {
            TimeSpan? timebuttonPassed = null;
            if (button.Value != null)
            {
                // Updates timer of the buttons CD.
                timebuttonPassed = DateTime.Now - button.Value;
                float secondsPassed = (float)(timePassed.TotalSeconds);
                if (secondsPassed < 3600)
                {
                    GameEventsPetCare.OnActivateCoolDown?.Invoke(button.Key, 3600 - secondsPassed);
                }

                // Updates intervals and timer of the attriute buttons long term effects. TODO
                if (secondsPassed < 1800)
                {
                    if(button.Key == "ButtonInsulin")
                    {
                        _iterationAmountInsulin = (int)(timePassed.TotalSeconds / 300);
                        if (_iterationAmountFood > 0)
                        {
                            AttributeManager.Instance.ActivateExerciseButton(simulated: true, time: 1800 - secondsPassed);
                        }
                    }
                    if(button.Key == "ButtonExercise")
                    {
                        _iterationAmountExercise = (int)(timePassed.TotalSeconds / 300);
                        if (_iterationAmountFood > 0)
                        {
                            AttributeManager.Instance.ActivateExerciseButton(simulated: true, time: 1800 - secondsPassed);
                        }
                    }
                    if(button.Key == "ButtonFood")
                    {
                        _iterationAmountFood = (int)(timePassed.TotalSeconds / 300);
                        if(_iterationAmountFood > 0)
                        {
                            AttributeManager.Instance.ActivateFoodButton(simulated: true, time: 1800 - secondsPassed);
                        }
                    }
                }
            }
        }
    }
}