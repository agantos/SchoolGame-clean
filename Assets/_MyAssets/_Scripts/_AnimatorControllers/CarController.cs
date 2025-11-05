using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	public static CarController Instance { get; private set; }

	[Header("Car Pool Settings")]
	public List<GameObject> Cars; 
	public Queue<GameObject> carsQueue = new();

	private void Awake()
	{
		// Enforce singleton
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		// Initialize queue with inactive cars
		foreach (var car in Cars)
		{
			if (car != null)
			{
				car.SetActive(false);
				carsQueue.Enqueue(car);
			}
		}

		Debug.Log($"[CarController] Initialized with {carsQueue.Count} cars.");
	}

	public GameObject TryDequeueCar()
	{
		if (carsQueue.Count > 0)
		{
			var car = carsQueue.Dequeue();
			car.SetActive(true);
			return car;
		}

		Debug.LogWarning("[CarController] Car pool empty!");
		return null;
	}

	public void ReturnCar(GameObject car)
	{
		if (car == null) return;

		car.SetActive(false);
		carsQueue.Enqueue(car);
	}
}