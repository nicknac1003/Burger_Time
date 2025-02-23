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

    public void Dropped(Storage storage){
        held = false;
        spriteRenderer.sortingOrder = 2;
        if (storage.name == "FireExtinguisherSpot") hitbox.enabled = true;
    }

}