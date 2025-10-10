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

	
}
