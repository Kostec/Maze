using Assets.Scripts;
using Assets.Scripts.Servers;
using Assets.Scripts.Servers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameState : uint
{
    None,
    FieldShifting,
    BlockRotate,
    PlayerMoving
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public FieldController fieldController;
    [SerializeField]
    public uint playerNumber;

    private GameState gameState;
    public GameState GameState { 
        get
        {
            return gameState;
        }
        set
        {
            gameState = value;
            if (onGameStateChanged != null)
            {
                onGameStateChanged(gameState);
            }
        }
    }
    public delegate void GameStateEventHandler(GameState newgameState);
    public event GameStateEventHandler onGameStateChanged;

    private List<Player> players = new List<Player>();
    private int activePlayerCount = -1;
    private Player activePlayer = null;
    private Pathfinder pathfinder = new Pathfinder();
    public static IServerWrapper serverWrapper = new DummyServer();

    // Start is called before the first frame update
    void Start()
    {
        fieldController.finishCurrentState += FinishCurrentState;
        fieldController.onBlockClickedEvent += onBlockClicked;
        onGameStateChanged += fieldController.onGameStateChanged;

        GameState = GameState.FieldShifting;

        for (int i = 0; i < 4; i++)
        {
            IPlayer serverPlayer = serverWrapper.CreatePlayer($"Player {i + 1}", PlayerType.Human);
            if (serverPlayer == null)
            {
                break;
            }
            GameObject player = Instantiate(playerPrefab, serverPlayer.InitalPosition, Quaternion.identity);
            Player _player = player.GetComponent<Player>();
            _player.Init(serverPlayer);

            onGameStateChanged += _player.onGameStateChanged;
            _player.finishCurrentState += FinishCurrentState;

            var playerBlock = fieldController.GetBlock(_player.Position);
            _player.SetBaseItem(playerBlock);
            players.Add(_player);
        }
    }

    void MovePlayerToTarget(Player player, Vector3 target)
    {
        pathfinder.SetField(fieldController.GetFieldArray());
        Vector3 playerPosition = new Vector3(player.transform.position.x, player.transform.position.y);
        var path = pathfinder.EstimatePath(playerPosition, target);
        if (path.Count == 0)
        {
            Debug.Log("Onreachable path");
            return;
        }
        Vector3 newPosition = path.First();
        newPosition.z = -1;
        player.Position = newPosition;
        var newBlock = fieldController.GetBlock(newPosition);
        player.SetBaseItem(newBlock);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FinishCurrentState(object sender, EventArgs e)
    {
        switch (gameState)
        {
            case GameState.FieldShifting:
                GameState = GameState.BlockRotate;
                break;
            case GameState.BlockRotate:
                activePlayerCount++;
                if(activePlayerCount >= IServerWrapper.MaximumPlayerCount)
                {
                    activePlayerCount = 0;
                }
                if (activePlayer != null)
                {
                    activePlayer.isActive = false;
                }
                activePlayer = players[activePlayerCount];
                activePlayer.isActive = true;
                GameState = GameState.PlayerMoving;
                break;
            case GameState.PlayerMoving:
                GameState = GameState.FieldShifting;
                break;
            default:
                GameState = GameState.None;
                break;
        }
        Debug.Log($"GameState: {gameState}");
    }

    private void onBlockClicked(GameObject obj)
    {
        Block block = obj.GetComponent<Block>();

        if(GameState == GameState.PlayerMoving)
        {
            MovePlayerToTarget(activePlayer, block.transform.position);
        }
    }
}