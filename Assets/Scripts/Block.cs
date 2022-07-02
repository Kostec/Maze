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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetType(BlockType newType)
    {
        type = newType;
        spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spriteRenderer.sprite = sprites[(int)type];
    }

    private void OnMouseDown()
    {
        onBlockClicked(gameObject);
    }

}
