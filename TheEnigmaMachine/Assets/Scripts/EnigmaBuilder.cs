using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds a series of output keys, rotors and plugboard 
/// to emulate an M3 enigma machine
/// </summary>
public class EnigmaBuilder : MonoBehaviour
{

    #region Fields

    // output key spawning support
    [SerializeField]
    GameObject outputKeyPrefab;             // saved prefab of output key
    CircleCollider2D keyCollider;           // reference to output key's collider
    float keyXOffset;                       // x-offset between key spawn locations
    float keyYOffset;                       // y-offset between rows of keys
    float[] keyXLocs = new float[3];        // initial x locations to spawn each row of keys
    float keyYLocation;                     // initial y location to spawn first rows of keys

    // arrays determining the rows of keys / plugboard sockets the script will build
    char[] topRowKeys = { 'q', 'w', 'e', 'r', 't', 'z', 'u', 'i', 'o' };
    char[] midRowKeys = { 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k' };
    char[] botRowKeys = { 'p', 'y', 'x', 'c', 'v', 'b', 'n', 'm', 'l' };
    List<char[]> keyboardRows = new List<char[]>();

    // rotor spawning fields
    [SerializeField]
    GameObject rotorPrefab;             // saved prefab of rotor game object
    BoxCollider2D rotorCollider;        // collider for the rotor game object
    int numOfRotors = 3;                // number of rotors to spawn -- constrained to 3 in current version
    float rotorYPos = 1.5f;             // y-position of each rotor
    float initRotorXPos;                // initial position to begin spawning row of rotors

    // reflector spawning fields
    [SerializeField]
    GameObject reflectorPrefab;                     // saved prefab of reflector game object
    BoxCollider2D reflectorCollider;                // collider for reflector game object
    Vector2 reflectorSpawnPoint = new Vector2();    // position to instantiate reflector display

    // plugboard socket spawning fields
    [SerializeField]
    GameObject socketPrefab;                // saved prefab of plugboard socket game object
    BoxCollider2D socketCollider;           // socket's collider box
    float socketXOffset;                    // x-offset between socket spawn locations
    float socketYOffset;                    // y-offset between socket spawn locations
    float[] socketXLocs = new float[3];     // initial x-locations to spawn each row of sockets
    float socketYLocation;                  // initial y-location to spawn first row of sockets

    // substitution plug spawning fields
    [SerializeField]
    GameObject plugPrefab;                  // saved prefab of substitution plug game object
    int numOfPlugPairs = 3;                 // number of substitution plug pairs to create
                                            // Note: Current version supports up to 3 pairs

    #endregion

    #region Methods

    /// <summary>
    /// Called before the Start() method
    /// </summary>
    void Awake()
    {
        // initialize relevant utility scripts
        ScreenUtils.Initialize();
    }

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start()
    {
        // save dimensions and spawn offset of output key
        keyCollider = outputKeyPrefab.GetComponent<CircleCollider2D>();
        keyXOffset = keyCollider.radius + .15f;
        keyYOffset = keyCollider.radius + .1f;

        // save initial spawn x-location of each key row
        keyXLocs[0] = .275f - (keyXOffset * (float)topRowKeys.Length / 2);
        keyXLocs[1] = .275f - (keyXOffset * (float)midRowKeys.Length / 2);
        keyXLocs[2] = .275f - (keyXOffset * (float)botRowKeys.Length / 2);

        // add each row of keys to multi-dimensional 'lightboard' list
        keyboardRows.Add(topRowKeys);
        keyboardRows.Add(midRowKeys);
        keyboardRows.Add(botRowKeys);

        // create a 3-row lightboard
        for (int i = 0; i < keyboardRows.Count; i++)
        {
            // fill each row with corresponding output keys
            for (int j = 0; j < keyboardRows[i].Length; j++)
            {
                // instantiate new output key at proper place in row
                GameObject newKey = Instantiate(outputKeyPrefab,
                    new Vector2(keyXLocs[i] + (keyXOffset * j), -(keyYOffset * i)),
                    Quaternion.identity);

                // assign new key its proper letter
                newKey.GetComponent<OutputKey>().Letter = keyboardRows[i][j];
            }
        }

        // save dimensions and initial spawn position of rotors
        rotorCollider = rotorPrefab.GetComponent<BoxCollider2D>();
        initRotorXPos = rotorCollider.size.x + .4f;

        // create a row of enigma rotors
        for (int i = 0; i < numOfRotors; i++)
        {
            // instantiate new rotor at proper place in row
            GameObject newRotor = Instantiate(rotorPrefab,
                new Vector2(initRotorXPos - (i * initRotorXPos), rotorYPos),
                Quaternion.identity);

            // assign default rotational position and model to new rotor
            Rotor rotorComp = newRotor.GetComponent<Rotor>();
            rotorComp.RotationalPosition = 1;
            rotorComp.Model = RotorModel.I;

            // assign enigma position to rotor according to its order in creation
            rotorComp.PositionInEnigma = i;
        }

        // save dimensions and spawn point of reflector
        reflectorCollider = reflectorPrefab.GetComponent<BoxCollider2D>();
        reflectorSpawnPoint.x = 0 - initRotorXPos - (2.5f * reflectorCollider.size.x);
        reflectorSpawnPoint.y = rotorYPos;

        // spawn reflector display and assign defaults to its model
        GameObject newReflector = Instantiate(reflectorPrefab, reflectorSpawnPoint, Quaternion.identity);
        newReflector.GetComponent<Reflector>().Model = ReflectorModel.A;

        // save dimensions and spawn offsets of plugboard sockets
        socketCollider = socketPrefab.GetComponent<BoxCollider2D>();
        socketXOffset = socketCollider.size.x + .3f;
        socketYOffset = socketCollider.size.y + .1f;

        // save initial spawn x-locations of each socket row
        socketXLocs[0] = .35f - (socketXOffset * (float)topRowKeys.Length / 2);
        socketXLocs[1] = .35f - (socketXOffset * (float)midRowKeys.Length / 2);
        socketXLocs[2] = .35f - (socketXOffset * (float)botRowKeys.Length / 2);

        // save initial spawn y-location to spawn socket rows
        socketYLocation = ScreenUtils.ScreenBottom + (3.5f * socketCollider.size.y);

        // create a 3-row plugboard
        for (int i = 0; i < keyboardRows.Count; i++)
        {
            // fill each row with corresponding plugboard sockets
            for (int j = 0; j < keyboardRows[i].Length; j++)
            {
                // instantiate new socket at proper place in row
                GameObject newSocket = Instantiate(socketPrefab,
                    new Vector2(socketXLocs[i] + (socketXOffset * j), socketYLocation - (socketYOffset * i)),
                    Quaternion.identity);

                // assign proper letter and unique tag to new socket
                newSocket.GetComponent<PlugboardSocket>().Letter = keyboardRows[i][j];
                newSocket.tag = "socket" + keyboardRows[i][j];
            }
        }

        // create a 3 sets of substitution plug pairs
        for (int i = 0; i < numOfPlugPairs * 2; i++)
        {
            // instantiate new plug and assign it unique ID
            GameObject newPlug = Instantiate(plugPrefab);
            newPlug.GetComponent<SubstitutionPlug>().ID = (PlugID)i;
        }
    }
    
    #endregion

}
