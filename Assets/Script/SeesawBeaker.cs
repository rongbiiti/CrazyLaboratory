using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeesawBeaker : MonoBehaviour {

	private GameObject inAcid;
	private Vector3 inAcidStartScale;
	private Vector3 increaseScale;

	[HideInInspector, CustomLabel("酸が溜まる量")] private int _acidCollectMax = 4;

	private int intoAcidCount = 0;

	public int IntoAcidCount
	{
		get { return intoAcidCount; }
	}

	private void Awake()
	{
		inAcid = transform.GetChild(0).gameObject;
		inAcidStartScale = inAcid.transform.localScale;
		increaseScale = new Vector3(0,inAcidStartScale.y / _acidCollectMax,0);
		inAcid.transform.localScale = new Vector3(1,0,1);
	}

	void Start ()
	{
	}

	private void FixedUpdate()
	{
		
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("AcidFlask") && intoAcidCount < _acidCollectMax)
		{
			++intoAcidCount;
			inAcid.transform.localScale += increaseScale;
		}
	}
	

}
