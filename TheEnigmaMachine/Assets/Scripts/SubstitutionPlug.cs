using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A moveable letter substitution plug to be inserted into the plugboard.
/// Note: Plugboard letter-substitution handled in Enigma script.
/// </summary>
public class SubstitutionPlug : MonoBehaviour
{
    // identification fields
    [SerializeField]
    PlugID id = PlugID.RedA;                    // unique ID given to this plug
    SpriteRenderer spriteRenderer;              // reference to plug's sprite renderer -- used to change sprite according to ID
    [SerializeField]
    Sprite redPlugSprite;                       // sprite used by red plug pairs
    [SerializeField]
    Sprite bluePlugSprite;                      // sprite used by blue plug pairs
    [SerializeField]
    Sprite yellowPlugSprite;                    // sprite used by yellow plug pairs

    // placement fields
    string stdAlphabet = "abcdefghijklmnopqrstuvwxyz";      // standard alphabet used for initial plug placement
    char occupiedLetter;                                    // current letter which plug occupies
    PlugboardSocket currSocket;                             // reference to occupied plugboard socket
    Vector2 currSocketPos = new Vector2();                  // transform position of currently occupied socket

    // movement support fields
    bool heldByUser = false;        // flag indicating whether user currently holds this plug
    BoxCollider2D plugCollider;     // reference to plug's collider box

    // insertion event support
    PlugInsertedEvent plugInsertedEvent;

    /// <summary>
    /// Provides read / write access to plug's ID
    /// </summary>
    public PlugID ID
    {
        get { return id; }
        set { id = value; }
    }

    /// <summary>
    /// Provides read / write access to plug's current letter socket
    /// </summary>
    public char OccupiedLetter
    {
        get { return occupiedLetter; }
        set { occupiedLetter = value; }
    }

    /// <summary>
    /// Use for initialization
    /// </summary>
    void Awake()
    {
        // retrieve relevant components
        plugCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start()
    {
        // add self as invoker of "Plug Inserted" event
        plugInsertedEvent = new PlugInsertedEvent();
        EventManager.AddPlugInsertedInvoker(this);

        // set plug's sprite according to set ID
        switch (id)
        {
            case PlugID.RedA:
                spriteRenderer.sprite = redPlugSprite;
                break;
            case PlugID.RedB:
                spriteRenderer.sprite = redPlugSprite;
                break;
            case PlugID.BlueA:
                spriteRenderer.sprite = bluePlugSprite;
                break;
            case PlugID.BlueB:
                spriteRenderer.sprite = bluePlugSprite;
                break;
            case PlugID.YellowA:
                spriteRenderer.sprite = yellowPlugSprite;
                break;
            case PlugID.YellowB:
                spriteRenderer.sprite = yellowPlugSprite;
                break;
            default:
                break;
        }

        // bind to first plug according to ID
        BindToSocket(GameObject.FindGameObjectWithTag("socket" + stdAlphabet[(int)id]));
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        // if plug is currently held by user
        if (heldByUser)
        {
            // follow user's mouse position
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = transform.position.z - Camera.main.transform.position.z;
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    /// <summary>
    /// Called when user clicks within plug's collision box
    /// </summary>
    void OnMouseDown()
    {
        // if not currently held by user
        if (!heldByUser)
        {
            // begin to follow mouse position
            // and disable collisions between this plug and sockets
            heldByUser = true;
            Physics2D.IgnoreLayerCollision(8, 9, true);
        }
        // otherwise (i.e., currently held by user)
        else
        {
            // unfollow mouse position
            // and enable plug-socket collisions
            heldByUser = false;
            Physics2D.IgnoreLayerCollision(8, 9, false);

            // TODO: snap to position of current socket
            //transform.position = currSocketPos;
        }
    }

    /// <summary>
    /// Called when plug's collider box intersects with other collider
    /// </summary>
    /// <param name="other">detected collision</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // if other object exists on "socket" layer
        if (other.gameObject.layer == 9)
        {
            // attempt to bind plug to collided socket
            BindToSocket(other.gameObject);
        }
    }

    /// <summary>
    /// Called when plug's collider box exits another object's
    /// </summary>
    /// <param name="other">other collider box</param>
    void OnTriggerExit2D(Collider2D other)
    {
        // if currently occupying a socket, unoccupy it
        if (currSocket != null)
        {
            currSocket.Occupied = false;
        }
    }

    /// <summary>
    /// Inserts substitution plug into a plugboard socket
    /// </summary>
    /// <param name="socket">socket to bind plug to</param>
    void BindToSocket (GameObject socket)
    {
        // if socket is on "socket" layer and isn't occupied
        if (socket.layer == 9 && 
            !socket.GetComponent<PlugboardSocket>().Occupied)
        {
            // move plug's position to cover socket
            currSocketPos = socket.transform.position;
            transform.position = currSocketPos;

            // occupy socket and set plug's occupied letter
            currSocket = socket.GetComponent<PlugboardSocket>();
            currSocket.Occupied = true;
            occupiedLetter = currSocket.Letter;

            // TODO: update plugboard info within Enigma script (event)
            plugInsertedEvent.Invoke(id, occupiedLetter);

            // TEST: send binding feedback to debug log
            //Debug.Log("Plug " + id + " bound to socket " + currSocket.Letter);
        }
    }

    /// <summary>
    /// Adds new listener for "Plug Inserted" event
    /// </summary>
    /// <param name="newListener">new listener for event</param>
    public void AddPlugInsertedListener(UnityAction<PlugID, char> newListener)
    {
        plugInsertedEvent.AddListener(newListener);
    }
}
