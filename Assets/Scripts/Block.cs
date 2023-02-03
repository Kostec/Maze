using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.Servers.Items;

public delegate void ClickBlockHandler(GameObject obj);

public class Block : MonoBehaviour, IBlock
{
    private SpriteRenderer spriteRenderer;
    public event ClickBlockHandler onBlockClicked;
    public event FieldItemShifted ItemShifted;
    public event FieldItemRotated ItemRotated;
    public event FieldPositionChanged ItemPositionChanged;
    public int Angle { private set; get; }

    public BlockType type { get; private set; } = BlockType.Straight;
    public ShapeMode shapeMode { get; private set; } = ShapeMode.Square;

    [SerializeField]
    public Sprite[] sprites;
    public List<Vector3> PossibleDirections { get; set; } = new List<Vector3>();
    public bool FixedPoint { get; set; } = false;

    private List<GameObject> items = new List<GameObject>();

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = new Vector3(value.x, value.y, 0);
            ItemPositionChanged?.Invoke(this, Position);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(IBlock block)
    {
        Position = block.Position;
        type = block.type;
        shapeMode = block.shapeMode;
        spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spriteRenderer.sprite = sprites[(int)type];
        foreach (Vector3 dir in block.PossibleDirections)
        {
            PossibleDirections.Add(dir);
        }
        Rotate(block.Angle);
    }

    public void Rotate(int angle)
    {
        Angle += angle;
        // Поворот блока на сцене и всего что на нём находится
        transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f), angle);
        foreach (var item in items)
        {
            item.transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f), angle);
        }
        ItemRotated?.Invoke(this, angle);
    }

    private void OnMouseDown()
    {
        Debug.Log($"Block type: {this.type}");
        Debug.Log($"Block directions: {this.PossibleDirections}");

        onBlockClicked(gameObject);
    }

    public void Shift(Vector3 direction)
    {
        Position += direction;
        ItemShifted?.Invoke(this, direction);
    }

    public void onBaseItemShifted(IFieldItem sender, Vector3 direction)
    {
        
    }

    public void onBaseItemRotated(IFieldItem sender, int angle)
    {
        
    }

    public void onBaseItemPositionChanged(IFieldItem sender, Vector3 newPosition)
    {
        
    }

    public void SetBaseItem(IFieldItem baseItem)
    {
        
    }
}
