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

    GameObject player;
    private Pathfinder pathfinder;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 playerSpawnPosition = new Vector3(0, 0, -1);
        player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        Player _player = player.GetComponent<Player>();
        _player.onPlayerMoving += PlayerMoveTo;

        fieldController.finishCurrentState += FinishCurrentState;
        fieldController.onBlockClickedEvent += onBlockClicked;
        fieldController.OnLineShiftedEvent += onLineShifted;
        onGameStateChanged += fieldController.onGameStateChanged;
        onGameStateChanged += _player.onGameStateChanged;
        _player.finishCurrentState += FinishCurrentState;

        GameState = GameState.FieldShifting;
        pathfinder = new Pathfinder();

        var playerBlock = fieldController.GetBlock(new Vector3(0, 0, 0));
        playerBlock.AttachItem(player);
    }

    void MovePlayerToTarget(GameObject player, Vector3 target)
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
        player.transform.position = newPosition;
    }

    void PlayerMoveTo(Player player, Vector3 direction)
    {
        Block block = fieldController.GetBlock(player.transform.position);
        Block targetBblock = fieldController.GetBlock(player.transform.position + direction);
        direction.Normalize();
        if ((block != null && block.PossibleDirections.Contains(direction)) &&
                (targetBblock != null && targetBblock.PossibleDirections.Contains(-direction)))
        {
            player.gameObject.transform.position += direction;
            block.GetComponent<Block>().DetachItem(player.gameObject);
            targetBblock.GetComponent<Block>().AttachItem(player.gameObject);
        }
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

    private void onLineShifted(IEnumerable<Vector3> line, Vector3 offset)
    {
        //player.transform.position += offset;
    }

    private void onBlockClicked(GameObject obj)
    {
        Block block = obj.GetComponent<Block>();

        if(GameState == GameState.PlayerMoving)
        {
            MovePlayerToTarget(player, block.transform.position);
        }
    }
}