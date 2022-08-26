using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController
{
	public float lookSensibility = 0.5f;
	public float lookSpeed { get { return lookSensibility * 2000; } set { lookSensibility = value / 2000; } }

	private Quaternion wantedRot;

	public PlayerInput pl;
	

	public void move(InputAction.CallbackContext context)
	{
		if(context.performed)
        {
			movement = context.ReadValue<Vector2>();
        }
		else if (context.canceled)
		{
			movement = Vector2.zero;
        }
	}

	public void jump(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			jumping = true;
		}
		else if(context.canceled)
        {
			jumping = false;

		}
	}
	public void sprint(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			sprinting = true;
		}
		else if (context.canceled)
		{
			sprinting = false;

		}
	}
	public void crouch(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			crouching = true;
		}
		else if (context.canceled)
		{
			crouching = false;

		}
	}

	public void look(InputAction.CallbackContext context)
	{
		Vector2 lookDir = context.ReadValue<Vector2>();

		wantedRot = Quaternion.Euler(new Vector3(Mathf.Clamp(-Mathf.DeltaAngle(lookRotation.eulerAngles.x, 0) + -lookDir.y, -89, 89), lookRotation.eulerAngles.y + lookDir.x, 0));
	}

    public void Start()
    {

		SetInput(GetComponent<PlayerInput>());
	}

	public void SetInput(PlayerInput _pl)
    {
		pl = _pl;

		if (pl)
        {
			//pl.actions.FindAction("Move").started += move;
			//Debug.Log(pl.actionEvents[0].);

			pl.actionEvents.FirstOrDefault(e => e.actionName.Contains("Player/Move")).AddListener(move);
			pl.actionEvents.FirstOrDefault(e => e.actionName.Contains("Player/Jump")).AddListener(jump);
			pl.actionEvents.FirstOrDefault(e => e.actionName.Contains("Player/Look")).AddListener(look);
			pl.actionEvents.FirstOrDefault(e => e.actionName.Contains("Player/Sprint")).AddListener(sprint);
			pl.actionEvents.FirstOrDefault(e => e.actionName.Contains("Player/Crouch")).AddListener(crouch);
		}
	}

    // Update is called once per frame
    void Update()
    {
		//Quaternion rot = Quaternion.Slerp(lookRotation, wantedRot, 10 * Time.deltaTime);

		lookRotation = Quaternion.Slerp(lookRotation, wantedRot, lookSpeed * Time.deltaTime);
	}

    private void FixedUpdate()
    {
	}
}
