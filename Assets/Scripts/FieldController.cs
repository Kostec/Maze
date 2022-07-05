using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FieldController : MonoBehaviour
{
    [SerializeField]
    public uint heigth = 7;
    [SerializeField]
    public uint width = 7;
    [SerializeField]
    public ShapeMode shapeMode = ShapeMode.Square;
    [SerializeField]
    public GameObject blockPrefab;

    private GameObject bufferBlock;
    private Dictionary<Vector3, GameObject> gameArray;
    private Vector3 bufferPosition = new Vector3(-1, -1, 0);
    private InputHandler inputHandler;

    private BlockRotationHandler blockRotationHandler;
    private FieldShiftHandler fieldShiftHandler;
    public EventHandler finishCurrentState;
    // Start is called before the first frame update
    void Start()
    {
        gameArray = new Dictionary<Vector3, GameObject>();
        GenerateBlocks();
        fieldShiftHandler = new FieldShiftHandler(gameArray, bufferPosition, bufferBlock);
        blockRotationHandler = new BlockRotationHandler(bufferBlock);
        inputHandler = fieldShiftHandler;
    }

    private void onBlockClicked(GameObject blockClicked)
    {
        blockRotationHandler.onObjectSelected(bufferBlock);
        fieldShiftHandler.onObjectSelected(blockClicked);
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

    public Block GetBlock(Vector3 position)
    {
        return gameArray.ContainsKey(position) ? gameArray[position].GetComponent<Block>() : null;
    }
    private GameObject CreateBlock(Vector3 position)
    {
        GameObject currentBlock = Instantiate(blockPrefab, position, Quaternion.identity);
        Block obj = currentBlock.GetComponent<Block>();
        obj.Rotate(90 * UnityEngine.Random.Range(0, 3));
        obj.onBlockClicked += onBlockClicked;
        var type = (BlockType)UnityEngine.Random.Range(0, 3);
        obj.SetType(type);
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
}

public class FieldShiftHandler : InputHandler
{
    public GameObject BufferBlock { get; set; }
    private Dictionary<Vector3, GameObject> GameArray;
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

        if (Input.GetKeyUp(KeyCode.W))
        {
            // Move up
            var toMove = GameArray.Where(pair => pair.Key.x == SelectedBlock.Key.x).OrderBy(pair => pair.Key.y).Reverse();
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector3(0, 1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, GameObject>();
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            // Move down
            var toMove = GameArray.Where(pair => pair.Key.x == SelectedBlock.Key.x).OrderBy(pair => pair.Key.y);
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector3(0, -1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, GameObject>();
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            // Move left
            var toMove = GameArray.Where(pair => pair.Key.y == SelectedBlock.Key.y).OrderBy(pair => pair.Key.x);
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector3(-1, 0));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, GameObject>();
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            // Move right
            var toMove = GameArray.Where(pair => pair.Key.y == SelectedBlock.Key.y).OrderBy(pair => pair.Key.x).Reverse();
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector3(1, 0));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, GameObject>();
            return true;
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
            MoveBlocks(toMove, new Vector3(-1, 1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, GameObject>();
            return true;
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
            MoveBlocks(toMove, new Vector3(1, -1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, GameObject>();
            return true;
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
            MoveBlocks(toMove, new Vector3(1, 1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, GameObject>();
            return true;
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
            MoveBlocks(toMove, new Vector3(-1, -1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector3, GameObject>();
            return true;
        }
        return false;
    }

    private void MoveBlocks(IEnumerable<KeyValuePair<Vector3, GameObject>> toMove, Vector3 offset)
    {
        foreach (var movedBlock in toMove)
        {
            Vector3 newPosition = movedBlock.Key + offset;
            movedBlock.Value.transform.position = newPosition;
            if (GameArray.ContainsKey(newPosition))
            {
                GameArray[newPosition] = movedBlock.Value;
            }
        }
    }

    private void SwapBufferBlock(GameObject toBuffer, Vector3 newBufferLocation)
    {
        // Move and update buffer
        BufferBlock.transform.position = newBufferLocation;
        GameArray[newBufferLocation] = BufferBlock;
        BufferBlock = toBuffer;
        BufferBlock.transform.position = bufferPosition;
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

    public BlockRotationHandler(GameObject selectedBlock)
    {
        SelectedBlock = new KeyValuePair<Vector3, GameObject>(new Vector3(), selectedBlock);
    }

    public bool KeyCheck()
    {
        if (Input.GetKeyUp(KeyCode.D))
        {
            SelectedBlock.Value.transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f), -90);
            return false;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            SelectedBlock.Value.transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f), 90);
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
