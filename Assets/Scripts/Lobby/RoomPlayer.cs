using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(OnColorChoiceChanged))]
    public PlayerColorChoice colorChoice = PlayerColorChoice.Red;

    [Header("UI")]
    [SerializeField] private TMP_Dropdown colorDropdown;
    [SerializeField] private TextMeshProUGUI selectedColorText;
    [SerializeField] private Image selectedColorImage;

    [Header("Lobby Preview")]
    [SerializeField] private Renderer previewRenderer;
    private bool isUpdatingUI;

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        Debug.Log($"Lobby color changed to {newColor}");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        SetupColorDropdown();
        UpdateColorUI(colorChoice);
        ApplyLobbyPreviewColor(colorChoice);
    }

    private void SetupColorDropdown()
    {
        if (colorDropdown == null)
            return;

        colorDropdown.ClearOptions();
        colorDropdown.AddOptions(
            new System.Collections.Generic.List<string>(System.Enum.GetNames(typeof(PlayerColorChoice)))
        );

        colorDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        colorDropdown.onValueChanged.AddListener(OnDropdownChanged);

        colorDropdown.interactable = isLocalPlayer;
    }

    private void OnDropdownChanged(int index)
    {
        if(isUpdatingUI)
            return;

        if(!isLocalPlayer)
            return;

        PlayerColorChoice choice = (PlayerColorChoice)index;

        CmdSetColorChoice(choice);

        UpdateColorUI(choice);
        ApplyLobbyPreviewColor(choice);
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
        UpdateColorUI(newChoice);
        ApplyLobbyPreviewColor(newChoice);
    }

    private void UpdateColorUI(PlayerColorChoice choice)
    {
        isUpdatingUI = true;

        if (colorDropdown != null)
            colorDropdown.value = (int)choice;

        if (selectedColorText != null)
            selectedColorText.text = $"Color: {choice}";

        if (selectedColorImage != null)
            selectedColorImage.color = PlayerColorUtility.ToColor(choice);

        isUpdatingUI = false;
    }

    private void ApplyLobbyPreviewColor(PlayerColorChoice choice)
    {
        if (previewRenderer == null)
            previewRenderer = GetComponentInChildren<Renderer>();

        if (previewRenderer != null)
            previewRenderer.material.color = PlayerColorUtility.ToColor(choice);
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
