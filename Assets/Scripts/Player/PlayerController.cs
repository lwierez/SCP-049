using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
	[Header("Player statistics")]
	[SerializeField] private float _speed = 3f;														// Movement speed
	[SerializeField] private float _sprintSpeed = 5f;												// Movement speed when sprinting									  
	[SerializeField] private Vector2 _cameraSensivity = new Vector2(90, -60);                       // X and Y sensivity in the camera
	[SerializeField] private int _magSize = 30;                                                     // Size of the magazine of the player's weapon
	[SerializeField] private float _weaponDamage = 6f;												// Damage the player deal in one shot
	[SerializeField] private float _reloadTime = 0.8f;												// Time to load a new magazine
	[SerializeField] private float _shotCooldown = 0.10f;                                           // Cooldown between each shot
	[SerializeField] private float _recoilStrength = 2.5f;											// Strenght applied to the weapon when firing

	private int _remainingAmmo;																		// Current ammo stored in the player magazine
	private int remainingAmmo
	{
		get
		{
			return _remainingAmmo;
		}
		set
		{
			_remainingAmmo = value;
			_uiController.UpdateAmmoText(_remainingAmmo);
		}
	}


	[Header("Player components")]
	[SerializeField] private Transform _muzzleTransform = null;                                     // Start position for shots
	[SerializeField] private Transform _laserLight = null;											// Light that indicate the impact of a shot

	private Camera _attachedCamera;                                                                 // Camera component
	private float _cameraAngle = 0;																		// Camera component x angle since the begining of the simulation
	private UIController _uiController;																// Controller of the UI component
	private GameObject _flashlight;                                                                 // Flashlight

	// Shooting mechanics
	private bool _canShoot = true;                                                                  // Can the player shoot
	private bool _isReloading = false;																// Is the player reloading
	private bool _isSelectiveFireOnSemi = true;														// Is the player on semi mod, else he's to be considered on auto
	private bool isSelectiveFireOnSemi
	{
		get 
		{
			return _isSelectiveFireOnSemi;
		}
		set
		{
			_isSelectiveFireOnSemi = value;
			if (_isSelectiveFireOnSemi)
				_uiController.UpdateSelectiveFireText("Semi");
			else
				_uiController.UpdateSelectiveFireText("Auto");
		}
	}


	private void Start()
	{
		// Camera configuration
		_attachedCamera = GetComponentInChildren<Camera>();
		Cursor.lockState = CursorLockMode.Locked;

		// Flashlight configuration
		_flashlight = GetComponentInChildren<Light>().gameObject;
		_flashlight.SetActive(false);

		// UI initialization
		_uiController = GetComponentInChildren<UIController>();
		if (isSelectiveFireOnSemi)
			_uiController.UpdateSelectiveFireText("Semi");
		else
			_uiController.UpdateSelectiveFireText("Auto");

		// Magazine initialization
		remainingAmmo = _magSize;
	}

	private void Update()
	{
		// Controls
		if (Input.GetKeyDown(KeyCode.L)) // flashlight
			_flashlight.SetActive(!_flashlight.activeSelf);
		if (Input.GetKeyDown(KeyCode.F)) // selective fire
			isSelectiveFireOnSemi = !isSelectiveFireOnSemi;
		if (Input.GetKeyDown(KeyCode.R)) // reload
			StartCoroutine(Reload());
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
		float movementValue = Input.GetAxis("Mouse Y") * _cameraSensivity.y * Time.fixedDeltaTime;
		if (_cameraAngle + movementValue < 35 && 
			(_cameraAngle + movementValue > -40 || movementValue > 0))
		{
			_attachedCamera.transform.Rotate(Vector3.right, movementValue);
			_cameraAngle += movementValue;
		}

		RaycastHit hitInfo;
		if (Physics.Raycast(_muzzleTransform.position, _muzzleTransform.forward, out hitInfo, 100f))
		{
			_laserLight.position = hitInfo.point;
			_laserLight.position = Vector3.MoveTowards(_laserLight.position, _muzzleTransform.position, .01f);
		}

		// Shooting functions
		if ((isSelectiveFireOnSemi ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0)) && _canShoot && _remainingAmmo > 0)
			StartCoroutine(Shoot());
	}

	private IEnumerator Shoot()
	{
		// Forbid a next shot until the cooldown is over
		_canShoot = false;
		remainingAmmo--;

		// Use a raycast to know if something is hit
		RaycastHit hitInfo;
		int layerMask = 1 << 8;
		if (Physics.Raycast(_muzzleTransform.position, _muzzleTransform.forward, out hitInfo, 100f, layerMask))
		{
			hitInfo.collider.gameObject.GetComponent<IShootable>()?.Hit(_weaponDamage);
		}

		// Recoil effect
		transform.Rotate(Vector3.up, Random.Range(_recoilStrength/-2, _recoilStrength/2));
		float recoilValue = Random.Range(-_recoilStrength, 0);
		Debug.Log(_cameraAngle.ToString());
		if (_cameraAngle + recoilValue > -40)
		{
			_attachedCamera.transform.Rotate(Vector3.right, recoilValue);
			_cameraAngle += recoilValue;
		}

		// Sound effect
		GetComponentInChildren<WeaponNoise>().TriggerNoise();

		// Muzzle flash effect
		_muzzleTransform.gameObject.SetActive(true);
		yield return new WaitForSeconds(_shotCooldown/2);
		_muzzleTransform.gameObject.SetActive(false);

		// Wait for the cooldown and allow fire if the player is not reloading
		yield return new WaitForSeconds(_shotCooldown/2);
		_canShoot = true && !_isReloading;
	}

	private IEnumerator Reload()
	{
		// Registering reload action, forbid firing action
		_isReloading = true;
		_canShoot = false;
		// Starting reload animation on ui and wait for the reload time
		_uiController.StartReload();
		yield return new WaitForSeconds(_reloadTime);

		// Filling the mag
		remainingAmmo = _magSize;

		// Ending animation on UI, and unlock firing action
		_uiController.EndReload(remainingAmmo);
		_isReloading = false;
		_canShoot = true;
	}
}