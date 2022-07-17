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
            transform.position = value;
            ItemPositionChanged?.Invoke(this, Position);
            foreach (var item in items)
            {
                item.transform.position += new Vector3(Position.x, Position.y);
            }
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
        PossibleDirections = block.PossibleDirections;
    }

    public void Rotate(int angle)
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f), angle);
        foreach(var item in items)
        {
            item.transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f), angle);
        }

        for(int i = 0; i < PossibleDirections.Count; i++)
        {
            var currentDirection = PossibleDirections[i];
            float cs = (float)Math.Cos(angle * Math.PI / 180);
            float sn = (float)Math.Sin(angle * Math.PI / 180);
            float oldX = currentDirection.x;
            float oldY = currentDirection.y;
            currentDirection.x = (int)Math.Round(oldX * cs - oldY * sn);
            currentDirection.y = (int)Math.Round(oldX * sn + oldY * cs);
            PossibleDirections[i] = currentDirection;
        }
        ItemRotated?.Invoke(this, angle);
    }

    private void OnMouseDown()
    {
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
