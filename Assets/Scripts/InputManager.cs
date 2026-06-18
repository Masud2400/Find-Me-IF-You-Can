using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
	private PlayerControls.PlayerMoveActions playerMove;
	[SerializeField] private PlayerController playerController;
	
	void Awake()
	{
		playerControls = new PlayerControls();
		playerMove = playerControls.PlayerMove;
	}
	
	void FixedUpdate()
	{
		playerController.ProcessMove(playerMove.Move.ReadValue<Vector2>());
	}
	
	private void OnEnable()
	{
		playerMove.Enable();
	}
	
	private void OnDisable()
	{
		playerMove.Disable();
	}
}
