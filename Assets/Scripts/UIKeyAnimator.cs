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
    [Range(0.1f,5f)][SerializeField] private float   time    = 1f;

    private static Dictionary<KeyIcon, UIKey> keys = new();

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;

    static UIKeyAnimator()
    {
        UIKey[] loadedKeys = Resources.LoadAll<UIKey>("UIKeys");
        foreach(UIKey key in loadedKeys)
        {
            keys.Add(key.Key(), key);
        }
    }

    void Start()
    {
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
                break;
            }
        }

        timer += Time.deltaTime;
    }
}
