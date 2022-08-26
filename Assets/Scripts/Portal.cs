using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
	public Portal exit;
	//public GameObject walls;

	private void OnTriggerStay(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Entity"))
		{
			other.gameObject.layer = LayerMask.NameToLayer("PortalEntity");
		}
		
		if(Vector3.Scale(transform.forward, (other.transform.position - transform.position)).magnitude <= 0.05f )
		{
			other.transform.position = exit.transform.TransformPoint(transform.InverseTransformPoint(other.transform.position)) + (exit.transform.forward * 0.1f);

			EntityController ec = other.GetComponent<EntityController>();

			if(ec)
            {
				//ec.lookRotation = exit.transform.rotation * Quaternion.Inverse(transform.rotation) * ec.lookRotation;
				//other.transform.rotation = exit.transform.rotation * Quaternion.Inverse(transform.rotation) * other.transform.rotation;

			}
			else
            {
				other.transform.rotation = exit.transform.rotation * Quaternion.Inverse(transform.rotation) * other.transform.rotation;
			}

			Rigidbody orb = other.gameObject.GetComponent<Rigidbody>();

			if(orb)
            {
				orb.velocity = exit.transform.TransformDirection(transform.InverseTransformDirection(orb.velocity));
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("PortalEntity"))
		{
			other.gameObject.layer = LayerMask.NameToLayer("Entity");
		}
	}
}
