using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine;

public enum BlockType
{
    Straight, // Прямо
    Turn, // Поворот
    Crossroad_T, // Т-образный перекрёсток
    Crossroad // перекрёсток
}

public enum ShapeMode
{
    Triangle,
    Square,
    Gexsa,
    Octo
};

public class Block : MonoBehaviour
{
    public delegate void ClickBlockHandler(GameObject obj);
    private SpriteRenderer spriteRenderer;
    public event ClickBlockHandler onBlockClicked;

    private BlockType type = BlockType.Straight;

    [SerializeField]
    public Sprite[] sprites;
    public List<Vector3> PossibleDirections;
    private int angle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var vec = new Vector3(1, 0, 0);
    }

    private void GenerateDirections()
    {
        PossibleDirections.Clear();
        switch (type)
        {
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

    public void Rotate(int angle)
    {
        this.angle += angle;
        if (Math.Abs(angle) > 360)
        {
            this.angle = 0;
        }
        transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f), angle);
        for(int i = 0; i < PossibleDirections.Count; i++)
        {
            var currentDirection = PossibleDirections[i];
            float cs = (float)Math.Cos(angle * Math.PI / 180);
            float sn = (float)Math.Sin(angle * Math.PI / 180);
            float oldX = currentDirection.x;
            float oldY = currentDirection.y;
            currentDirection.x = (int)Math.Round(oldX * cs - oldY * sn);
            currentDirection.y = (int)Math.Round(oldX * sn + oldY * cs);
            PossibleDirections[i] = currentDirection.normalized;
        }
    }

    public void SetType(BlockType newType)
    {
        type = newType;
        spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spriteRenderer.sprite = sprites[(int)type];
        GenerateDirections();
    }

    private void OnMouseDown()
    {
        onBlockClicked(gameObject);
    }

}
