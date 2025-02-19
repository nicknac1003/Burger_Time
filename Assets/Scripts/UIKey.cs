using UnityEngine;

[CreateAssetMenu(fileName = "New UIKey", menuName = "UI Key", order = 0)]
public class UIKey : ScriptableObject
{
    [SerializeField] private Sprite  up;
    [SerializeField] private Sprite  half;
    [SerializeField] private Sprite  down;

    public Sprite GetSprite(KeyState state)
    {
        return state switch
        {
            KeyState.Up   => up,
            KeyState.Half => half,
            KeyState.Down => down,
            _ => null,
        };
    }
}

public enum KeyState
{
    Up,
    Half,
    Down
}