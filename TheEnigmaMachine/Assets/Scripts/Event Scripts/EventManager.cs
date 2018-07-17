using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A manager handling events fired within the enigma project
/// </summary>
public static class EventManager
{
    #region Fields

    // encoded key event support
    static List<Enigma> keyEncodedInvokers = new List<Enigma>();
    static List<UnityAction<char>> keyEncodedListeners = new List<UnityAction<char>>();

    // darken keys event support
    static List<OutputKey> darkenKeysInvokers = new List<OutputKey>();
    static List<UnityAction> darkenKeysListeners = new List<UnityAction>();

    // manual rotor set event support
    static List<Rotor> rotorSetInvokers = new List<Rotor>();
    static List<UnityAction<int, int>> rotorSetListeners = new List<UnityAction<int, int>>();

    // automatic advance rotor event support
    static List<Enigma> advanceRotorInvokers = new List<Enigma>();
    static List<UnityAction<int>> advanceRotorListeners = new List<UnityAction<int>>();

    // manual rotor wiring set event support
    static List<Rotor> wiringSetInvokers = new List<Rotor>();
    static List<UnityAction<int, RotorModel>> wiringSetListeners = new List<UnityAction<int, RotorModel>>();

    // manual reflector wiring set event support
    static List<Reflector> reflectorSetInvokers = new List<Reflector>();
    static List<UnityAction<ReflectorModel>> reflectorSetListeners = new List<UnityAction<ReflectorModel>>();

    // manual plug insertion event support
    static List<SubstitutionPlug> plugInsertedInvokers = new List<SubstitutionPlug>();
    static List<UnityAction<PlugID, char>> plugInsertedListeners = new List<UnityAction<PlugID, char>>();

    #endregion

    #region Methods

    /// <summary>
    /// Adds given script as invoker of "Key Encoded" event
    /// </summary>
    /// <param name="invoker">new invoker of event</param>
    public static void AddKeyEncodedInvoker(Enigma invoker)
    {
        // add invoker to list and add all listeners to this invoker
        keyEncodedInvokers.Add(invoker);
        foreach (UnityAction<char> listener in keyEncodedListeners)
        {
            invoker.AddKeyEncodedListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as listener for "Key Encoded" event
    /// </summary>
    /// <param name="listener">new listener for event</param>
    public static void AddKeyEncodedListener(UnityAction<char> listener)
    {
        // add listener to list and to all invokers
        keyEncodedListeners.Add(listener);
        foreach (Enigma invoker in keyEncodedInvokers)
        {
            invoker.AddKeyEncodedListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as invoker of "Darken Keys" event
    /// </summary>
    /// <param name="invoker">new invoker of event</param>
    public static void AddDarkenKeysInvoker(OutputKey invoker)
    {
        // add invoker to list and add all listeners to this invoker
        darkenKeysInvokers.Add(invoker);
        foreach (UnityAction listener in darkenKeysListeners)
        {
            invoker.AddDarkenKeyListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as listener for "Darken Keys" event
    /// </summary>
    /// <param name="listener">new listener for event</param>
    public static void AddDarkenKeysListener(UnityAction listener)
    {
        // add listener to list and to all invokers
        darkenKeysListeners.Add(listener);
        foreach (OutputKey invoker in darkenKeysInvokers)
        {
            invoker.AddDarkenKeyListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as invoker of "Rotor Set" event
    /// </summary>
    /// <param name="invoker">new invoker of event</param>
    public static void AddRotorSetInvoker (Rotor invoker)
    {
        // add invoker to list and add all listeners to this invoker
        rotorSetInvokers.Add(invoker);
        foreach (UnityAction<int, int> listener in rotorSetListeners)
        {
            invoker.AddRotorSetListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as listener for "Rotor Set" event
    /// </summary>
    /// <param name="listener">new listener for event</param>
    public static void AddRotorSetListener (UnityAction<int, int> listener)
    {
        // add listener to list and to all invokers
        rotorSetListeners.Add(listener);
        foreach (Rotor invoker in rotorSetInvokers)
        {
            invoker.AddRotorSetListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as invoker of "Advance Rotor" event
    /// </summary>
    /// <param name="invoker">new invoker of event</param>
    public static void AddAdvanceRotorInvoker (Enigma invoker)
    {
        // add invoker to list and add all listeners to this invoker
        advanceRotorInvokers.Add(invoker);
        foreach (UnityAction<int> listener in advanceRotorListeners)
        {
            invoker.AddAdvanceRotorListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as listener for "Advance Rotor" event
    /// </summary>
    /// <param name="listener">new listener for event</param>
    public static void AddAdvanceRotorListener (UnityAction<int> listener)
    {
        // add listener to list and to all invokers
        advanceRotorListeners.Add(listener);
        foreach (Enigma invoker in advanceRotorInvokers)
        {
            invoker.AddAdvanceRotorListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as invoker of "Wiring Set" event
    /// </summary>
    /// <param name="invoker">new invoker of event</param>
    public static void AddWiringSetInvoker (Rotor invoker)
    {
        // add invoker to list and add all listeners to this invoker
        wiringSetInvokers.Add(invoker);
        foreach (UnityAction<int, RotorModel> listener in wiringSetListeners)
        {
            invoker.AddWiringSetListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as listener for "Wiring Set" event
    /// </summary>
    /// <param name="listener">new listener for event</param>
    public static void AddWiringSetListener (UnityAction<int, RotorModel> listener)
    {
        // add listener to list and to all invokers
        wiringSetListeners.Add(listener);
        foreach (Rotor invoker in wiringSetInvokers)
        {
            invoker.AddWiringSetListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as invoker of "Reflector Set" event
    /// </summary>
    /// <param name="invoker">new invoker of event</param>
    public static void AddReflectorSetInvoker (Reflector invoker)
    {
        // add invoker to list and add all listeners to this invoker
        reflectorSetInvokers.Add(invoker);
        foreach (UnityAction<ReflectorModel> listener in reflectorSetListeners)
        {
            invoker.AddReflectorSetListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as listener for "Reflector Set" event
    /// </summary>
    /// <param name="listener">new listener for event</param>
    public static void AddReflectorSetListener (UnityAction<ReflectorModel> listener)
    {
        // add listener to list and to all invokers
        reflectorSetListeners.Add(listener);
        foreach (Reflector invoker in reflectorSetInvokers)
        {
            invoker.AddReflectorSetListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as invoker of "Plug Inserted" event
    /// </summary>
    /// <param name="invoker">new invoker of event</param>
    public static void AddPlugInsertedInvoker (SubstitutionPlug invoker)
    {
        // add invoker to list and add all listeners to this invoker
        plugInsertedInvokers.Add(invoker);
        foreach (UnityAction<PlugID, char> listener in plugInsertedListeners)
        {
            invoker.AddPlugInsertedListener(listener);
        }
    }

    /// <summary>
    /// Adds given script as listener for "Plug Inserted" event
    /// </summary>
    /// <param name="listener">new listener for event</param>
    public static void AddPlugInsertedListener (UnityAction<PlugID, char> listener)
    {
        // add listener to list and to all invokers
        plugInsertedListeners.Add(listener);
        foreach (SubstitutionPlug invoker in plugInsertedInvokers)
        {
            invoker.AddPlugInsertedListener(listener);
        }
    }

    #endregion
}
