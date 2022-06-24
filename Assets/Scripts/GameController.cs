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

    // Start is called before the first frame update
    void Start()
    {
        blockController.finishCurrentState += FinishCurrentState;
        onGameStateChanged += blockController.onGameStateChanged;

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