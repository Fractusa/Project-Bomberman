using UnityEngine;
using Mirror;

public class PlayerColor : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChoiceChanged))]
    public PlayerColorChoice colorChoice = PlayerColorChoice.Red;

    [SerializeField] private Renderer targetRenderer;

    public override void OnStartClient()
    {
        base.OnStartClient();
        ApplyColor(colorChoice);
    }

    private void OnColorChoiceChanged(PlayerColorChoice oldChoice, PlayerColorChoice newChoice)
    {
        ApplyColor(newChoice);
    }

    private void ApplyColor(PlayerColorChoice choice)
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
            targetRenderer.material.color = PlayerColorUtility.ToColor(choice);

    }

}
