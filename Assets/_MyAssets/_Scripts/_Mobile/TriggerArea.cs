using UnityEngine;

public abstract class TriggerArea : MonoBehaviour
{
	[SerializeField] protected LayerMask playerLayer;

	protected void OnTriggerEnter(Collider other)
	{
		if (IsPlayer(other.gameObject))
		{
			OnPlayerEnter();
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		if (IsPlayer(other.gameObject))
		{
			OnPlayerExit();
		}
	}

	protected bool IsPlayer(GameObject obj)
	{
		return ((1 << obj.layer) & playerLayer) != 0;
	}

	protected abstract void OnPlayerEnter();

	protected abstract void OnPlayerExit();

	protected void Disable()
	{
		gameObject.GetComponent<SphereCollider>().enabled = false;
		gameObject.GetComponent<MeshRenderer>().enabled = false;
	}

	protected void Enable()
	{
		gameObject.GetComponent<SphereCollider>().enabled = true;
		gameObject.GetComponent<MeshRenderer>().enabled = true;
	}
}