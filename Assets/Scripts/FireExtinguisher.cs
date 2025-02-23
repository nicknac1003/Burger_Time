using NUnit.Framework;
using UnityEngine;

public class FireExtinguisher: Holdable {

    private bool held = false;

    [SerializeField] private Collider2D hitbox;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Taken(){
        held = true;
        hitbox.enabled = false;
        spriteRenderer.sortingOrder = 3;
    }

    public void Dropped(){
        Debug.Log("Dropped");
        held = false;
        hitbox.enabled = true;
        spriteRenderer.sortingOrder = 2;
    }

}