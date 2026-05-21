using UnityEngine;
using Mirror;
using System.Collections;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-room-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomManager.html

	See Also: NetworkManager
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

/// <summary>
/// This is a specialized NetworkManager that includes a networked room.
/// The room has slots that track the joined players, and a maximum player count that is enforced.
/// It requires that the NetworkRoomPlayer component be on the room player objects.
/// NetworkRoomManager is derived from NetworkManager, and so it implements many of the virtual functions provided by the NetworkManager class.
/// </summary>
public class RoomManager : NetworkRoomManager
{
    // Overrides the base singleton so we don't
    // have to cast to this type everywhere.
    public static new RoomManager singleton => (RoomManager)NetworkRoomManager.singleton;

    private bool isRestartingRound;

    [Server]
    public void RestartGameplayScene()
    {
        if (isRestartingRound)
            return;

        isRestartingRound = true;
        StartCoroutine(RestartGameplaySceneRoutine());
    }

    [Server]
    private IEnumerator RestartGameplaySceneRoutine()
    {
        // Destroy current game players.
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity != null)
            {
                NetworkServer.Destroy(conn.identity.gameObject);
            }
        }

        yield return null;

        // Reload the gameplay scene for everyone.
        ServerChangeScene(GameplayScene);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName != GameplayScene)
            return;

        if (!isRestartingRound)
            return;

        isRestartingRound = false;

        // Recreate game players from existing room players.
        foreach (NetworkRoomPlayer rp in roomSlots)
        {
            if (rp == null)
                continue;

            NetworkConnectionToClient conn = rp.connectionToClient;

            if (conn == null)
                continue;

            Transform startPos = GetStartPosition();

            GameObject gamePlayer = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            // Copy room data onto the new game player.
            RoomPlayer roomPlayer = rp.GetComponent<RoomPlayer>();
            PlayerColor playerColor = gamePlayer.GetComponent<PlayerColor>();

            if (playerColor != null && playerColor != null)
            {
                playerColor.colorChoice = roomPlayer.colorChoice;
            }

            NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, true);
        }
    }

    /// <summary>
    /// This allows customization of the creation of the room-player object on the server.
    /// <para>By default the roomPlayerPrefab is used to create the room-player, but this function allows that behaviour to be customized.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    /// <returns>The new room-player object.</returns>
    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        return base.OnRoomServerCreateRoomPlayer(conn);
    }

    /// <summary>
    /// This allows customization of the creation of the GamePlayer object on the server.
    /// <para>By default the gamePlayerPrefab is used to create the game-player, but this function allows that behaviour to be customized. The object returned from the function will be used to replace the room-player on the connection.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    /// <param name="roomPlayer">The room player object for this connection.</param>
    /// <returns>A new GamePlayer object.</returns>
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        Transform startPos = GetStartPosition();

        GameObject gamePlayer = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        RoomPlayer room = roomPlayer.GetComponent<RoomPlayer>();
        PlayerColor player = gamePlayer.GetComponent<PlayerColor>();

        if (room != null && player != null)
        {
            player.colorChoice = room.colorChoice;
        }

        return gamePlayer;
    }

    /// <summary>
    /// This allows customization of the creation of the GamePlayer object on the server.
    /// <para>This is only called for subsequent GamePlay scenes after the first one.</para>
    /// <para>See OnRoomServerCreateGamePlayer to customize the player object for the initial GamePlay scene.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);
    }

    /// <summary>
    /// This is called on the server when it is told that a client has finished switching from the room scene to a game player scene.
    /// <para>When switching from the room, the room-player is replaced with a game-player object. This callback function gives an opportunity to apply state from the room-player to the game-player object.</para>
    /// </summary>
    /// <param name="conn">The connection of the player</param>
    /// <param name="roomPlayer">The room player object.</param>
    /// <param name="gamePlayer">The game player object.</param>
    /// <returns>False to not allow this player to replace the room player.</returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
    }

    /// <summary>
    /// This is called on server from NetworkRoomPlayer.CmdChangeReadyState when client indicates change in Ready status.
    /// </summary>
    public override void ReadyStatusChanged()
    {
        base.ReadyStatusChanged();
    }

    /// <summary>
    /// This is called on the server when all the players in the room are ready.
    /// <para>The default implementation of this function uses ServerChangeScene() to switch to the game player scene. By implementing this callback you can customize what happens when all the players in the room are ready, such as adding a countdown or a confirmation for a group leader.</para>
    /// </summary>
    public override void OnRoomServerPlayersReady()
    {
        base.OnRoomServerPlayersReady();
    }

    #region Optional UI

    public override void OnGUI()
    {
        base.OnGUI();
    }

    #endregion
}
