using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GunView view;

    private float rotationSpeed = 50.0f;
    private float maxLeftAngle = 85.0f;
    private float maxRightAngle = 275.0f;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		view.ready = true;
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	private void Update()
	{
		if (view.ready){
			float mouseAxisX = Input.GetAxis("Mouse X");
			transform.Rotate(Vector3.back * mouseAxisX * rotationSpeed * Time.deltaTime);
			if (transform.eulerAngles.z > maxLeftAngle && transform.eulerAngles.z < 180.0 ){
				transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y, maxLeftAngle);
			}
			if (transform.eulerAngles.z < maxRightAngle && transform.eulerAngles.z > 180.0 ){
				transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y, maxRightAngle);
			}
		}
	}

}
