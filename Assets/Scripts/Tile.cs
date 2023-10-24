using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int _tileType;

    public int TileType { get => _tileType; set => _tileType = value; }
    public int GetTileType() { return TileType; }  // tile tipini gönderir
    public int GetX() { return (int)transform.localPosition.x; }  // X pozisyonu localde
    public int GetY() { return (int)transform.localPosition.y * -1; }  // Y pozisyonu localde (*-1 : tahtada yukardan aþaðý götüreleceði için)

    public void SetSprite(Sprite sprite)
    {
        // Gelen Sprite bu objenin sprite olarak atanýr
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
