using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class UIKeyAnimator : MonoBehaviour
{
    public enum Perform
    {
        Press,
        Release,
        Hold,
        Mash
    }

    [SerializeField] private KeyIcon key     = KeyIcon.None;
    [SerializeField] private Perform perform = Perform.Press;
    [SerializeField] private bool    reverse = false;
    [Range(0.1f,5f)][SerializeField] private float time = 1f;

    private static Dictionary<KeyIcon, UIKey> keys = new();

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;


    void Start()
    {
        if(keys.Count == 0)
        {
            LoadKeys();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        switch(perform)
        {
            case Perform.Press:
            {

                break;
            }
            case Perform.Release:
            {
                break;
            }
            case Perform.Hold:
            {
                break;
            }
            case Perform.Mash:
            {
                if(timer < time / 3f)
                {
                    spriteRenderer.sprite = keys[key].GetSprite(KeyState.Up, reverse);
                }
                else if(timer < time * 2f / 3f)
                {
                    spriteRenderer.sprite = keys[key].GetSprite(KeyState.Half, reverse);
                }
                else
                {
                    spriteRenderer.sprite = keys[key].GetSprite(KeyState.Down, reverse);
                }
                break;
            }
        }

        timer += Time.deltaTime;

        if(timer >= time)
        {
            timer = 0f;
        }
    }

    private void LoadKeys()
    {
        UIKey[] loadedKeys = Resources.LoadAll<UIKey>("UIKeys");

        foreach(UIKey key in loadedKeys)
        {
            keys.Add(key.Key(), key);
        }
    }
}
