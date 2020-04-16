using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IShootable
{
	[Header("Enemy statistics")]
	// Health of the enemy
	[SerializeField] protected float _health = 10;
	// Range aw which the enemy will try to interact with the player
	[SerializeField] protected float _maxActionRange = 20;

	public float health { get { return _health; } }


	// Reference to the player
	protected GameObject _player = null;
	// Distance between the player and the enemy
	protected float _distanceWithPlayer = 0;


	private void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player");
		_distanceWithPlayer = Vector3.Distance(transform.position, _player.transform.position);

		StartCoroutine(InteractWithPlayer());
	}

	private void FixedUpdate()
	{
		// Updating distance between the player and the enemy
		_distanceWithPlayer = Vector3.Distance(transform.position, _player.transform.position);
	}

	protected IEnumerator InteractWithPlayer()
	{
		while (true)
		{
			if (_distanceWithPlayer < _maxActionRange)
			{
				RaycastHit hit;

				int layerMask = 1 << 8;
				layerMask = ~layerMask;

				if (Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, 10, layerMask))
					Debug.Log("Hit on layer " + hit.collider.gameObject.layer);
			}

			yield return new WaitForSeconds(.33f);
		}
	}

	public void Hit(float damageAmount)
	{
		// Don't try to do something if the enemy is already dead
		if (health > 0)
		{
			// Prevent from dealing negative damage
			if (damageAmount < 0)
				damageAmount = 0;

			// Updating healt
			_health -= damageAmount;

			// Triggering death if necessary
			if (health < 0)
				Die();
		}
	}

	public void Die()
	{
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
		transform.Rotate(Vector3.right, 90);
	}
}