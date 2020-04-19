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
	[SerializeField] private Text _healthBarText = null;                                            // Text that indicate the amount of health of the player
	private int _maxHealth = 10;                                                                    // Storage of the max health
	private int _currentHealth = 10;																// Current health of the player
	[SerializeField] private Slider _healthBarSlider = null;                                        // Slider of the health bar
	[SerializeField] private Animator _damageEffectAnimator = null;									// Animator component for the image of the damage effect

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


	public void SetMaxHealth(int health)
	{
		_maxHealth = health;
		ReloadHealthUI();
	}

	public void SetCurrentHealth(int health)
	{
		_currentHealth = health;
		ReloadHealthUI();
	}

	public void TriggerDamageEffet(bool dead = false)
	{
		if (dead)
			_damageEffectAnimator.SetTrigger("TriggerDeath");
		else
			_damageEffectAnimator.SetTrigger("TriggerAnim");
	}

	private void ReloadHealthUI()
	{
		_healthBarText.text = _currentHealth + " / " + _maxHealth;
		_healthBarSlider.value = _currentHealth == 0 ? 0 : (float)_currentHealth / _maxHealth;
	}
}
