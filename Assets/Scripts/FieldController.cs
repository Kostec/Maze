using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public delegate void onLineShifted(IEnumerable<Vector3> line, Vector3 direction);

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
    public event onLineShifted OnLineShiftedEvent;
    // Start is called before the first frame update
    void Start()
    {
        gameArray = new Dictionary<Vector3, GameObject>();
        GenerateBlocks();
        fieldShiftHandler = new FieldShiftHandler(gameArray, bufferPosition, bufferBlock);
        blockRotationHandler = new BlockRotationHandler(bufferBlock, shapeMode);
        inputHandler = fieldShiftHandler;
        fieldShiftHandler.OnLineShifted += OnLineShifted;
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
        Dictionary<Vector3, Block> dict = new Dictionary<Vector3, Block>();
        foreach (var pair in gameArray)
        {
            dict.Add(pair.Key, pair.Value.GetComponent<Block>());
        }
        return dict;
    }
   
    private void onBlockClicked(GameObject blockClicked)
    {
        blockRotationHandler.onObjectSelected(bufferBlock);
        fieldShiftHandler.onObjectSelected(blockClicked);
        onBlockClickedEvent?.Invoke(blockClicked);
    }
    public Block GetBlock(Vector3 position)
    {
        position.z = 0;
        return gameArray.ContainsKey(position) ? gameArray[position].GetComponent<Block>() : null;
    }
    private GameObject CreateBlock(Vector3 position)
    {
        GameObject currentBlock = Instantiate(blockPrefab, position, Quaternion.identity);
        Block obj = currentBlock.GetComponent<Block>();
        obj.FixedPoint = FixedPoints.Contains(position);

        BlockType blockType = BlockType.Crossroad;
        int angle = 0;
        switch (shapeMode)
        {
            case ShapeMode.Triangle:
                break;
            case ShapeMode.Square:
                blockType = (BlockType)UnityEngine.Random.Range(0, 4); ;
                angle = Block.RotateAngle[shapeMode] * UnityEngine.Random.Range(0, 3);
                break;
            case ShapeMode.Hexa:
                blockType = (BlockType)UnityEngine.Random.Range(4, 10);
                angle = Block.RotateAngle[shapeMode] * UnityEngine.Random.Range(0, 6);
                break;
            case ShapeMode.Octo:
                break;
        }
        obj.SetType(blockType, shapeMode);
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < heigth; y++)
            {
                Vector3 position = new Vector3(x, y, 0);
                GameObject currentBlock = CreateBlock(position);
                gameArray.Add(position, currentBlock);
            }
        }

        bufferBlock = CreateBlock(bufferPosition);
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
    private void OnLineShifted(IEnumerable<Vector3> line, Vector3 offset)
    {
        OnLineShiftedEvent?.Invoke(line, offset);
    }
}

public class FieldShiftHandler : InputHandler
{
    public GameObject BufferBlock { get; set; }
    private Dictionary<Vector3, GameObject> GameArray;
    private Vector3 bufferPosition;
    private KeyValuePair<Vector3, GameObject> SelectedBlock;
    public event onLineShifted OnLineShifted;
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

        if (Input.GetKeyUp(KeyCode.W))
        {
            // Move up
            var toMove = GameArray.Where(pair => pair.Key.x == SelectedBlock.Key.x).OrderBy(pair => pair.Key.y).Reverse();
            return Shift(toMove, Vector3.up);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            // Move down
            var toMove = GameArray.Where(pair => pair.Key.x == SelectedBlock.Key.x).OrderBy(pair => pair.Key.y);
            return Shift(toMove, Vector3.down);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            // Move left
            var toMove = GameArray.Where(pair => pair.Key.y == SelectedBlock.Key.y).OrderBy(pair => pair.Key.x);
            return Shift(toMove, Vector3.left);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            // Move right
            var toMove = GameArray.Where(pair => pair.Key.y == SelectedBlock.Key.y).OrderBy(pair => pair.Key.x).Reverse();
            return Shift(toMove, Vector3.right);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            // Move diagonal up
            // \
            //  \
            //   \
            var first = SelectedBlock;
            var last = SelectedBlock;
            var toMove = GetDiagonale(true, false, out first, out last);
            return Shift(toMove, Vector3.up + Vector3.left, first, last);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            // Move diagonal down
            // \
            //  \
            //   \
            var first = SelectedBlock;
            var last = SelectedBlock;
            var toMove = GetDiagonale(false, true, out first, out last);
            return Shift(toMove, Vector3.down + Vector3.right, first, last);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            // Move diagonal up
            //   /
            //  /
            // /
            var first = SelectedBlock;
            var last = SelectedBlock;
            var toMove = GetDiagonale(true, true, out first, out last);
            return Shift(toMove, Vector3.up + Vector3.right, first, last);
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            // Move diagonal down
            //   /
            //  /
            // /
            var first = SelectedBlock;
            var last = SelectedBlock;
            var toMove = GetDiagonale(false, false, out first, out last);
            return Shift(toMove, Vector3.down + Vector3.left, first, last);
        }
        return false;
    }

    private bool Shift(IEnumerable<KeyValuePair<Vector3, GameObject>> toMove, Vector3 offset, KeyValuePair<Vector3, GameObject> first = default, KeyValuePair<Vector3, GameObject> last = default)
    {
        if (toMove.FirstOrDefault(obj => obj.Value.GetComponent<Block>().FixedPoint).Value != null)
        {
            Debug.Log("Line have fixed block");
            return false;
        }
        first = first.Value == default ? toMove.First() : first;
        last = last.Value == default ? toMove.Last() : last; // Element which should be pop from field
        MoveBlocks(toMove, offset);
        SwapBufferBlock(first.Value, last.Key);
        SelectedBlock = new KeyValuePair<Vector3, GameObject>();
        List<Vector3> shiftedLine = toMove.Select(pair => pair.Key).ToList();
        OnLineShifted?.Invoke(shiftedLine, offset);
        return true;
    }

    private void MoveBlocks(IEnumerable<KeyValuePair<Vector3, GameObject>> toMove, Vector3 offset)
    {
        foreach (var movedBlock in toMove)
        {
            movedBlock.Value.GetComponent<Block>().Shift(offset);
            Vector3 newPosition = movedBlock.Value.GetComponent<Block>().Position;
            if (GameArray.ContainsKey(newPosition))
            {
                GameArray[newPosition] = movedBlock.Value;
            }
        }
    }

    private void SwapBufferBlock(GameObject toBuffer, Vector3 newBufferLocation)
    {
        // Move and update buffer
        BufferBlock.GetComponent<Block>().Position = newBufferLocation;
        GameArray[newBufferLocation] = BufferBlock;
        BufferBlock = toBuffer;
        BufferBlock.GetComponent<Block>().Position = bufferPosition;
        // Reset selected block
        SelectedBlock = new KeyValuePair<Vector3, GameObject>();
    }

    private IEnumerable<KeyValuePair<Vector3, GameObject>> GetDiagonale(bool up, bool right, out KeyValuePair<Vector3, GameObject> first, out KeyValuePair<Vector3, GameObject> last)
    {
        Dictionary<Vector3, GameObject> result = new Dictionary<Vector3, GameObject>();

        result.Add(SelectedBlock.Key, SelectedBlock.Value);
        first = SelectedBlock;
        last = SelectedBlock;

        for (int i = 1; true; i++)
        {
            int xOffset = right ? 1 : -1;
            int yOffset = up ? 1 : -1;
            Vector3 position1 = new Vector3(xOffset * i, yOffset * i);
            Vector3 position2 = new Vector3(-xOffset * i, -yOffset * i);
            if (!GameArray.ContainsKey(SelectedBlock.Key + position2) && !GameArray.ContainsKey(SelectedBlock.Key + position1))
            {
                break;
            }
            if (GameArray.ContainsKey(SelectedBlock.Key + position1))
            {
                first = GameArray.FirstOrDefault(item => item.Key == (SelectedBlock.Key + position1));
                result.Add(first.Key, first.Value);
            }
            if (GameArray.ContainsKey(SelectedBlock.Key + position2))
            {
                last = GameArray.FirstOrDefault(item => item.Key == (SelectedBlock.Key + position2));
                result.Add(last.Key, last.Value);
            }
        }
        return result;
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
    private ShapeMode ShapeMode { get; set; }

    public BlockRotationHandler(GameObject selectedBlock, ShapeMode shapeMode)
    {
        SelectedBlock = new KeyValuePair<Vector3, GameObject>(new Vector3(), selectedBlock);
        ShapeMode = shapeMode;
    }

    public bool KeyCheck()
    {
        
        if (Input.GetKeyUp(KeyCode.D))
        {
            Block _block = SelectedBlock.Value.GetComponent<Block>();
            _block.Rotate(-Block.RotateAngle[ShapeMode]);
            return false;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            Block _block = SelectedBlock.Value.GetComponent<Block>();
            _block.Rotate(Block.RotateAngle[ShapeMode]);
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

