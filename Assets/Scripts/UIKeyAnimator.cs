using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class UIKeyAnimator : MonoBehaviour
{
    private UIKey    key   = GlobalConstants.keyBlank;
    private KeyState state = KeyState.Up;
    private bool     done  = true;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = key.GetSprite(state);
    }

    public void Init(UIKey inKey, KeyState initialState)
    {
        key   = inKey;
        state = initialState;
        spriteRenderer.sprite = key.GetSprite(state);
    }

    public void ToggleKey(float time)
    {
        if(done) StartCoroutine(Toggle(time));
    }
    public void PushKey(float time)
    {
        if(done) StartCoroutine(Push(time));
    }
    public void ReleaseKey(float time)
    {
        if(done) StartCoroutine(Release(time));
    }

    private void ChangeState(KeyState newState)
    {
        state = newState;
        spriteRenderer.sprite = key.GetSprite(state);
    }

    private IEnumerator Toggle(float time)
    {
        done = false;

        bool pressed = state == KeyState.Down;

        ChangeState(KeyState.Half);

        yield return new WaitForSeconds(time / 2f);

        if(pressed)
        {
            ChangeState(KeyState.Up);
        }
        else
        {
            ChangeState(KeyState.Down);
        }

        yield return new WaitForSeconds(time / 2f);

        done = true;
    }

    private IEnumerator Push(float time)
    {
        if(state == KeyState.Down) yield break;

        done = false;

        ChangeState(KeyState.Half);
        yield return new WaitForSeconds(time / 2f);

        ChangeState(KeyState.Down);
        yield return new WaitForSeconds(time / 2f);

        done = true;
    }

    private IEnumerator Release(float time)
    {
        if(state == KeyState.Up) yield break;

        done = false;

        ChangeState(KeyState.Half);
        yield return new WaitForSeconds(time / 2f);

        ChangeState(KeyState.Up);
        yield return new WaitForSeconds(time / 2f);

        done = true;
    }
}
