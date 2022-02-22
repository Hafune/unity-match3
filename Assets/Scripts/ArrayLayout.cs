using System.Collections.Generic;
using UnityEngine;

public class ArrayLayout : MonoBehaviour
{
    public List<SelectSprite> list = new List<SelectSprite>();
}

[System.Serializable]
public class SelectSprite
{
    public List<Sprite?> images = new List<Sprite?>();
    // public Sprite? image;
}