using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
	public Portal exit;
	public MeshRenderer meshRenderer;

    public void Start()
    {
		if(!meshRenderer)
        {
			meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

	}

    private void OnTriggerStay(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Entity"))
		{
			other.gameObject.layer = LayerMask.NameToLayer("PortalEntity");
		}
		
		if(meshRenderer.transform.InverseTransformPoint(other.transform.position).z >= 0)
		{
			other.transform.position = exit.transform.TransformPoint(transform.InverseTransformPoint(other.transform.position).invertX()) + (exit.transform.forward * 0.1f);

			EntityController ec = other.GetComponent<EntityController>();

			if(ec)
            {
				ec.SetLookRotation(Quaternion.LookRotation(exit.transform.TransformDirection(-transform.InverseTransformDirection(ec.lookRotation * Vector3.forward).invertY())));
			}
			else
            {
				other.transform.forward = exit.transform.TransformDirection(-transform.InverseTransformDirection(other.transform.forward).invertY());
			}

			Rigidbody orb = other.gameObject.GetComponent<Rigidbody>();

			if(orb)
            {
				//orb.velocity = exit.transform.rotation * Quaternion.Inverse(transform.rotation) * -orb.velocity;
				orb.velocity = exit.transform.TransformVector(-transform.InverseTransformVector(orb.velocity).invertY());
			}
		}
	}

	void modifyVel()
    {

    }

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("PortalEntity"))
		{
			other.gameObject.layer = LayerMask.NameToLayer("Entity");
		}
	}
}
