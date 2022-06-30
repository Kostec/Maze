using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BlockController : MonoBehaviour
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
    private Dictionary<Vector2, GameObject> gameArray;
    private Vector2 bufferPosition = new Vector2(-1, -1);
    private InputHandler inputHandler;

    private BlockRotationHandler blockRotationHandler;
    private FieldShiftHandler fieldShiftHandler;
    public EventHandler finishCurrentState;
    // Start is called before the first frame update
    void Start()
    {
        gameArray = new Dictionary<Vector2, GameObject>();
        GenerateBlocks();
        fieldShiftHandler = new FieldShiftHandler(gameArray, bufferPosition, bufferBlock);
        blockRotationHandler = new BlockRotationHandler(gameArray, bufferBlock);
        inputHandler = fieldShiftHandler;
    }

    private void onBlockClicked(GameObject blockClicked)
    {
        blockRotationHandler.onBlockClicked(bufferBlock);
        fieldShiftHandler.onBlockClicked(blockClicked);
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
                Vector2 position = new Vector2(x, y);
                GameObject currentBlock = Instantiate(blockPrefab, position, Quaternion.identity);
                Block obj = currentBlock.GetComponent<Block>();
                obj.onBlockClicked += onBlockClicked;
                gameArray.Add(position, currentBlock);
            }
        }

        bufferBlock = Instantiate(blockPrefab, bufferPosition, Quaternion.identity);
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
                break;
            case GameState.PlayerMoving:
                inputHandler = null;
                break;
            default:
                inputHandler = null;
                break;
        }
    }
}
public class FieldShiftHandler : InputHandler
{
    public GameObject BufferBlock;
    public Dictionary<Vector2, GameObject> GameArray { get; set; }
    public Vector2 BufferPosition { get; set; }
    public KeyValuePair<Vector2, GameObject> SelectedBlock { get; set; }
    public FieldShiftHandler(Dictionary<Vector2, GameObject> gameArray, Vector2 bufferPosition, GameObject bufferBlock)
    {
        GameArray = gameArray;
        BufferPosition = bufferPosition;
        BufferBlock = bufferBlock;
        SelectedBlock = new KeyValuePair<Vector2, GameObject>();
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
            MoveBlocks(toMove, new Vector2(0, 1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector2, GameObject>();
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            // Move down
            var toMove = GameArray.Where(pair => pair.Key.x == SelectedBlock.Key.x).OrderBy(pair => pair.Key.y);
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector2(0, -1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector2, GameObject>();
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            // Move left
            var toMove = GameArray.Where(pair => pair.Key.y == SelectedBlock.Key.y).OrderBy(pair => pair.Key.x);
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector2(-1, 0));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector2, GameObject>();
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            // Move right
            var toMove = GameArray.Where(pair => pair.Key.y == SelectedBlock.Key.y).OrderBy(pair => pair.Key.x).Reverse();
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector2(1, 0));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector2, GameObject>();
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
            MoveBlocks(toMove, new Vector2(-1, 1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector2, GameObject>();
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
            MoveBlocks(toMove, new Vector2(1, -1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector2, GameObject>();
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
            MoveBlocks(toMove, new Vector2(1, 1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector2, GameObject>();
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
            MoveBlocks(toMove, new Vector2(-1, -1));
            SwapBufferBlock(first.Value, last.Key);
            SelectedBlock = new KeyValuePair<Vector2, GameObject>();
            return true;
        }
        return false;
    }

    private void MoveBlocks(IEnumerable<KeyValuePair<Vector2, GameObject>> toMove, Vector2 offset)
    {
        foreach (var movedBlock in toMove)
        {
            Vector2 newPosition = movedBlock.Key + offset;
            movedBlock.Value.transform.position = newPosition;
            if (GameArray.ContainsKey(newPosition))
            {
                GameArray[newPosition] = movedBlock.Value;
            }
        }
    }

    private void SwapBufferBlock(GameObject toBuffer, Vector2 newBufferLocation)
    {
        // Move and update buffer
        BufferBlock.transform.position = newBufferLocation;
        GameArray[newBufferLocation] = BufferBlock;
        BufferBlock = toBuffer;
        BufferBlock.transform.position = BufferPosition;
        // Reset selected block
        SelectedBlock = new KeyValuePair<Vector2, GameObject>();
    }

    private IEnumerable<KeyValuePair<Vector2, GameObject>> GetDiagonale(bool up, bool right, out KeyValuePair<Vector2, GameObject> first, out KeyValuePair<Vector2, GameObject> last)
    {
        Dictionary<Vector2, GameObject> result = new Dictionary<Vector2, GameObject>();

        result.Add(SelectedBlock.Key, SelectedBlock.Value);
        first = SelectedBlock;
        last = SelectedBlock;

        for (int i = 1; true; i++)
        {
            int xOffset = right ? 1 : -1;
            int yOffset = up ? 1 : -1;
            Vector2 position1 = new Vector2(xOffset * i, yOffset * i);
            Vector2 position2 = new Vector2(-xOffset * i, -yOffset * i);
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

    public void onBlockClicked(GameObject block)
    {
        var foundedBlock = GameArray.FirstOrDefault((pair) => pair.Value == block);
        SelectedBlock = new KeyValuePair<Vector2, GameObject>(foundedBlock.Key, foundedBlock.Value);
    }
}
public class BlockRotationHandler : InputHandler
{
    public Dictionary<Vector2, GameObject> GameArray { get; set; }
    public Vector2 BufferPosition { get; set; }
    public KeyValuePair<Vector2, GameObject> SelectedBlock { get; set; }

    public BlockRotationHandler(Dictionary<Vector2, GameObject> gameArray, GameObject selectedBlock)
    {
        SelectedBlock = new KeyValuePair<Vector2, GameObject>(new Vector2(), selectedBlock);
        GameArray = gameArray;
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

    public void onBlockClicked(GameObject block)
    {
        SelectedBlock = new KeyValuePair<Vector2, GameObject>(new Vector2(), block);
    }
}
