using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// A configurable enigma rotor display.
/// Note: Wiring and rotational position data
/// and conversion handled within Enigma script.
/// </summary>
public class Rotor : MonoBehaviour
{

    #region Fields

    // positional fields
    int rotationalPosition = 1;     // rotational position of rotor
    Text positionText;              // text displaying the rotational position of rotor
    int enigmaPosition;             // position of rotor within the enigma machine

    // model fields
    RotorModel model = RotorModel.I;    // current wiring model used by rotor
    Text modelText;                     // text displaying rotor's wiring model

    // event support
    RotorSetEvent rotorSetEvent;
    WiringSetEvent wiringSetEvent;

    #endregion

    #region Properties

    /// <summary>
    /// Provides get / set access to rotor's rotational position
    /// </summary>
    public int RotationalPosition
    {
        get { return rotationalPosition; }
        set
        {
            // handles inputs greater than valid range
            while (value > 26)
            {
                value -= 26;
            }

            // handles inputs lesser than valid range
            while (value < 1)
            {
                value += 26;
            }

            // set fields to value and update text accordingly
            rotationalPosition = value;
            UpdatePositionText(value);
        }
    }

    /// <summary>
    /// Provides read / write access to rotor's position within enigma
    /// </summary>
    public int PositionInEnigma
    {
        get { return enigmaPosition; }
        set { enigmaPosition = value; }
    }

    /// <summary>
    /// Provides read / write access to model of rotor
    /// </summary>
    public RotorModel Model
    {
        get { return model; }
        set { model = value; }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Called before the Start() method
    /// </summary>
    void Awake()
    {
        // retrieve text component of position display
        positionText = GetComponentInChildren<Text>();
        UpdatePositionText(rotationalPosition);

        // retrieves text component of model display
        // by searching grandchildren of rotor game object
        foreach (Transform child in transform)
        {
            foreach (Transform grandchild in child.transform)
            {
                // if tag matches model display text
                if (grandchild.gameObject.CompareTag("ModelDisplayText"))
                {
                    // retrieve text component of this child
                    modelText = grandchild.gameObject.GetComponent<Text>();
                }
            }
        }
        UpdateModelText(model);
    }

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start()
    {
        // adds self as invoker of the (manual) "Rotor Set" event
        rotorSetEvent = new RotorSetEvent();
        EventManager.AddRotorSetInvoker(this);

        // adds self as invoker of the "Wiring Set" event
        wiringSetEvent = new WiringSetEvent();
        EventManager.AddWiringSetInvoker(this);

        // adds self as listener of (automatic) "Advance Rotor" event
        EventManager.AddAdvanceRotorListener(AdvancePosition);
    }

    /// <summary>
    /// Updates rotation display text
    /// </summary>
    /// <param name="newPosition">new rotational position of rotor</param>
    void UpdatePositionText(int newPosition)
    {
        // if new position is less than 10
        if (newPosition < 10)
        {
            // update text with '0' before it (for authenticity)
            positionText.text = "0" + newPosition.ToString();
        }
        // otherwise (i.e., position greater or equal to 10)
        else
        {
            // update text normally
            positionText.text = newPosition.ToString();
        }
    }

    /// <summary>
    /// Updates model display text
    /// </summary>
    /// <param name="newModel">new wiring model of rotor</param>
    void UpdateModelText(RotorModel newModel)
    {
        // alters model text according to new model
        switch (newModel)
        {
            case RotorModel.I:
                modelText.text = "I";
                break;
            case RotorModel.II:
                modelText.text = "II";
                break;
            case RotorModel.III:
                modelText.text = "III";
                break;
            case RotorModel.IV:
                modelText.text = "IV";
                break;
            case RotorModel.V:
                modelText.text = "V";
                break;
            default:
                modelText.text = "err";
                break;
        }
    }

    /// <summary>
    /// Automatically advances position of rotor.
    /// Called when enigma encodes letter and determines
    /// to advance position of given rotor.
    /// </summary>
    /// <param name="rotorNumber">rotor enigma determines to advance</param>
    void AdvancePosition(int rotorNumber)
    {
        // if rotor number passed by event matches this
        // rotor's position in enigma
        if (rotorNumber == enigmaPosition)
        {
            // increment position and wrap if necessary
            rotationalPosition++;
            while (rotationalPosition > 26)
            {
                rotationalPosition -= 26;
            }

            // update position displayed by rotor
            UpdatePositionText(rotationalPosition);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Increments rotational position of rotor
    /// Called when user manually presses increment button
    /// </summary>
    public void IncrementPosition()
    {
        // increments position and wraps if necessary
        rotationalPosition++;
        while (rotationalPosition > 26)
        {
            rotationalPosition -= 26;
        }

        // update displayed position and position in Enigma
        UpdatePositionText(rotationalPosition);
        rotorSetEvent.Invoke(enigmaPosition, rotationalPosition);
    }

    /// <summary>
    /// Decrements rotational position of rotor
    /// Called when user manually presses decrement button
    /// </summary>
    public void DecrementPosition()
    {
        // decrements position and wraps if necessary
        rotationalPosition--;
        while (rotationalPosition < 1)
        {
            rotationalPosition += 26;
        }

        // update displayed position and position in Enigma
        UpdatePositionText(rotationalPosition);
        rotorSetEvent.Invoke(enigmaPosition, rotationalPosition);
    }

    /// <summary>
    /// Increments wiring model of rotor
    /// Called when user manually presses next model button
    /// </summary>
    public void IncrementModel()
    {
        // if current model isn't last in enum
        if (model != RotorModel.V)
        {
            // increment model as normal
            model += 1;
        }
        // otherwise,
        else
        {
            // wrap model to be first in enum
            model = RotorModel.I;
        }

        // update displayed model text and model in enigma
        UpdateModelText(model);
        wiringSetEvent.Invoke(enigmaPosition, model);
    }

    /// <summary>
    /// Decrements wiring model of rotor
    /// Called when user manually presses previous model button
    /// </summary>
    public void DecrementModel()
    {
        // if current model isn't first in enum
        if (model != RotorModel.I)
        {
            // decrement model as normal
            model -= 1;
        }
        // otherwise,
        else
        {
            // wrap model to be last in enum
            model = RotorModel.V;
        }

        // update displayed model text and model in enigma
        UpdateModelText(model);
        wiringSetEvent.Invoke(enigmaPosition, model);
    }

    /// <summary>
    /// Adds given listener for the "Rotor Set" event
    /// </summary>
    /// <param name="newListener">new listener for event</param>
    public void AddRotorSetListener(UnityAction<int, int> newListener)
    {
        rotorSetEvent.AddListener(newListener);
    }

    /// <summary>
    /// Adds given listener for the "Wiring Set" event
    /// </summary>
    /// <param name="newListener">new listener for event</param>
    public void AddWiringSetListener(UnityAction<int, RotorModel> newListener)
    {
        wiringSetEvent.AddListener(newListener);
    }

    #endregion

}
