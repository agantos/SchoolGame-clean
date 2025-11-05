using System.Collections.Generic;
using UnityEngine;

public class CarMovingArea : TriggerArea
{
	private List<GameObject> cars;
	public bool positive = false;
	public float step = 1f;

	public bool movesAtXAxis;
	public float RotationOfObjectToMove;

	private void Start()
	{
		cars = new List<GameObject>();
	}

	private void FixedUpdate()
	{
		int sign = positive ? 1 : -1;

		// Clean up destroyed cars (null entries)
		cars.RemoveAll(c => c.activeSelf == false);

		Vector3 movement = movesAtXAxis ? sign * new Vector3(1 * step * Time.fixedDeltaTime, 0f, 0f) : sign * new Vector3(0f, 0f, 1 * step * Time.fixedDeltaTime);

		foreach (GameObject c in cars) {
			if (c != null && c.activeSelf && (Mathf.Abs(c.transform.rotation.eulerAngles.y - RotationOfObjectToMove) < 0.1f)) {
				c.transform.position += movement;  
			}
		}
	}

	protected override void OnObjectEnter(GameObject obj = null)
	{
		if (obj != null) cars.Add(obj);
	}

	protected override void OnObjectExit(GameObject obj = null)
	{		
		if(obj != null) cars.Remove(obj);
	}
}
