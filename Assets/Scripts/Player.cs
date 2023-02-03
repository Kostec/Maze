using Assets.Scripts;
using Assets.Scripts.Servers.Interfaces;
using Assets.Scripts.Servers.Items;
using System;
using UnityEngine;

public delegate void PlayerMoving(Player player, Vector3 direction);

public class Player : MonoBehaviour, IPlayer, IFieldItem
{
    public EventHandler finishCurrentState;
    public uint Id { get; private set; }
    public string Name { get; private set; }
    public PlayerType Type { get; private set; }

    public int Angle { get; }

    public event PlayerMoving onPlayerMoving;
    public event FieldItemShifted ItemShifted;
    public event FieldItemRotated ItemRotated;
    public event FieldPositionChanged ItemPositionChanged;

    private InputHandler inputhandler;
    private IFieldItem baseItem; // Блок на котором стоит игрок

    bool playerCanMove;

    public Vector3 Position { 
        get 
        {
            return transform.position;
        }
        set 
        {
            Vector3 newPosition = new Vector3(value.x, value.y, -1);
            transform.position = newPosition;
            ItemPositionChanged?.Invoke(this, transform.position);
        } 
    }
    public Vector3 InitalPosition { get; private set; }

    public bool isActive { get; set; }

    public void Init(IPlayer player)
    {
        Id = player.Id;
        Name = player.Name;
        Type = player.Type;
        InitalPosition = player.InitalPosition;
        Position = InitalPosition;
        isActive = player.isActive;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputHandler playerHandler = new PlayerInputHandler(this);
        inputhandler = playerHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCanMove && (inputhandler != null) && inputhandler.KeyCheck())
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
        playerCanMove = isActive && newGameState == GameState.PlayerMoving;
    }

    public void Shift(Vector3 direction)
    {

    }

    public void Rotate(int angle)
    {
        ItemRotated?.Invoke(this, angle);
    }

    public void onBaseItemShifted(IFieldItem sender, Vector3 direction)
    {
        Shift(direction);
    }

    public void onBaseItemRotated(IFieldItem sender, int angle)
    {
        Rotate(angle);
    }

    public void onBaseItemPositionChanged(IFieldItem sender, Vector3 newPosition)
    {
        Position = newPosition;
    }

    public void SetBaseItem(IFieldItem baseItem)
    {
        if(this.baseItem != null)
        {
            this.baseItem.ItemShifted -= onBaseItemShifted;
            this.baseItem.ItemRotated -= onBaseItemRotated;
            this.baseItem.ItemPositionChanged -= onBaseItemPositionChanged;
        }
        if(baseItem != null)
        {
            this.baseItem = baseItem;
            this.baseItem.ItemShifted += onBaseItemShifted;
            this.baseItem.ItemRotated += onBaseItemRotated;
            this.baseItem.ItemPositionChanged += onBaseItemPositionChanged;
            Position = this.baseItem.Position;
        }
    }
}

public class PlayerInputHandler : InputHandler
{
    private IFieldItem player;

    public event PlayerMoving onPlayerTryMoving;

    public PlayerInputHandler(IFieldItem player)
    {
        this.player = player;
    }

    public bool KeyCheck()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Завершение движения
            return true;
        }
        return false;
    }

    public void onObjectSelected(GameObject block)
    { 
        // TODO перемещение к позиции
    }
}