using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public InputAction moveAction;
    private CharacterController controller;

    public override void OnStartLocalPlayer()
    {
        // Camera.main.transform.SetParent(transform);
        // Camera.main.transform.localPosition = new Vector3(0,0,0);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions.FindAction("Move");
    }
    
    void Update()
    {
        if (!isLocalPlayer) {return;}

        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(moveValue.x, 0, moveValue.y);
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
