using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class EntityController : NetworkBehaviour
{
	[SerializeField]
	[SyncVar]
	protected Vector2 _movement = Vector2.zero;
	[SerializeField]
	[SyncVar]
	protected Quaternion _lookRotation = Quaternion.identity;


	public bool jumping = false;
	public bool blockMotion = false;
	public bool hasGround = false;

	[SerializeField]
	[SyncVar]
	protected bool _sprinting = false;
	[SerializeField]
	[SyncVar]
	protected bool _crouching = false;


	public virtual Vector2 movement
	{ get {
			//if (isClientOnly) return getServerVar(ref _movement);
			return _movement;
		}
		set {
			if (isServer) _movement = value;
			else setMovement(value);
		} }
	public virtual Quaternion lookRotation
	{
		get
		{
			//if (isClientOnly) return getServerVar(ref _lookRotation);
			return _lookRotation;
		}
		set
		{
			if (isServer) _lookRotation = value;
			else setLookRotation(value);
		}
	}
	public virtual bool sprinting
	{
		get
		{
			//if (isClientOnly) return getServerVar(ref _sprinting);
			return _sprinting;
		}
		set
		{
			if (isServer) _sprinting = value;
			else setSprint(value);
		}
	}
	public virtual bool crouching
	{
		get
		{
			//if (isClientOnly) return getServerVar(ref _crouching);
			return _crouching;
		}
		set
		{
			if (isServer) _crouching = value;
			else setCrouch(value);
		}
	}


	// Start is called before the first frame update
	void Start()
	{

	}

	public virtual void SetLookRotation(Quaternion rot)
	{
		lookRotation = rot;

	}

	// Update is called once per frame
	void Update()
	{

	}

	[Command]
	public void setSprint(bool value)
	{
		_sprinting = value;

	}

	[Command]
	public void setCrouch(bool value)
	{
		_crouching = value;
	}

	[Command]
	public void setMovement(Vector2 value)
	{
		_movement = value;

	}

	[Command]
	public void setLookRotation(Quaternion value)
	{
		_lookRotation = value;

	}
}
