using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbBehaviour : MonoBehaviour
{
	public float climbingSpeed = 2;
	public bool canClimbLedges = false;
	public float grabDistance = 0.2f;
	public float maxClimbHeight = 1;
	public LayerMask ledgeClimbingLayer;

	public Transform righthand;
	//public Transform lefthand;

	public Collider mainCollider;

	private bool alreadyLedgeClimbing = false;

	[HideInInspector]
	public EntityController controls;
	[HideInInspector]
	public EntityMovement movement;
	[HideInInspector]
	public Rigidbody rb;

	// Start is called before the first frame update
	void Start()
	{
		if (!mainCollider)
		{
			mainCollider = GetComponent<Collider>();
		}

		rb = GetComponent<Rigidbody>();
		controls = GetComponent<EntityController>();
		movement = GetComponent<EntityMovement>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		

		if (canClimbLedges && !movement.hasGround && !alreadyLedgeClimbing && controls.jumping && rb.velocity.y <= climbingSpeed)
		{
			Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + maxClimbHeight, transform.position.z) + transform.forward * (mainCollider.bounds.extents.x + grabDistance);
			RaycastHit hit;
			if (Physics.Raycast(rayOrigin, Vector3.down, out hit, maxClimbHeight - 0.1f, ledgeClimbingLayer) && hit.normal.y >= 0.5f)
			{
				StartCoroutine(ClimbLedge(hit.point));

				//Debug.Log("shoould have started");
				//righthand.position = hit.point + transform.right * mainCollider.bounds.extents.x;
				//rb.velocity = new Vector3(rb.velocity.x, climbingSpeed, rb.velocity.z);
			}
			else
            {
				Debug.DrawLine(rayOrigin, rayOrigin + Vector3.down * maxClimbHeight, Color.red);
            }
		}
	}

	IEnumerator ClimbLedge(Vector3 endPosition)
	{
		alreadyLedgeClimbing = true;
		controls.blockMotion = true;
		rb.isKinematic = true;

		Vector3 initPos = transform.position;
		Vector3 ledgePos = endPosition - Vector3.up * mainCollider.bounds.extents.y;

		float progress = 0;

		//Debug.Log("started");

		for(float time = 0; progress < 1; time += Time.deltaTime)
		{
			progress = (climbingSpeed * time) / (endPosition - initPos).magnitude;

			transform.position = CustomMath.twoPoleBezierLerp(initPos, endPosition,
				new Vector3(initPos.x, endPosition.y, initPos.z),
				ledgePos,
				progress);

			yield return new WaitForEndOfFrame();
		}

		alreadyLedgeClimbing = false;
		controls.blockMotion = false;
		controls.jumping = false;
		rb.isKinematic = false;
	}
}
