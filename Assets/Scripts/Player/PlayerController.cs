using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
	[Header("Player statistics")]
	[SerializeField] private float _speed = 2.5f;													// Movement speed
	[SerializeField] private float _sprintSpeed = 5f;												// Movement speed when sprinting									  
	[SerializeField] private Vector2 _cameraSensivity = new Vector2(90, -60);                       // X and Y sensivity in the camera
	[SerializeField] private float _shotCooldown = 0.20f;                                           // Cooldown between each shot


	[Header("Player components")]
	[SerializeField] private Transform _muzzleTransform;											// Start position for shots

	private Camera _attachedCamera;																	// Camera component
	private GameObject _flashlight;                                                                 // Flashlight


	// Shooting mechanics
	private bool _canShoot = true;																	// Can the player shoot
	private bool _isSelectiveFireOnSemi = true;														// Is the player on semi mod, else he's to be considered on auto


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
		// Controls
		if (Input.GetKeyDown(KeyCode.L))
			_flashlight.SetActive(!_flashlight.activeSelf);
		if (Input.GetKeyDown(KeyCode.F))
			_isSelectiveFireOnSemi = !_isSelectiveFireOnSemi;
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

		// Shooting functions
		if ((_isSelectiveFireOnSemi ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0)) && _canShoot)
			StartCoroutine(Shoot());
	}

	private IEnumerator Shoot()
	{
		_canShoot = false;

		RaycastHit hitInfo;
		int layerMask = 1 << 8;
		if (Physics.Raycast(_muzzleTransform.position, _muzzleTransform.forward, out hitInfo, 100f, layerMask))
			Debug.Log("Enemy shot");

		yield return new WaitForSeconds(_shotCooldown);
		_canShoot = true;
	}
}