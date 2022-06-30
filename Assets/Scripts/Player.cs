using System;
using System.Collections;
using System.Collections.Generic;
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

    bool playerCanMove;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCanMove)
        {
            if (KeyCheck())
            {
                finishCurrentState(this, new EventArgs());
            }
        }
    }

    private bool KeyCheck()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            transform.position += new Vector3(0, 1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            transform.position += new Vector3(0, -1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            transform.position += new Vector3(-1, 0, 0);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            transform.position += new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            // Move diagonal up
            // \
            //  \
            //   \
            transform.position += new Vector3(-1, 1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            // Move diagonal down
            // \
            //  \
            //   \
            transform.position += new Vector3(1, -1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            // Move diagonal up
            //   /
            //  /
            // /
            transform.position += new Vector3(1, 1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            // Move diagonal down
            //   /
            //  /
            // /
            transform.position += new Vector3(-1, -1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            return true;
        }
        return false;
    }

    public void onGameStateChanged(GameState newGameState)
    {
        playerCanMove = newGameState == GameState.PlayerMoving;
    }
}
