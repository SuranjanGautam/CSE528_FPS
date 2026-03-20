using UnityEngine;
using System.Collections;

// basic WASD-style movement control
// commented out line demonstrates that transform.Translate instead of 
// charController.Move doesn't have collision detection

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour {
	public float speed = 6.0f;
	public float gravity = -9.8f;

	public float jumpVel = 6;

	private CharacterController _charController;

	float yvel = 0; // to track vertical velocity for jumping and falling, since charController.Move doesn't handle gravity or jumping for us

    void Start()
    {
		_charController = GetComponent<CharacterController>();
	}
	
	void Update()
    {
		//transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime);
		
		float deltaX = Input.GetAxis("Horizontal") * speed;
		float deltaZ = Input.GetAxis("Vertical") * speed;

		Vector3 movement = new Vector3(deltaX, 0, deltaZ);

        movement = Vector3.ClampMagnitude(movement, speed); //prevents faster diagonal movement


        if (_charController.isGrounded)
        {
            yvel = 0;
        }
        else
        {
            yvel += gravity * Time.deltaTime;
        }

        if (Input.GetAxis("Jump")>0.5 && _charController.isGrounded)
		{
			yvel = jumpVel;
        }

        
        movement.y += yvel;

        movement *= Time.deltaTime;

        // Transforms movement from local space to world space.
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
		
    }
}
