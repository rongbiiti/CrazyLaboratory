using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeesawStand : MonoBehaviour
{
	private SeesawBeaker sb;
	private float startRot;
	private float endRot;
	private float targetRot;

	void Start ()
	{
		Transform transform1;
		sb = (transform1 = transform).GetChild(0).transform.GetChild(0).GetComponent<SeesawBeaker>();
		startRot = transform1.eulerAngles.z;
		endRot = -startRot;
		targetRot = startRot;
	}

	private void FixedUpdate()
	{
		var rota = Quaternion.Euler(0,0,targetRot);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rota, 1f);
	}
	
	private void OnTriggerStay2D(Collider2D other)
	{
		if (!other.CompareTag("Player")) return;
		if (sb.IntoAcidCount == 0 || sb.IntoAcidCount == 1)
		{
			targetRot = endRot;
			
		} else if (sb.IntoAcidCount == 2|| sb.IntoAcidCount == 3)
		{
			targetRot = 0f;
		} else if (sb.IntoAcidCount == 4)
		{
			targetRot = startRot;
		}
		
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			targetRot = startRot;
		}
	}
}
