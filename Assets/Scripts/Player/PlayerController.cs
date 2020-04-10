using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
	[Header("PlayerStatistics")]
	// Movement speed
	[SerializeField] private float _speed = 2.5f;
	// Movement speed when sprinting
	[SerializeField] private float _sprintSpeed = 5f;
	// X and Y sensivity in the camera
	[SerializeField] private Vector2 _cameraSensivity = new Vector2(90, -60);

	// Camera component
	private Camera _attachedCamera;
	// Flashlight
	private GameObject _flashlight;

	private void Start()
	{
		// Camera configuration
		_attachedCamera = GetComponentInChildren<Camera>();
		Cursor.lockState = CursorLockMode.Locked;

		// Flashlight configuration
		_flashlight = GetComponentInChildren<Light>().gameObject;
		_flashlight.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
			_flashlight.SetActive(!_flashlight.activeSelf);
	}

	private void FixedUpdate()
	{
		// Movement
		transform.Translate(
			(Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal")).normalized
			* (Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _speed)
			* Time.fixedDeltaTime);
		// Rotation
		transform.Rotate(Vector3.up,
			Input.GetAxis("Mouse X") * _cameraSensivity.x * Time.fixedDeltaTime);
		// Camera movement
		_attachedCamera.transform.Rotate(Vector3.right,
			Input.GetAxis("Mouse Y") * _cameraSensivity.y * Time.fixedDeltaTime);
	}
}