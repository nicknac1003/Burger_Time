using UnityEngine;

[CreateAssetMenu(fileName = "New UIKey", menuName = "UI Key", order = 0)]
public class UIKey : ScriptableObject
{
    [SerializeField] private KeyIcon key = KeyIcon.None;
    [SerializeField] private Sprite  up;
    [SerializeField] private Sprite  half;
    [SerializeField] private Sprite  down;

    public KeyIcon Key() => key;

    public Sprite GetSprite(KeyState state, bool reverse)
    {
        if(reverse)
        {
            switch(state)
            {
                case KeyState.Up:   return down;
                case KeyState.Half: return half;
                case KeyState.Down: return up;
            }
        }
        else
        {
            switch(state)
            {
                case KeyState.Up:   return up;
                case KeyState.Half: return half;
                case KeyState.Down: return down;
            }
        }

        return null;
    }
}

public enum KeyIcon
{
    None,
    Z,
    X,
    C,
    Space,
    Up,
    Down,
    Right,
    Left,
    Esc
}

public enum KeyState
{
    Up,
    Half,
    Down
}