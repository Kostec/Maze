using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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

    private Dictionary<Vector2, GameObject> gameArray;
    private GameObject bufferBlock;
    private Vector2 bufferPosition = new Vector2(-1, -1);
    private KeyValuePair<Vector2, GameObject> selectedBlock;

    // Start is called before the first frame update
    void Start()
    {
        gameArray = new Dictionary<Vector2, GameObject>();
        GenerateBlocks();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedBlock.Value != null)
        {
            KeyCheck();
        }
    }

    private void MoveBlocks(IEnumerable<KeyValuePair<Vector2, GameObject>> toMove, Vector2 offset)
    {
        foreach (var movedBlock in toMove)
        {
            Vector2 newPosition = movedBlock.Key + offset;
            movedBlock.Value.transform.position = newPosition;
            if (gameArray.ContainsKey(newPosition))
            {
                gameArray[newPosition] = movedBlock.Value;
            }
        }
    }

    private void SwapBufferBlock(GameObject toBuffer, Vector2 newBufferLocation)
    {
        // Move and update buffer
        bufferBlock.transform.position = newBufferLocation;
        gameArray[newBufferLocation] = bufferBlock;
        bufferBlock = toBuffer;
        bufferBlock.transform.position = bufferPosition;
        // Reset selected block
        selectedBlock = new KeyValuePair<Vector2, GameObject>();
    }

    private IEnumerable<KeyValuePair<Vector2, GameObject>> GetDiagonale(bool up, bool right, out KeyValuePair<Vector2, GameObject> first, out KeyValuePair<Vector2, GameObject> last)
    {
        Dictionary<Vector2, GameObject> result = new Dictionary<Vector2, GameObject>();

        result.Add(selectedBlock.Key, selectedBlock.Value);
        first = selectedBlock;
        last = selectedBlock;
        for (int i = 1; true; i++)
        {
            int xOffset = right ? 1 : -1;
            int yOffset = up ? 1 : -1;
            Vector2 position1 = new Vector2(xOffset * i, yOffset * i);
            Vector2 position2 = new Vector2(-xOffset * i, -yOffset * i);
            if (!gameArray.ContainsKey(selectedBlock.Key + position2) && !gameArray.ContainsKey(selectedBlock.Key + position1))
            {
                break;
            }
            if (gameArray.ContainsKey(selectedBlock.Key + position1))
            {
                first = gameArray.FirstOrDefault(item => item.Key == (selectedBlock.Key + position1));
                result.Add(first.Key, first.Value);
            }
            if (gameArray.ContainsKey(selectedBlock.Key + position2))
            {
                last = gameArray.FirstOrDefault(item => item.Key == (selectedBlock.Key + position2));
                result.Add(last.Key, last.Value);
            }
        }
        return result;
    }

    private void KeyCheck()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            // Move up
            var toMove = gameArray.Where(pair => pair.Key.x == selectedBlock.Key.x).OrderBy(pair => pair.Key.y).Reverse();
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector2(0, 1));
            SwapBufferBlock(first.Value, last.Key);
            selectedBlock = new KeyValuePair<Vector2, GameObject>();
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            // Move down
            var toMove = gameArray.Where(pair => pair.Key.x == selectedBlock.Key.x).OrderBy(pair => pair.Key.y);
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector2(0, -1));
            SwapBufferBlock(first.Value, last.Key);
            selectedBlock = new KeyValuePair<Vector2, GameObject>();
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            // Move left
            var toMove = gameArray.Where(pair => pair.Key.y == selectedBlock.Key.y).OrderBy(pair => pair.Key.x);
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector2(-1, 0));
            SwapBufferBlock(first.Value, last.Key);
            selectedBlock = new KeyValuePair<Vector2, GameObject>();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            // Move right
            var toMove = gameArray.Where(pair => pair.Key.y == selectedBlock.Key.y).OrderBy(pair => pair.Key.x).Reverse();
            var first = toMove.First();
            var last = toMove.Last();
            MoveBlocks(toMove, new Vector2(1, 0));
            SwapBufferBlock(first.Value, last.Key);
            selectedBlock = new KeyValuePair<Vector2, GameObject>();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            // Move diagonal up
            // \
            //  \
            //   \
            var first = selectedBlock;
            var last = selectedBlock;
            var toMove = GetDiagonale(true, false, out first, out last);
            MoveBlocks(toMove, new Vector2(-1, 1));
            SwapBufferBlock(first.Value, last.Key);
            selectedBlock = new KeyValuePair<Vector2, GameObject>();
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            // Move diagonal down
            // \
            //  \
            //   \
            var first = selectedBlock;
            var last = selectedBlock;
            var toMove = GetDiagonale(false, true, out first, out last);
            MoveBlocks(toMove, new Vector2(1, -1));
            SwapBufferBlock(first.Value, last.Key);
            selectedBlock = new KeyValuePair<Vector2, GameObject>();
        }

        else if (Input.GetKeyUp(KeyCode.E))
        {
            // Move diagonal up
            //   /
            //  /
            // /
            var first = selectedBlock;
            var last = selectedBlock;
            var toMove = GetDiagonale(true, true, out first, out last);
            MoveBlocks(toMove, new Vector2(1, 1));
            SwapBufferBlock(first.Value, last.Key);
            selectedBlock = new KeyValuePair<Vector2, GameObject>();
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            // Move diagonal down
            //   /
            //  /
            // /
            var first = selectedBlock;
            var last = selectedBlock;
            var toMove = GetDiagonale(false, false, out first, out last);
            MoveBlocks(toMove, new Vector2(-1, -1));
            SwapBufferBlock(first.Value, last.Key);
            selectedBlock = new KeyValuePair<Vector2, GameObject>();
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

    private void onBlockClicked(GameObject block)
    {
        Debug.Log($"{block.name}: {block.transform.position}");
        var foundedBlock = gameArray.FirstOrDefault((pair) => pair.Value == block);
        selectedBlock = new KeyValuePair<Vector2, GameObject>(foundedBlock.Key, foundedBlock.Value);
    }
}
