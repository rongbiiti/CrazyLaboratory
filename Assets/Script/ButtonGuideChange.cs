using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGuideChange : MonoBehaviour
{

	[SerializeField] GameObject buttonGuide;
	private InputManager im;

	void Start ()
	{
		im = InputManager.Instance;
	}
	
	void Update () {
		if (im.ControlTypeChange == 1)
		{
			gameObject.SetActive(false);
		}
		
	}

	private void OnDisable()
	{
		buttonGuide.SetActive(true);
	}
}
