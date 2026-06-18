using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Speeds")]
	[SerializeField] private float currentSpeed = 5f;
	[SerializeField] private float rotationSpeed = 10f; 
	
	[Header("Game Objects")]
	[SerializeField] private Transform cameraTransform;
	
	private CharacterController controller;
	private Vector3 currentMoveDirection;
	
	void Start()
	{
		controller = GetComponent<CharacterController>();
	}
	
	public void ProcessMove(Vector2 input)
	{	
		Vector3 forward = cameraTransform.forward;
		Vector3 right = cameraTransform.right;

		forward.y = 0f;
		right.y = 0f;
		forward.Normalize();
		right.Normalize();

		Vector3 targetDirection = (forward * input.y) + (right * input.x);
		
		// The change of direction was snappy but I solved it with MoveTowards
		currentMoveDirection = Vector3.MoveTowards(
			currentMoveDirection, 
			targetDirection, 
			rotationSpeed * Time.fixedDeltaTime
		);
		
		controller.Move(currentMoveDirection * currentSpeed * Time.fixedDeltaTime);
	}
}
