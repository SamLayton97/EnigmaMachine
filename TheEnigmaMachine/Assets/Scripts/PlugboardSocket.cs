using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A socket for letter-substitution plugs
/// Note: Plugboard letter-substitution handled in
/// Enigma script.
/// </summary>
public class PlugboardSocket : MonoBehaviour
{
    // letter display fields
    [SerializeField]
    char plugLetter;
    Text letterText;

    // plug-insertion support fields
    bool occupied = false;

    /// <summary>
    /// Provides read / write access to letter which socket connects to
    /// </summary>
    public char Letter
    {
        get { return plugLetter; }
        set
        {
            plugLetter = value;

            // Update letter text to reflect value
            letterText.text = plugLetter.ToString().ToUpper();
        }
    }

    /// <summary>
    /// Provides read / write access to whether socket is occupied by a plug
    /// </summary>
    public bool Occupied
    {
        get { return occupied; }
        set { occupied = value; }
    }

    /// <summary>
    /// Called before Start() method
    /// </summary>
    void Awake()
    {
        // retrieve letter text and set it to display current letter
        // Note: letter is always set to upper-case
        letterText = GetComponentInChildren<Text>();
        letterText.text = plugLetter.ToString().ToUpper();
    }

}
