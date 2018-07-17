using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simulation of the M3 enigma machine
/// </summary>
public class Enigma : MonoBehaviour
{

    #region Fields

    // input recognition fields
    KeyCode[] acceptedInputs = {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
                                KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
                                KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
                                KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
                                KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,KeyCode.Y,
                                KeyCode.Z};

    // rotor wiring details
    Dictionary<RotorModel, string> rotorWirings = new Dictionary<RotorModel, string>();
    string stdAlphabet = "abcdefghijklmnopqrstuvwxyz";
    string rotorIWiring = "EKMFLGDQVZNTOWYHXUSPAIBRCJ".ToLower();
    string rotorIIWiring = "AJDKSIRUXBLHWTMCQGZNPYFVOE".ToLower();
    string rotorIIIWiring = "BDFHJLCPRTXVZNYEIWGAKMUSQO".ToLower();
    string rotorM3IVWiring = "ESOVPZJAYQUIRHXLNFTGKDCMWB".ToLower();
    string rotorM3VWiring = "VZBRGITYUPSDNHLXAWMJQOFECK".ToLower();

    // reflector wiring details
    Dictionary<ReflectorModel, string> reflectorWirings = new Dictionary<ReflectorModel, string>();
    string reflectorAWiring = "EJMZALYXVBWFCRQUONTSPIKHGD".ToLower();
    string reflectorBWiring = "YRUHQSLDPXNGOKMIEBFZCWVJAT".ToLower();
    string reflectorCWiring = "FVPJIAOYEDRZXWGCTKUQSBNMHL".ToLower();

    // current rotor and reflector fields
    // Note: current version of Enigma supports only 3 rotors
    string[] currentRotors = new string[3];         // array storing the current rotors' models and wirings
    string currentReflector;                        // reference to the current reflector in enigma
    int[] rotorPositions = new int[3];              // array storing the rotational positions of each rotor
    char[] turnoverPos = new char[3];               // array storing tunrover positions on each rotor determining when next should advance
                                                    // Note: turnover point entails that next rotor advances when current rotor leaves this position

    // plugboard support
    Dictionary<PlugID, char> plugboardInfo = new Dictionary<PlugID, char>();

    // event support
    KeyEncodedEvent keyEncodedEvent;
    AdvanceRotorEvent advanceRotorEvent;
    
    #endregion

    #region Encoding Methods

    /// <summary>
    /// Encodes a letter
    /// </summary>
    /// <param name="letterToEncode">user input</param>
    /// <returns>encoded letter</returns>
    char EncodeInput(char letterToEncode)
    {
        // pass user input through the plugboard
        char newInput = PlugboardSubstitution(letterToEncode);

        // determine parially converted letter from forward pass though rotors (i.e., what is to be reflected)
        char partialConvChar = PositionalConversion(letterToEncode, 0, false);
        partialConvChar = WiringConversion(partialConvChar, 0, true);
        partialConvChar = PositionalConversion(partialConvChar, 0, 1);
        partialConvChar = WiringConversion(partialConvChar, 1, true);
        partialConvChar = PositionalConversion(partialConvChar, 1, 2);
        partialConvChar = WiringConversion(partialConvChar, 2, true);
        partialConvChar = PositionalConversion(partialConvChar, 2, true);

        // determine reflected letter from current reflector model
        char reflectedChar = WiringConversion(partialConvChar, currentReflector);

        // determine fully converted letter from backwards pass through rotors (i.e, what returns to static wheel)
        char fullConvChar = PositionalConversion(reflectedChar, 2, false);
        fullConvChar = WiringConversion(fullConvChar, 2, false);
        fullConvChar = PositionalConversion(fullConvChar, 2, 1);
        fullConvChar = WiringConversion(fullConvChar, 1, false);
        fullConvChar = PositionalConversion(fullConvChar, 1, 0);
        fullConvChar = WiringConversion(fullConvChar, 0, false);
        fullConvChar = PositionalConversion(fullConvChar, 0, true);

        // pass converted character back through plugboard
        char freshOutput = PlugboardSubstitution(fullConvChar);

        // return encoded character
        return freshOutput;
    }

    /// <summary>
    /// Converts given letter through wiring of single rotor.
    /// Overload used when letter passes through a rotor rather
    /// than reflector.
    /// </summary>
    /// <param name="letterToConvert">letter to pass through rotor</param>
    /// <param name="rotorPosition">which rotor in enigma to pass through</param>
    /// <param name="passingForward">whether character is passing forwards or backwards through wiring</param>
    /// <returns>newly converted letter</returns>
    char WiringConversion(char letterToConvert, int rotorPosition, bool passingForward)
    {
        int wiringLoc;      // position of wire to pass letter through
        int shiftVal;       // resultant value to shift letter by

        // if input passes forward through rotor wiring
        if (passingForward)
        {
            // calculate wiring shift value as normal
            wiringLoc = stdAlphabet.IndexOf(letterToConvert);
            shiftVal = currentRotors[rotorPosition][wiringLoc] - stdAlphabet[wiringLoc];
        }
        // otherwise (i.e., reflected input passing back through rotor)
        else
        {
            // calculate wiring shift value in reverse
            wiringLoc = currentRotors[rotorPosition].IndexOf(letterToConvert);
            shiftVal = stdAlphabet[wiringLoc] - currentRotors[rotorPosition][wiringLoc];
        }

        // apply shift value to letter and confine result to std alphabet
        letterToConvert += (char)shiftVal;
        while (letterToConvert > 'z')
        {
            letterToConvert -= (char)26;
        }
        while (letterToConvert < 'a')
        {
            letterToConvert += (char)26;
        }

        // return resulting character
        return letterToConvert;
    }

    /// <summary>
    /// Converts letter through wiring of a single reflector
    /// Overload used when letter passes through reflector
    /// rather than rotor.
    /// </summary>
    /// <param name="letterToReflect">letter to reflect</param>
    /// <param name="reflectorModel">model of reflector wiring in enigma</param>
    /// <returns>reflected letter</returns>
    char WiringConversion(char letterToReflect, string reflectorModel)
    {
        // return reflected value
        return reflectorModel[stdAlphabet.IndexOf(letterToReflect)];
    }

    /// <summary>
    /// Converts character based on rotational position of rotors it passes between.
    /// Overload used when input passes between two non-static rotors
    /// </summary>
    /// <param name="letterToConvert">letter to apply conversion to</param>
    /// <param name="prevRotor">rotor letter has just passed through</param>
    /// <param name="nextRotor">rotor letter will pass through next</param>
    /// <returns>newly converted letter</returns>
    char PositionalConversion(char letterToConvert, int prevRotor, int nextRotor)
    {
        // calculate positional shift value and apply to letter
        int shiftVal = rotorPositions[nextRotor] - rotorPositions[prevRotor];
        letterToConvert += (char)shiftVal;

        // confine resulting letter to standard alphabet
        while (letterToConvert > 'z')
        {
            letterToConvert -= (char)26;
        }
        while (letterToConvert < 'a')
        {
            letterToConvert += (char)26;
        }

        // return converted letter 
        return letterToConvert;
    }

    /// <summary>
    /// Converts character based on rotational position of rotor and static wheel
    /// it passes between.
    /// Overload used when input passes between non-static and static wheels
    /// </summary>
    /// <param name="letterToConvert">letter to apply conversion to</param>
    /// <param name="rotorNumber">position in enigma of non-static rotor involved in conversion</param>
    /// <param name="fromRotorToStatic">whether letter passes from rotor to static wheel</param>
    /// <returns>newly converted character</returns>
    char PositionalConversion(char letterToConvert, int rotorNumber, bool fromRotorToStatic)
    {
        // if letter passes from rotor to static wheel
        if (fromRotorToStatic)
        {
            // shift letter according to rotational position from
            // rotor to static wheel / reflector
            letterToConvert -= (char)(rotorPositions[rotorNumber] - 1);
        }
        // otherwise (i.e., letter passes from static wheel to rotor)
        else
        {
            // shift letter according to rotational position from
            // static wheel / reflector to non-static rotor
            letterToConvert += (char)(rotorPositions[rotorNumber] - 1);
        }

        // confine resulting letter to standard alphabet
        while (letterToConvert > 'z')
        {
            letterToConvert -= (char)26;
        }
        while (letterToConvert < 'a')
        {
            letterToConvert += (char)26;
        }

        // return converted letter 
        return letterToConvert;
    }

    /// <summary>
    /// Converts character based on plug pair data within plugboard
    /// </summary>
    /// <param name="letterToSub">letter to pass through plugboard</param>
    /// <returns>newly substituted character</returns>
    char PlugboardSubstitution(char letterToSub)
    {
        // iterate over internal plugboard connections
        for (int i = 0; i < plugboardInfo.Count; i++)
        {
            // if letter to convert matches value of even-numbered plug
            if (letterToSub == plugboardInfo[(PlugID)i]
                && i % 2 == 0)
            {
                // substitute letter to convert with following value (plug's partner)
                return plugboardInfo[(PlugID)(i + 1)];
            }
            // if letter to convert matches value of odd-numbered plug
            else if (letterToSub == plugboardInfo[(PlugID)i]
                && i % 2 == 1)
            {
                // substitute letter to convert with preceding value (plug's partner)
                return plugboardInfo[(PlugID)(i - 1)];
            }
        }

        // if input letter does not match any plugs, return itself
        return letterToSub;
    }

    #endregion

    #region Other Methods

    // Use this for initialization
    void Start()
    {
        // pair rotor models with appropriate wiring details in a dictionary
        rotorWirings.Add(RotorModel.I, rotorIWiring);
        rotorWirings.Add(RotorModel.II, rotorIIWiring);
        rotorWirings.Add(RotorModel.III, rotorIIIWiring);
        rotorWirings.Add(RotorModel.IV, rotorM3IVWiring);
        rotorWirings.Add(RotorModel.V, rotorM3VWiring);

        // pair reflector models with appropriate wiring details in a dictionary
        reflectorWirings.Add(ReflectorModel.A, reflectorAWiring);
        reflectorWirings.Add(ReflectorModel.B, reflectorBWiring);
        reflectorWirings.Add(ReflectorModel.C, reflectorCWiring);

        // adds self as invoker of the relevant events
        keyEncodedEvent = new KeyEncodedEvent();
        EventManager.AddKeyEncodedInvoker(this);
        advanceRotorEvent = new AdvanceRotorEvent();
        EventManager.AddAdvanceRotorInvoker(this);

        // adds self as listener for relevant events
        EventManager.AddRotorSetListener(DetectManualRotorShift);
        EventManager.AddWiringSetListener(DetectRotorRewiring);
        EventManager.AddReflectorSetListener(DetectReflectorRewiring);
        EventManager.AddPlugInsertedListener(DetectPlugInsertion);

        // iterate through rotors in machine
        for (int i = 0; i < 3; i++)
        {
            // set both rotor model, rotational position, and turnover points to safe defaults
            currentRotors[i] = rotorWirings[RotorModel.I];
            rotorPositions[i] = 1;
            turnoverPos[i] = 'q';
        }

        // set current reflector to save default
        currentReflector = reflectorAWiring;

        // pair substitution plug IDs with starting plugboard details in a dictionary
        // Note: current version of project supports 3 plug pairs
        for (int i = 0; i < 6; i++)
        {
            plugboardInfo.Add((PlugID)i, stdAlphabet[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // if enigma detects input from user
        if (Input.anyKeyDown)
        {
            // if KeyCode of input matches one of enigma's accepted inputs
            foreach (KeyCode input in acceptedInputs)
            {
                if (Input.GetKeyDown(input))
                {
                    // encode letter and light matching output key
                    char encodedLetter = EncodeInput((char)input);
                    keyEncodedEvent.Invoke(encodedLetter);

                    // advance right-most rotor
                    rotorPositions[0]++;
                    while (rotorPositions[0] > 26)
                    {
                        rotorPositions[0] -= 26;
                    }
                    advanceRotorEvent.Invoke(0);

                    // if right rotor's new position matches its turnover point
                    if (rotorPositions[0] == stdAlphabet.IndexOf(turnoverPos[0]))
                    {
                        // advance middle rotor
                        rotorPositions[1]++;
                        while (rotorPositions[1] > 26)
                        {
                            rotorPositions[1] -= 26;
                        }
                        advanceRotorEvent.Invoke(1);

                        // if middle rotor's new position matches its turnover point
                        if (rotorPositions[1] == stdAlphabet.IndexOf(turnoverPos[1]))
                        {
                            // advance left rotor
                            rotorPositions[2]++;
                            while (rotorPositions[2] > 26)
                            {
                                rotorPositions[2] -= 26;
                            }
                            advanceRotorEvent.Invoke(2);
                        }
                    }

                    break;
                }
            }
        }

        // if user presses ESC, close enigma application
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    /// <summary>
    /// Adjusts internal rotational position of a given rotor
    /// </summary>
    /// <param name="rotorNumber">which rotor shifted in enigma</param>
    /// <param name="newPosition">new rotational position of rotor </param>
    void DetectManualRotorShift(int rotorNumber, int newPosition)
    {
        rotorPositions[rotorNumber] = newPosition;
    }

    /// <summary>
    /// Adjusts internal wiring of a given rotor
    /// </summary>
    /// <param name="rotorNumber">which rotor shifted wiring</param>
    /// <param name="newModel">new wiring model for given rotor</param>
    void DetectRotorRewiring(int rotorNumber, RotorModel newModel)
    {
        currentRotors[rotorNumber] = rotorWirings[newModel];
    }

    /// <summary>
    /// Adjusts internal wiring of reflector
    /// </summary>
    /// <param name="newModel">new wiring model for reflector</param>
    void DetectReflectorRewiring(ReflectorModel newModel)
    {
        currentReflector = reflectorWirings[newModel];
    }

    /// <summary>
    /// Adjusts internal connections of plugboard
    /// </summary>
    /// <param name="plug">ID of newly inserted plug</param>
    /// <param name="socket">socket which plug was inserted into</param>
    void DetectPlugInsertion(PlugID plug, char socket)
    {
        plugboardInfo[plug] = socket;
    }

    /// <summary>
    /// Adds given listener for the "Key Encoded" event
    /// </summary>
    /// <param name="newListener">new listener for event</param>
    public void AddKeyEncodedListener(UnityAction<char> newListener)
    {
        keyEncodedEvent.AddListener(newListener);
    }

    /// <summary>
    /// Adds given listener for the "Advance Rotor" event
    /// </summary>
    /// <param name="newListener">new listener for event</param>
    public void AddAdvanceRotorListener(UnityAction<int> newListener)
    {
        advanceRotorEvent.AddListener(newListener);
    }

    #endregion

}
