using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerType
    {
        NPC,
        Human
    }

    [SerializeField]
    public string playerName;
    [SerializeField]
    public PlayerType playerType;

    public EventHandler finishCurrentState;

    private InputHandler inputhandler;

    bool playerCanMove;
    // Start is called before the first frame update
    void Start()
    {
        inputhandler = new PlayerInputHandler(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCanMove && inputhandler.KeyCheck())
        {
            finishCurrentState(this, new EventArgs());
        }
    }

    public InputHandler getInputHandler()
    {
        return inputhandler;
    }

    public void onGameStateChanged(GameState newGameState)
    {
        playerCanMove = newGameState == GameState.PlayerMoving;
    }
}

public class PlayerInputHandler : InputHandler
{
    private GameObject player;

    public PlayerInputHandler(GameObject player)
    {
        this.player = player;
    }

    public bool KeyCheck()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            player.transform.position += new Vector3(0, 1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            player.transform.position += new Vector3(0, -1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            player.transform.position += new Vector3(-1, 0, 0);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            player.transform.position += new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            // Move diagonal up
            // \
            //  \
            //   \
            player.transform.position += new Vector3(-1, 1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            // Move diagonal down
            // \
            //  \
            //   \
            player.transform.position += new Vector3(1, -1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            // Move diagonal up
            //   /
            //  /
            // /
            player.transform.position += new Vector3(1, 1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            // Move diagonal down
            //   /
            //  /
            // /
            player.transform.position += new Vector3(-1, -1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            return true;
        }
        return false;
    }

    public void onObjectClicked(GameObject block)
    {
        // TODO перемещение к позиции
    }
}