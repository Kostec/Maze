using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface InputHandler
{
    public Dictionary<Vector2, GameObject> GameArray { get; set; }
    public Vector2 BufferPosition { get; set; }
    public KeyValuePair<Vector2, GameObject> SelectedBlock { get; set; }
    public bool KeyCheck();
    public void onBlockClicked(GameObject block);
}

