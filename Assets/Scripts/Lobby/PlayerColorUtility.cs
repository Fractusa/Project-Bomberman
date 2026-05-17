using UnityEngine;

public static class PlayerColorUtility
{
    public static Color ToColor(PlayerColorChoice choice)
    {
        return choice switch
        {
            PlayerColorChoice.Red => Color.red,
            PlayerColorChoice.Blue => Color.blue,
            PlayerColorChoice.Green => Color.green,
            PlayerColorChoice.Yellow => Color.yellow,
            PlayerColorChoice.Purple => new Color(0.5f, 0f, 1f),
            PlayerColorChoice.Orange => new Color(1f, 0.5f, 0f),
            _ => Color.white
        };
    }
}
