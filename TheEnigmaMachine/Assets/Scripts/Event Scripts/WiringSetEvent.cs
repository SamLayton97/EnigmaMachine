using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event triggered when user manually changes
/// wiring / model of a rotor
/// </summary>
public class WiringSetEvent : UnityEvent<int, RotorModel>
{
}
