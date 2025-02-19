using UnityEngine;

[CreateAssetMenu(fileName = "New UIKey", menuName = "UI Key", order = 0)]
public class UIKey : ScriptableObject
{
    [SerializeField] private KeyIcon key = KeyIcon.None;
    [SerializeField] private Sprite  up;
    [SerializeField] private Sprite  half;
    [SerializeField] private Sprite  down;

    public KeyIcon Key() => key;
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