using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityMovement : MonoBehaviour
{
	[HideInInspector]
	public Rigidbody rb;

	public float walkVelocity;
	public float sprintVelocity;
	public float crouchVelocity;
	public float acceleration;
	public float deceletarion;

	public float crouchScale = 0.5f;

	public bool canSlide = false;
	public bool isSliding = false;
	private bool crouching = false;
	public float slidingFriction = 0.1f;

	//public float lookSpeed;
	public Transform head;

	public float jumpSpeed;

	public float airFriction;
	private float friction { get {

			if (!hasGround)
			{
				return airFriction;
			}
			else if (isSliding)
			{
				return slidingFriction;
			}
            else
            {
				return Mathf.Max(ground.collider.material.staticFriction, airFriction);
			}
		} }
	private float bounciness { get { return hasGround ? ground.collider.material.bounciness : 0; } }

	public LayerMask groundLayer;

	public Collider mainCollider;

	RaycastHit ground;
	public bool hasGround { get { return ground.collider; } }

	[HideInInspector]
	public EntityController controls;

	void Awake()
	{
		if(!mainCollider)
		{
			mainCollider = GetComponent<Collider>();
		}

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		rb = GetComponent<Rigidbody>();
		controls = GetComponent<EntityController>();
	}

	// Update is called once per frame
	void Update()
	{
		head.localRotation = Quaternion.Euler(controls.lookRotation.eulerAngles.x, 0, 0);
		transform.rotation = Quaternion.Euler(0, controls.lookRotation.eulerAngles.y, 0);
	}

	void FixedUpdate()
	{
		Physics.SphereCast(transform.position + Vector3.up * mainCollider.bounds.extents.y, mainCollider.bounds.extents.x - 0.05f, Vector3.down, out ground, mainCollider.bounds.extents.y + 0.1f - mainCollider.bounds.extents.x, groundLayer);

		//Debug.Log(transform.position + Vector3.up * mainCollider.bounds.extents.y);

		if (controls.jumping && hasGround)
        {
			rb.velocity = new Vector3(rb.velocity.x, jumpSpeed + ((-rb.velocity.y) * bounciness), rb.velocity.z);
		}

		isSliding = canSlide && controls.crouching && controls.sprinting && rb.velocity.magnitude > crouchVelocity;

		if(controls.crouching && !crouching)
        {
			head.transform.position = transform.position + (head.transform.position - transform.position) * crouchScale;
			mainCollider.Scale(new Vector3(1, crouchScale, 1));
			mainCollider.SetCenter(Vector3.up * mainCollider.bounds.extents.y);
			//temporal
        }
        else if(!controls.crouching && crouching)
		{
			head.transform.position = transform.position + (head.transform.position - transform.position) / crouchScale;
			mainCollider.Scale(new Vector3(1, 1/crouchScale, 1));
			mainCollider.SetCenter(Vector3.up * mainCollider.bounds.extents.y);

		}

		crouching = controls.crouching;

		if (controls.movement.magnitude != 0 || (!CustomMath.Aproximately(rb.velocity.magnitude, 0) && hasGround))
		{
			//Debug.Log(controls.movement);
			Vector2 vel;

			Vector2 movementDir = CustomMath.RotateVector(controls.movement, new Vector2(transform.forward.x, transform.forward.z).normalized);
			if(controls.sprinting && !isSliding)
            {
				vel = CustomMath.CalculateVelocity(new Vector2(rb.velocity.x, rb.velocity.z), movementDir, acceleration, deceletarion, sprintVelocity, friction);
			}
			else if(controls.crouching)
            {
				vel = CustomMath.CalculateVelocity(new Vector2(rb.velocity.x, rb.velocity.z), movementDir, acceleration, deceletarion, crouchVelocity, friction);
			}
            else
            {
				vel = CustomMath.CalculateVelocity(new Vector2(rb.velocity.x, rb.velocity.z), movementDir, acceleration, deceletarion, walkVelocity, friction);
            }

			rb.velocity = new Vector3(vel.x, rb.velocity.y, vel.y);
		}
	}

    public void OnDrawGizmos()
    {
		if (Application.isPlaying)
		{
			Gizmos.color = hasGround ? Color.blue : Color.red;
			//Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (mainCollider.bounds.size.y / 2 - mainCollider.bounds.size.x / 2 + 0.1f));
			//Gizmos.DrawWireSphere(transform.position + Vector3.down * (mainCollider.bounds.size.y / 2 - mainCollider.bounds.size.x / 2 + 0.1f), mainCollider.bounds.size.x / 2 - 0.05f);

			Gizmos.DrawLine(transform.position + Vector3.up * mainCollider.bounds.extents.y, transform.position + Vector3.up * mainCollider.bounds.extents.y + Vector3.down * (hasGround ? mainCollider.bounds.extents.y - ground.distance : mainCollider.bounds.extents.y + 0.1f - mainCollider.bounds.extents.x));
			Gizmos.DrawWireSphere(transform.position + Vector3.up * mainCollider.bounds.extents.y + Vector3.down * (hasGround ? mainCollider.bounds.extents.y - ground.distance : mainCollider.bounds.extents.y + 0.1f - mainCollider.bounds.extents.x), mainCollider.bounds.extents.x - 0.05f);
		}
	}
}
