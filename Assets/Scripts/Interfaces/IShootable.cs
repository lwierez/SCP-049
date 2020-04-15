using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShootable
{
	float health { get; }

	void Hit(float damageAmount);
	void Die();
}
