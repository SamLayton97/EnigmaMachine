using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event triggered when enigma encodes a letter and 
/// automatically changes rotational position of rotor
/// </summary>
public class AdvanceRotorEvent : UnityEvent<int>
{
}
