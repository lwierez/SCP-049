using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("PlayerStatistics")]
	// Movement speed
	[SerializeField] private float _speed = 5f;
	// X and Y sensivity in the camera
	[SerializeField] private Vector2 _cameraSensivity = new Vector2(60, -60);

	private Camera _attachedCamera;

	private void Start()
	{
		// Camera configuration
		_attachedCamera = GetComponentInChildren<Camera>();
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void FixedUpdate()
	{
		// Movement
		transform.Translate(
			(Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal")).normalized
			* _speed * Time.fixedDeltaTime);
		// Rotation
		transform.Rotate(Vector3.up,
			Input.GetAxis("Mouse X") * _cameraSensivity.x * Time.fixedDeltaTime);
		// Camera movement
		_attachedCamera.transform.Rotate(Vector3.right,
			Input.GetAxis("Mouse Y") * _cameraSensivity.y * Time.fixedDeltaTime);
	}
}
