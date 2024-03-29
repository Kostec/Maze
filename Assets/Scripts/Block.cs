using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine;

public enum BlockType
{
    // Square
    Straight, // �����
    Turn, // �������
    Crossroad_T, // �-�������� ����������
    Crossroad, // ����������
    // Hexagon
    Hex_cross,
    Hex_J_road,
    Hex_straight,
    Hex_W_road,
    Hex_X_road,
    Hex_Y_road,
}

public enum HexType
{
    Hex_cross,
    Hex_J_road,
    Hex_straight,
    Hex_W_road,
    Hex_X_road,
    Hex_Y_road,
}

public enum ShapeMode
{
    Triangle,
    Square,
    Hexa,
    Octo
};

public delegate void ClickBlockHandler(GameObject obj);

public class Block : MonoBehaviour
{
    public static Dictionary<ShapeMode, int> RotateAngle = new Dictionary<ShapeMode, int>()
    {
        {ShapeMode.Triangle, 60 },
        {ShapeMode.Square, 90 },
        {ShapeMode.Hexa, 60 },
        {ShapeMode.Octo, 45 },
    };

    private SpriteRenderer spriteRenderer;
    public event ClickBlockHandler onBlockClicked;

    private BlockType type = BlockType.Straight;
    private ShapeMode shapeMode = ShapeMode.Square;

    [SerializeField]
    public Sprite[] sprites;
    public List<Vector3> PossibleDirections;
    public bool FixedPoint = false;

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

    public Vector3 Shift(Vector3 direction)
    {
        Position += direction;
        return transform.position;
    }

    public void AttachItem(GameObject item)
    {
        if(!items.Contains(item))
        {
            items.Add(item);
        }
    }

    public void DetachItem(GameObject item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    private void HexagonDirections()
    {
        switch (type)
        {
            case BlockType.Hex_cross:
                PossibleDirections.Add(Vector3.left);
                PossibleDirections.Add(Vector3.right);
                PossibleDirections.Add(Vector3.up + Vector3.left);
                PossibleDirections.Add(Vector3.down + Vector3.left);
                PossibleDirections.Add(Vector3.up + Vector3.right);
                PossibleDirections.Add(Vector3.down + Vector3.right);
                break;
            case BlockType.Hex_J_road:
                PossibleDirections.Add(Vector3.right);
                PossibleDirections.Add(Vector3.down + Vector3.right);
                break;
            case BlockType.Hex_straight:
                PossibleDirections.Add(Vector3.left);
                PossibleDirections.Add(Vector3.right);
                break;
            case BlockType.Hex_W_road:
                PossibleDirections.Add(Vector3.right);
                PossibleDirections.Add(Vector3.down + Vector3.left);
                PossibleDirections.Add(Vector3.down + Vector3.right);
                break;
            case BlockType.Hex_X_road:
                PossibleDirections.Add(Vector3.up + Vector3.left);
                PossibleDirections.Add(Vector3.down + Vector3.left);
                PossibleDirections.Add(Vector3.up + Vector3.right);
                PossibleDirections.Add(Vector3.down + Vector3.right);
                break;
            case BlockType.Hex_Y_road:
                PossibleDirections.Add(Vector3.right);
                PossibleDirections.Add(Vector3.up + Vector3.left);
                PossibleDirections.Add(Vector3.down + Vector3.right);
                break;
        }

    }

    private void SquareDirections()
    {
        switch (type)
        {
            // Square types
            case BlockType.Straight:
                PossibleDirections.Add(Vector3.up);
                PossibleDirections.Add(Vector3.down);
                break;
            case BlockType.Turn:
                PossibleDirections.Add(Vector3.right);
                PossibleDirections.Add(Vector3.down);
                break;
            case BlockType.Crossroad_T:
                PossibleDirections.Add(Vector3.right);
                PossibleDirections.Add(Vector3.left);
                PossibleDirections.Add(Vector3.down);
                break;
            case BlockType.Crossroad:
                PossibleDirections.Add(Vector3.right);
                PossibleDirections.Add(Vector3.left);
                PossibleDirections.Add(Vector3.up);
                PossibleDirections.Add(Vector3.down);
                break;
        }
    }

    private void GenerateDirections()
    {
        PossibleDirections.Clear();
        switch (shapeMode)
        {
            case ShapeMode.Triangle:
                break;
            case ShapeMode.Square:
                SquareDirections();
                break;
            case ShapeMode.Hexa:
                HexagonDirections();
                break;
            case ShapeMode.Octo:
                break;
        }

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
    }

    public void SetType(BlockType newType, ShapeMode shape = ShapeMode.Square)
    {
        type = newType;
        shapeMode = shape;
        spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spriteRenderer.sprite = sprites[(int)type];
        GenerateDirections();
    }

    private void OnMouseDown()
    {
        onBlockClicked(gameObject);
    }

}
