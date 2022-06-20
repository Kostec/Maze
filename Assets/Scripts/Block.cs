using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        if (spriteRenderer == null)
        {
            return;
        }
        type = (BlockType)UnityEngine.Random.Range(0, 3);
        switch (type)
        {
            case BlockType.Straight:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/straight");
                break;
            case BlockType.Turn:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/turn");
                break;
            case BlockType.Crossroad_T:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/T-crossroads");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit.collider.gameObject == gameObject)
            {
                onBlockClicked(gameObject);
            }
        }
    }

    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {
        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        return NewSprite;
    }

    public Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);
            if (Tex2D.LoadImage(FileData))
                return Tex2D;
        }
        return null;
    }

}
