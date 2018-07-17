using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event triggered when the user inserts a 
/// substitution plug into a new socket
/// </summary>
public class PlugInsertedEvent : UnityEvent<PlugID, char>
{
}
