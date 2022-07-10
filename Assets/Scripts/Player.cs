using System;
using UnityEngine;

public delegate void PlayerMoving(Player player, Vector3 direction);

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

    public event PlayerMoving onPlayerMoving;

    private InputHandler inputhandler;

    bool playerCanMove;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInputHandler playerHandler = new PlayerInputHandler(gameObject);
        playerHandler.onPlayerTryMoving += TryPlayerMoving;
        inputhandler = playerHandler;
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

    void TryPlayerMoving(Player player, Vector3 direction)
    {
        onPlayerMoving.Invoke(player, direction);
    }

    public void onGameStateChanged(GameState newGameState)
    {
        playerCanMove = newGameState == GameState.PlayerMoving;
    }
}

public class PlayerInputHandler : InputHandler
{
    private GameObject player;

    public event PlayerMoving onPlayerTryMoving;

    public PlayerInputHandler(GameObject player)
    {
        this.player = player;
    }

    public bool KeyCheck()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // «авершение движени€
            return true;
        }
        return false;
    }

    public void onObjectSelected(GameObject block)
    {
        // TODO перемещение к позиции
    }
}