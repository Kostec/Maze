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
    Crossroad_T // Т-образный перекрёсток
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
                PossibleDirections.Add(new Vector3(0, 1, 0));
                PossibleDirections.Add(new Vector3(0, -1, 0));
                break;
            case BlockType.Turn:
                PossibleDirections.Add(new Vector3(1, 0, 0));
                PossibleDirections.Add(new Vector3(0, -1, 0));
                break;
            case BlockType.Crossroad_T:
                PossibleDirections.Add(new Vector3(1, 0, 0));
                PossibleDirections.Add(new Vector3(-1, 0, 0));
                PossibleDirections.Add(new Vector3(0, 1, 0));
                PossibleDirections.Add(new Vector3(0, -1, 0));
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
