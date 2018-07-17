using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// A configurable reflector display.
/// Note: Wiring data and letter substitution handled
/// within Enigma script.
/// </summary>
public class Reflector : MonoBehaviour
{
    // wiring model support fields
    ReflectorModel model = ReflectorModel.A;    // current wiring of reflector
    Text modelDisplayText;                      // text displaying current wiring of reflector

    // event support
    ReflectorSetEvent reflectorSetEvent;

    /// <summary>
    /// Provides read / write access to reflector's current model
    /// </summary>
    public ReflectorModel Model
    {
        get { return model; }
        set { model = value; }
    }

    /// <summary>
    /// Called before Start() method
    /// </summary>
    void Awake()
    {
        // retrieve text component of model display
        modelDisplayText = GetComponentInChildren<Text>();
        UpdateWiringText(model);
    }

    void Start()
    {
        // adds self as invoker of "Reflector Set" event
        reflectorSetEvent = new ReflectorSetEvent();
        EventManager.AddReflectorSetInvoker(this);
    }

    /// <summary>
    /// Updates wiring model display text
    /// </summary>
    /// <param name="newWiring">new wiring model to display</param>
    void UpdateWiringText(ReflectorModel newWiring)
    {
        // alters model display text according to new wiring model
        switch (newWiring)
        {
            case ReflectorModel.A:
                modelDisplayText.text = "A";
                break;
            case ReflectorModel.B:
                modelDisplayText.text = "B";
                break;
            case ReflectorModel.C:
                modelDisplayText.text = "C";
                break;
            default:
                modelDisplayText.text = "err";
                break;
        }
    }

    /// <summary>
    /// Shifts wiring model of reflector up or down by one
    /// Called when user presses "Next Model" or "Prev Model" buttons
    /// </summary>
    /// <param name="shiftToNextModel">whether model should increment to next model</param>
    public void ChangeReflectorWiring(bool shiftToNextModel)
    {
        // if user shifts to next model
        if (shiftToNextModel)
        {
            // if current model isn't last in enum
            if (model != ReflectorModel.C)
            {
                // increment as normal
                model += 1;
            }
            // otherwise, wrap model to first in enum
            else
            {
                model = ReflectorModel.A;
            }
        }
        // otherwise (i.e., user shifts to prev model)
        else
        {
            // if current model isn't first in enum
            if (model != ReflectorModel.A)
            {
                // decrement as normal
                model -= 1;
            }
            // otherwise, wrap model to last in enum
            else
            {
                model = ReflectorModel.C;
            }
        }

        // update displayed model
        UpdateWiringText(model);
        reflectorSetEvent.Invoke(model);
    }

    /// <summary>
    /// Adds given listener for "Reflector Set" event
    /// </summary>
    /// <param name="newListener">new listener for event</param>
    public void AddReflectorSetListener (UnityAction<ReflectorModel> newListener)
    {
        reflectorSetEvent.AddListener(newListener);
    }

}
