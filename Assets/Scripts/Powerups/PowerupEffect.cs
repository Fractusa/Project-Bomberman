using UnityEngine;

[CreateAssetMenu(fileName = "NewPowerup", menuName = "Bomberman/Powerup")]
public class PowerupEffect : ScriptableObject
{
    public int maxBombs = 0;
    public int extraRange = 0;
    public int moveSpeed = 0;

    public void ApplyEffect(PlayerStats stats)
    {
        stats.maxBombs = maxBombs;
        stats.explosionRange = extraRange;
        stats.moveSpeed = moveSpeed;
    }
}
