using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IShootable
{
	[Header("Enemy statistics")]
	// Health of the enemy
	[SerializeField] protected float _health = 10;

	public float health { get { return _health; } }

	public void Hit(float damageAmount)
	{
		if (damageAmount < 0)
			damageAmount = 0;

		_health -= damageAmount;

		if (health < 0)
			Die();
	}

	public void Die()
	{
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePosition;
		GetComponent<CapsuleCollider>().enabled = false;
		transform.Rotate(Vector3.right, 90);
	}
}