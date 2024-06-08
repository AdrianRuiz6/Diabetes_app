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
        SimulateTimers();
        SimulateAttributes();
    }

    private void SimulateAttributes()
    {
        TimeSpan timePassed = DateTime.Now - _lastShutDownTime;
        int intervalsPassed = (int)(timePassed.TotalSeconds / 300);

        while (intervalsPassed != 0)
        {
            GameEventsPetCare.OnExecutingAttributes?.Invoke();
            while (_countAttributes != 3)
            {
                continue;
            }
            if (_iterationAmountInsulin > 0)
                _iterationAmountInsulin--;
            if (_iterationAmountExercise > 0)
                _iterationAmountExercise--;
            if (_iterationAmountFood > 0)
                _iterationAmountFood--;
            _countAttributes = 0;
        }
    }

    private void UpdateInterval()
    {
        _countAttributes++;
    }

    private void SimulateTimers()
    {
        TimeSpan timePassed = DateTime.Now - _lastShutDownTime;

        SimulateTimeAttribute(timePassed);

        // Update cooldown and effects for each button
        foreach (KeyValuePair<string, DateTime?> button in buttonsCD)
        {
            SimulataTimeButtons(button, timePassed);
        }
    }

    private void SimulateTimeAttribute(TimeSpan timePassed)
    {
        // Calculate the time passed since the last shutdown
        float currentTime = 300 - (float)(timePassed.TotalSeconds % 300);

        // Update the attribute timers
        AttributeSchedule.Instance.UpdateTimer(currentTime);
    }

    private void SimulataTimeButtons(KeyValuePair<string, DateTime?> button, TimeSpan timePassed)
    {
        if (button.Value == null)
            return;

        // Time passed since last time user pressed the button.
        TimeSpan timeButtonPassed = DateTime.Now - button.Value.Value;
        float secondsPassed = (float)timeButtonPassed.TotalSeconds;

        // Update the cooldown timers
        if (secondsPassed < 3600)
        {
            GameEventsPetCare.OnActivateCoolDown?.Invoke(button.Key, 3600 - secondsPassed);
        }

        // Update the long-term effects timers for the attribute buttons and their states on previous intervals.
        if (secondsPassed < 1800)
        {
            int iterations = (int)(timeButtonPassed.TotalSeconds / 300);
            float remainingTime = 1800 - secondsPassed;

            switch (button.Key)
            {
                case "ButtonInsulin":
                    _iterationAmountInsulin = iterations;
                    if (_iterationAmountInsulin > 0)
                    {
                        AttributeManager.Instance.ActivateInsulinButton(simulated: true, time: remainingTime);
                    }
                    break;

                case "ButtonExercise":
                    _iterationAmountExercise = iterations;
                    if (_iterationAmountExercise > 0)
                    {
                        AttributeManager.Instance.ActivateExerciseButton(simulated: true, time: remainingTime);
                    }
                    break;

                case "ButtonFood":
                    _iterationAmountFood = iterations;
                    if (_iterationAmountFood > 0)
                    {
                        AttributeManager.Instance.ActivateFoodButton(simulated: true, time: remainingTime);
                    }
                    break;
            }
        }
    }
}


// Coges el último