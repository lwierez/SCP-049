using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponNoise : MonoBehaviour
{
	[SerializeField]
	private SphereCollider _collider = null;

	public void TriggerNoise()
	{
		StartCoroutine(TriggerCollider());
	}

	private IEnumerator TriggerCollider()
	{
		_collider.enabled = true;
		yield return new WaitForSeconds(.5f);
		_collider.enabled = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		other.GetComponent<Enemy>()?.TriggerDetection();
	}
}
