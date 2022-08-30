using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerController : EntityController
{
	public override Vector2 movement
	{
		get
		{
			return _movement;
		}
		set
		{
			if (isLocalPlayer)
			{
				setMovement(value);
				_movement = value;
			}
		}
	}
	public override Quaternion lookRotation
	{
		get
		{
			return _lookRotation;
		}
		set
		{
			if (isLocalPlayer)
			{
				setLookRotation(value);
				_lookRotation = value;
			}
		}
	}
	public override bool sprinting
	{
		get
		{
			return _sprinting;
		}
		set
		{
			if (isLocalPlayer)
			{
				setSprint(value);
				_sprinting = value;
			}
		}
	}
	public override bool crouching
	{
		get
		{
			return _crouching;
		}
		set
		{
			if (isLocalPlayer)
			{
				setCrouch(value);
				_crouching = value;
			}
		}
	}

	public float lookSensibility = 0.5f;
	public float lookSpeed { get { return lookSensibility * 1000; } set { lookSensibility = value / 1000; } }

	private Quaternion wantedRot;

	public PlayerInput pl;

	public NetworkManager net = NetworkManager.singleton;

	public Camera camera;

	public List<GameObject> UnseeObjs = new List<GameObject>();

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

	public override void SetLookRotation(Quaternion rot)
    {
		lookRotation = rot;
		wantedRot = rot;

	}

    public void Start()
    {
		if (!camera) camera = GetComponentInChildren<Camera>();

		if (isLocalPlayer)
        {
			SetInput(GetComponent<PlayerInput>());

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			UnseeObjs.ForEach(uo => uo.layer = LayerMask.NameToLayer("PlayerUnsee"));

		}
        else
        {
			GetComponent<PlayerInput>().enabled = false;
			camera.gameObject.SetActive(false);
		}
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
		if(isLocalPlayer)
        {
			lookRotation = Quaternion.Slerp(lookRotation, wantedRot, lookSpeed * Time.deltaTime);
        }

		//Debug.Log(lookRotation);
	}

    private void FixedUpdate()
    {
	}
}
