using UnityEngine;

public abstract class TriggerArea : MonoBehaviour
{
	[SerializeField] protected LayerMask objectLayer;

	protected void OnTriggerEnter(Collider other)
	{
		if (IsTriggerObject(other.gameObject))
		{
			OnObjectEnter(other.gameObject);
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		if (IsTriggerObject(other.gameObject))
		{
			OnObjectExit(other.gameObject);
		}
	}

	protected bool IsTriggerObject(GameObject obj)
	{
		return ((1 << obj.layer) & objectLayer) != 0;
	}

	protected abstract void OnObjectEnter(GameObject obj = null);

	protected abstract void OnObjectExit(GameObject obj = null);

	public void Disable()
	{
		gameObject.GetComponent<SphereCollider>().enabled = false;
		gameObject.GetComponent<MeshRenderer>().enabled = false;
	}

	public void Enable()
	{
		gameObject.GetComponent<SphereCollider>().enabled = true;
		gameObject.GetComponent<MeshRenderer>().enabled = true;
	}
}