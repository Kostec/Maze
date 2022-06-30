using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState : uint
{
    FieldShifting,
    BlockRotate,
    PlayerMoving
}


public class GameController : MonoBehaviour
{
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public BlockController blockController;
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

    // Start is called before the first frame update
    void Start()
    {
        Vector3 playerSpawnPosition = new Vector3(0, 0, -1);
        player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        Player _player = player.GetComponent<Player>();

        blockController.finishCurrentState += FinishCurrentState;
        onGameStateChanged += blockController.onGameStateChanged;
        onGameStateChanged += _player.onGameStateChanged;
        _player.finishCurrentState += FinishCurrentState;

        GameState = GameState.FieldShifting;
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
        }
        Debug.Log($"GameState: {gameState}");
    }

}