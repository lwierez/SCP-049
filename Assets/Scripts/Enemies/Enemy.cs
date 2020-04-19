using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IShootable
{
	[Header("Enemy statistics")]
	// Health of the enemy
	[SerializeField] protected float _health = 10;
	// Range aw which the enemy will try to interact with the player
	[SerializeField] protected float _maxActionRange = 20;
	// Damage dealt in one hit
	[SerializeField] protected int _damageAmount = 6;
	// Cooldown between each hit
	[SerializeField] protected float _attackSpeed = .4f;

	public float health { get { return _health; } }


	// Reference to the player
	protected GameObject _player = null;
	// Is the player in sight
	protected bool _isChasingPlayer = false;
	// Can the enemy attack
	protected bool _canAttack = true;
	// Distance between the player and the enemy
	protected float _distanceWithPlayer = 0;


	// Reference to the navmesh agent
	private NavMeshAgent _navMeshAgent = null;


	private void Start()
	{
		// Initialize player references
		_player = GameObject.FindGameObjectWithTag("Player");
		_distanceWithPlayer = Vector3.Distance(transform.position, _player.transform.position);

		// Initialize navmesh agent reference
		_navMeshAgent = GetComponent<NavMeshAgent>();

		// Starting to look for the player
		StartCoroutine(InteractWithPlayer());
	}

	private void FixedUpdate()
	{
		// Updating distance between the player and the enemy
		_distanceWithPlayer = Vector3.Distance(transform.position, _player.transform.position);

		// Chasing player
		ChasePlayer();
	}

	protected IEnumerator InteractWithPlayer()
	{
		// Infinite loop
		while (true)
		{
			// If the player is within the action range
			if (_distanceWithPlayer < _maxActionRange)
			{
				RaycastHit hit;

				int layerMask = 1 << 8;
				layerMask = ~layerMask;

				// A raycast will determine if it is possible to directly see the player
				if (Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, 50, layerMask))
					if (hit.collider.gameObject.layer == 9)
						_isChasingPlayer = true;
			}

			yield return new WaitForSeconds(.33f);
		}
	}

	protected void ChasePlayer()
	{
		if (_isChasingPlayer && _navMeshAgent.enabled)
		{
			_navMeshAgent.destination = _player.transform.position;
		}

		if (_distanceWithPlayer < 1.6 && _canAttack)
		{
			_player.GetComponent<PlayerController>()?.Hit(_damageAmount);
			StartCoroutine(StartAttackCooldown());
		}
	}

	protected IEnumerator StartAttackCooldown()
	{
		_canAttack = false;
		yield return new WaitForSeconds(.5f);
		_canAttack = true;
	}

	public void TriggerDetection()
	{
		_isChasingPlayer = true;
	}

	public void Hit(float damageAmount)
	{
		// Don't try to do something if the enemy is already dead
		if (health > 0)
		{
			// Prevent from dealing negative damage
			if (damageAmount < 0)
				damageAmount = 0;

			// Updating health
			_health -= damageAmount;

			// Triggering death if necessary
			if (health < 0)
				Die();
		}
	}

	public void Die()
	{
		_navMeshAgent.enabled = false;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
		transform.Rotate(Vector3.right, 90);
		this.enabled = false;
	}
}