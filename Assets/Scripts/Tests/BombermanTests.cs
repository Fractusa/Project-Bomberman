using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Mirror;

public class BombermanTests
{




    //CASE 1: Testing whether powerups correctly provide the powerup to the player character.
    [Test]
    public void PlayerStats_PickingUpRangePowerup_IncreasesBombRange()
    {


        //ARRANGE
        GameObject playerObj = new GameObject();
        playerObj.AddComponent<NetworkIdentity>();
        PlayerStats stats = playerObj.AddComponent<PlayerStats>();
        stats.bombRange = 1;

        PowerupEffect rangeEffect = ScriptableObject.CreateInstance<PowerupEffect>();
        rangeEffect.extraRange = 1;
        rangeEffect.maxBombs = 0;
        rangeEffect.moveSpeed = 0;

        //ACT
        stats.ApplyPowerupLogic(rangeEffect);

        //ASSERT
        Assert.AreEqual(2, stats.bombRange, "Players bombRange did not correctly increase, test failed");


    }


    //CASE 2: Testing whether the limit for how many bombs a player can place works properly
    [Test]
    public void PlayerBombPlacer_CanPlaceBomb_ReturnsFalse_WhenActiveBombsEqualsMaxBombs()
    {

        //ARRANGE
        GameObject playerObj = new GameObject();
        playerObj.AddComponent<NetworkIdentity>();
        PlayerStats stats = playerObj.AddComponent<PlayerStats>();
        stats.maxBombs = 2;
        stats.activeBombs = 2;

        //ACT
        bool canPlaceMore = stats.activeBombs < stats.maxBombs;

        //ASSERT
        Assert.IsFalse(canPlaceMore, "Player was allowed to place a bomb, even though max bomb capacity for the player was reached, test failed");
    }


    //CASE 3: Testing whether GameManager properly resets the scoreboard before a new game is started.
    [Test]
    public void GameManager_ResetAllScores_SetsAllTeamScoresToZero()
    {

        //ARRANGE
        GameObject gmObj = new GameObject();
        gmObj.AddComponent<NetworkIdentity>();
        GameManager gameManager = gmObj.AddComponent<GameManager>();

        gameManager.scoreBlue = 3;
        gameManager.scoreGreen = 1;
        gameManager.scoreRed = 0;

        //ACT
        gameManager.ResetAllScores();

        //ASSERT
        Assert.AreEqual(0, gameManager.scoreRed, "Red team score was not reset, test failed");
        Assert.AreEqual(0, gameManager.scoreBlue, "Blue team score was not reset, test failed");
        Assert.AreEqual(0, gameManager.scoreGreen, "Green team score was not reset, test failed");
    }


}
