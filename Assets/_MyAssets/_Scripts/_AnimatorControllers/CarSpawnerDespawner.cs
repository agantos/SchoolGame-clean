using Cysharp.Threading.Tasks;
using UnityEngine;

public class CarSpawner : TriggerArea
{
	public bool Spawner;
	public float CarRotationY;

	private void Start()
	{
		if (Spawner)
			_ = SpawnCars();
	}

	private async UniTask SpawnCars()
	{
		while (true)
		{
			int waitTime = Random.Range(2000, 5000);

			for (int i = 0; i < 3; i++)
			{
				int burstTime = Random.Range(3000, 8000);

				var car = CarController.Instance.TryDequeueCar();
				if (car != null)
				{
					car.transform.position = transform.position;
					car.transform.rotation = Quaternion.Euler(0, CarRotationY, 0);
				}

				await UniTask.Delay(burstTime);
			}

			await UniTask.Delay(waitTime);
		}
	}

	protected override void OnObjectEnter(GameObject obj = null)
	{
		if (!Spawner && obj != null)
		{
			CarController.Instance.ReturnCar(obj);
		}
	}

	protected override void OnObjectExit(GameObject obj = null) { }
}
