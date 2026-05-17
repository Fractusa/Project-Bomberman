using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(OnColorChoiceChanged))]
    public PlayerColorChoice colorChoice = PlayerColorChoice.Red;
    [SerializeField] private Renderer renderer;

    public override void OnStartClient()
    {
        base.OnStartClient();

        ApplyLobbyPreviewColor(colorChoice);
    }

    [Command]
    private void CmdSetColorChoice(PlayerColorChoice newChoice)
    {
        if (!System.Enum.IsDefined(typeof(PlayerColorChoice), newChoice))
            return;
        
        colorChoice = newChoice;
    }

    private void OnColorChoiceChanged(PlayerColorChoice oldChoice, PlayerColorChoice newChoice)
    {
        ApplyLobbyPreviewColor(newChoice);
    }

    private void ApplyLobbyPreviewColor(PlayerColorChoice choice)
    {
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
            renderer.material.color = PlayerColorUtility.ToColor(choice);
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (!isLocalPlayer)
            return;

        GUILayout.BeginArea(new Rect(500, 20, 250, 300), "Player Color", GUI.skin.window);

        GUILayout.Label($"Chosen color: {colorChoice}");

        foreach (PlayerColorChoice choice in System.Enum.GetValues(typeof(PlayerColorChoice)))
        {
            GUI.backgroundColor = PlayerColorUtility.ToColor(choice);

            if (GUILayout.Button(choice.ToString(), GUILayout.Height(35)))
            {
                CmdSetColorChoice(choice);
            }
        }

        GUI.backgroundColor = Color.white;

        GUILayout.EndArea();
    }
}
