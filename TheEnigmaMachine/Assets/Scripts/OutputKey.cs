using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// A lighted key designating output of a given letter
/// </summary>
public class OutputKey : MonoBehaviour
{
    // text display fields
    [SerializeField]
    char letter;
    Text keyText;

    // illumination support fields
    [SerializeField]
    Sprite unlitBackground;             // standard background of an unlit key
    [SerializeField]
    Sprite illuminatedBackground;       // background of illuminated key
    SpriteRenderer spriteRenderer;      // game object's sprite renderer -- used for sprite swapping
    Timer illuminationTimer;            // timer to handle key illumination
    [SerializeField]
    float illuminationDuration = 1f;    // duration key is lit for
    bool isIlluminated = false;         // flag determining whether key is currently illuminated

    // darken other keys event support
    DarkenKeysEvent darkenKeysEvent;

    /// <summary>
    /// Provides get / set access to key's letter to display
    /// </summary>
    public char Letter
    {
        get { return letter; }
        set
        {
            letter = value;

            // sets key text to display new letter
            // Note: letter is always set to upper-case
            keyText = GetComponentInChildren<Text>();
            keyText.text = letter.ToString().ToUpper();
        }
    }

    /// <summary>
    /// Called before the start method
    /// </summary>
    void Awake()
    {
        // sets key text to display new letter
        // Note: letter is always set to upper-case
        keyText = GetComponentInChildren<Text>();
        keyText.text = letter.ToString().ToUpper();
    }

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start()
    {
        // adds self as invoker of "Darken Keys" event
        darkenKeysEvent = new DarkenKeysEvent();
        EventManager.AddDarkenKeysInvoker(this);

        // adds Illuminate() method as listener for relevant events
        EventManager.AddKeyEncodedListener(Illuminate);
        EventManager.AddDarkenKeysListener(DarkenKey);

        // retrieves key's sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // adds (but does not start) an illumination timer
        illuminationTimer = gameObject.AddComponent<Timer>();
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        // if key is illuminated and illumination timer finishes
        if (isIlluminated && illuminationTimer.Finished)
        {
            // swap key's background for standard sprite
            spriteRenderer.sprite = unlitBackground;
            isIlluminated = false;

            // return key's letter text color to white
            keyText.color = Color.white;
        }
    }

    /// <summary>
    /// Illuminates game object if encoded key matches this key's letter
    /// </summary>
    /// <param name="encodedKey">key encoded by the enigma</param>
    void Illuminate(char encodedKey)
    {
        // if encoded key matches output key's letter
        if (encodedKey == letter)
        {
            // darken any currently-illuminated keys
            darkenKeysEvent.Invoke();

            // swap key's background for illuminated sprite
            spriteRenderer.sprite = illuminatedBackground;
            isIlluminated = true;

            // swap key's letter text color to black (for readability)
            keyText.color = Color.black;

            // begin illumination timer with set duration
            illuminationTimer.Duration = illuminationDuration;
            illuminationTimer.Run();
        }
    }

    /// <summary>
    /// Immediately darkens key before timer ends.
    /// Called when another key illuminates itself
    /// before this key's illumination timer ends.
    /// </summary>
    void DarkenKey()
    {
        // if output key is currently illuminated
        if (isIlluminated)
        {
            // stop illumination timer
            illuminationTimer.Stop();

            // swap key's sprite to unlit background
            spriteRenderer.sprite = unlitBackground;
            isIlluminated = false;

            // return key's letter text color to white
            keyText.color = Color.white;
        }
    }

    /// <summary>
    /// Adds given listener for the "Darken Keys" event
    /// </summary>
    /// <param name="newListener">new listener for event</param>
    public void AddDarkenKeyListener(UnityAction newListener)
    {
        darkenKeysEvent.AddListener(newListener);
    }
}
