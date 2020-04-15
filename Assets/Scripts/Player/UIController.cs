using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] private Text _ammoText = null;                                                 // Text component that indicate remaining ammos
	private Animator _ammoTextAnimator;                                                             // Animator of the ammo text component
	private Color _ammoBaseColor;																	// Base color of the ammo text component
	[SerializeField] private Text _selectiveFireText = null;                                        // Text component that indicate the selective fire position
	private string _selectiveFireTextBase;                                                          // Base text for the firemode text component

	private void Start()
	{
		// Base properties registering
		_ammoBaseColor = _ammoText.color;
		_ammoTextAnimator = _ammoText.GetComponent<Animator>();
		_selectiveFireTextBase = _selectiveFireText.text;
	}


	public void UpdateAmmoText(int newValue)
	{
		_ammoText.text = newValue.ToString();
		if (newValue == 0)
			_ammoText.color = Color.red;
	}

	public void StartReload()
	{
		_ammoText.text = "--";
		_ammoText.color = Color.red;
		_ammoTextAnimator.SetBool("isReloading", true);
	}

	public void EndReload(int newValue)
	{
		UpdateAmmoText(newValue);
		_ammoText.color = _ammoBaseColor;
		_ammoTextAnimator.SetBool("isReloading", false);
	}


	public void UpdateSelectiveFireText(string newValue)
	{
		_selectiveFireText.text = _selectiveFireTextBase + newValue;
	}
}
