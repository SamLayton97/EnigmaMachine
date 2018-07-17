using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event triggered when user manually changes 
/// rotational position of a rotor
/// </summary>
public class RotorSetEvent : UnityEvent<int, int>
{
}
