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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
