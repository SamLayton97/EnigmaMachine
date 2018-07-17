using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event triggered when the enigma encodes
/// a key to display
/// </summary>
public class KeyEncodedEvent : UnityEvent<char>
{
}
