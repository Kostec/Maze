using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.Scripts.Servers.Items;
using Assets.Scripts.Servers.Interfaces;

public class FieldController : MonoBehaviour
{
    [SerializeField]
    public uint heigth = 7;
    [SerializeField]
    public uint width = 7;
    [SerializeField]
    public ShapeMode shapeMode = ShapeMode.Hexa;
    [SerializeField]
    public GameObject blockPrefab;

    private GameObject bufferBlock;
    private Dictionary<Vector3, GameObject> gameArray;
    private Vector3 bufferPosition = new Vector3(-1, -1, 0);
    private InputHandler inputHandler;
    private List<Vector3> FixedPoints = new List<Vector3>() {
        new Vector3(1,1),
        new Vector3(1,5),
        new Vector3(5,1),
        new Vector3(5,5),
    };

    private BlockRotationHandler blockRotationHandler;
    private FieldShiftHandler fieldShiftHandler;
    public EventHandler finishCurrentState;
    public event ClickBlockHandler onBlockClickedEvent;
    // Start is called before the first frame update
    void Start()
    {
        gameArray = new Dictionary<Vector3, GameObject>();
        GenerateBlocks();
        fieldShiftHandler = new FieldShiftHandler(gameArray, bufferPosition, bufferBlock);
        blockRotationHandler = new BlockRotationHandler(bufferBlock);
        inputHandler = fieldShiftHandler;
    }
    // Update is called once per frame
    void Update()
    {
        if (inputHandler != null)
        {
            if (inputHandler.KeyCheck())
            {
                if (finishCurrentState != null)
                {
                    finishCurrentState(this, new EventArgs());
                }
            }
        }
    }
    public Dictionary<Vector3, Block> GetFieldArray()
    {
        Dictionary<Vector3, Block> dict = gameArray.ToDictionary(p => p.Key, p => p.Value.GetComponent<Block>());
        return dict;
    }
   
    private void onBlockClicked(GameObject blockClicked)
    {
        if (inputHandler != blockRotationHandler)
        {
            blockRotationHandler.onObjectSelected(bufferBlock);
            fieldShiftHandler.onObjectSelected(blockClicked);
            onBlockClickedEvent?.Invoke(blockClicked);
        }
    }
    public Block GetBlock(Vector3 position)
    {
        position.z = 0;
        return gameArray.ContainsKey(position) ? gameArray[position].GetComponent<Block>() : null;
    }
    private GameObject CreateBlock(IBlock block)
    {
        GameObject currentBlock = Instantiate(blockPrefab, block.Position, Quaternion.identity);
        Block obj = currentBlock.GetComponent<Block>();

        int angle = 0;
        obj.Init(block);
        obj.Rotate(angle);
        obj.onBlockClicked += onBlockClicked;
        return currentBlock;
    }
    private void GenerateBlocks()
    {
        if (gameArray.Count > 0)
        {
            foreach(var item in gameArray)
            {
                Destroy(item.Value);
            }
            gameArray.Clear();
        }
        var field = GameController.serverWrapper.GenerateField(width, heigth, shapeMode);
        foreach(var item in field)
        {
            Vector3 position = item.Key;
            if (position == bufferPosition)
            {
                bufferBlock = CreateBlock(item.Value);
                continue;
            }
            GameObject currentBlock = CreateBlock(item.Value);
            gameArray.Add(position, currentBlock);
        }
    }
    public void onGameStateChanged(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.FieldShifting:
                inputHandler = fieldShiftHandler;
                break;
            case GameState.BlockRotate:
                inputHandler = blockRotationHandler;
                bufferBlock = fieldShiftHandler.BufferBlock;
                break;
            default:
                inputHandler = null;
                break;
        }
    }
    public InputHandler getInputHandler()
    {
        return inputHandler;
    }
}

public class FieldShiftHandler : InputHandler
{
    public GameObject BufferBlock { get; set; }
    public Dictionary<Vector3, GameObject> GameArray;
    private Vector3 bufferPosition;
    private KeyValuePair<Vector3, GameObject> SelectedBlock;
    public FieldShiftHandler(Dictionary<Vector3, GameObject> gameArray, Vector3 bufferPosition, GameObject bufferBlock)
    {
        GameArray = gameArray;
        this.bufferPosition = bufferPosition;
        BufferBlock = bufferBlock;
        SelectedBlock = new KeyValuePair<Vector3, GameObject>();
    }

    public bool KeyCheck()
    {
        if (SelectedBlock.Value == null)
        {
            return false;
        }

        Vector3 direction = Vector3.zero;

        if (Input.GetKeyUp(KeyCode.W))
        {
            direction = Vector3.up;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            direction = Vector3.down;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            direction = Vector3.left;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            direction = Vector3.right;
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            direction = Vector3.up + Vector3.left;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            direction = Vector3.down + Vector3.right;
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            direction = Vector3.up + Vector3.right;
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            direction = Vector3.down + Vector3.left;
        }
        if (direction != Vector3.zero)
        {
            var resp = GameController.serverWrapper.ShiftLine(SelectedBlock.Value.GetComponent<Block>(), direction);
            if (resp.success)
            {
                var newBufferPosition = resp.bufferTo;
                var popBlockPosition = resp.toBuffer;
                Vector3 position = popBlockPosition;
                Block popBlock = GameArray[resp.toBuffer].GetComponent<Block>();
                while (GameArray.ContainsKey(position))
                {
                    GameArray[position].GetComponent<Block>().Shift(direction);
                    // ������� ������� ����� ��������� � �����
                    if(!GameArray.ContainsKey(position + direction))
                    {
                        GameArray[position] = null;
                    }
                    else if (GameArray.ContainsKey(position))
                    {
                        GameArray[position + direction] = GameArray[position];
                    }
                    position -= direction;
                }
                GameArray[resp.bufferTo] = BufferBlock;
                popBlock.Position = bufferPosition;
                BufferBlock.GetComponent<Block>().Position = newBufferPosition;
                BufferBlock = popBlock.gameObject;
                return true;
            }
        }
        return false;
    }
    public void onObjectSelected(GameObject block)
    {
        var foundedBlock = GameArray.FirstOrDefault((pair) => pair.Value == block);
        SelectedBlock = new KeyValuePair<Vector3, GameObject>(foundedBlock.Key, foundedBlock.Value);
    }
}
public class BlockRotationHandler : InputHandler
{
    public KeyValuePair<Vector3, GameObject> SelectedBlock { get; set; }

    public BlockRotationHandler(GameObject selectedBlock)
    {
        SelectedBlock = new KeyValuePair<Vector3, GameObject>(new Vector3(), selectedBlock);
    }

    public bool KeyCheck()
    {
        
        if (Input.GetKeyUp(KeyCode.D))
        {
            Block _block = SelectedBlock.Value.GetComponent<Block>();
            var res = GameController.serverWrapper.RotateBlock(_block, RotateSide.Right);
            if (res.success)
            {
                _block.Rotate(res.angle);
            }
            return false;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            Block _block = SelectedBlock.Value.GetComponent<Block>();
            var res = GameController.serverWrapper.RotateBlock(_block, RotateSide.Left);
            if (res.success)
            {
                _block.Rotate(res.angle);
            }
            return false;
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            return true;
        }

        return false;
    }

    public void onObjectSelected(GameObject block)
    {
        SelectedBlock = new KeyValuePair<Vector3, GameObject>(new Vector3(), block);
    }
}

