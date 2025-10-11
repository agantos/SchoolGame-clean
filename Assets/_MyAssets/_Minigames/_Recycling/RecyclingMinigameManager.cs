using UnityEngine;

public class RecyclingMinigameManager : MonoBehaviour
{
	public GameObject[] waste;
	public GameObject[] recycleTrash;

	int _recycleCompleted = 0;
	int _wasteCompleted = 0;

	public BinAnimation wasteBin;
	public BinAnimation recycleBin;

	private void Awake()
	{
		
	}

	public bool GetIsWaste(GameObject obj)
	{
		foreach (GameObject w in waste)
		{
			if (w == obj) return true;
		}

		return false;
	}

	public bool IsTrash(GameObject obj)
	{
		foreach (GameObject w in waste)
		{
			if (w == obj) return true;
		}

		foreach (GameObject w in recycleTrash)
		{
			if (w == obj) return true;
		}

		return false;


	}
}
