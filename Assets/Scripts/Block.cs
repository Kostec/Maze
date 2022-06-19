using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public delegate void ClickBlockHandler(GameObject obj);
    private SpriteRenderer spriteRenderer;
    public event ClickBlockHandler onBlockClicked;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        if (spriteRenderer == null)
        {
            return;
        }
        spriteRenderer.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
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

}
