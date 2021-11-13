using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public BoxState state;
    public bool isEmpty;
    public int index;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        index = transform.GetSiblingIndex();
        isEmpty = true;
        state = BoxState.Empty;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MarkBox(Sprite sprite, BoxState newState)
    {
        isEmpty = false;
        this.state = newState;
        spriteRenderer.sprite = sprite;

        GetComponent<CircleCollider2D>().enabled = false;
    }
}
