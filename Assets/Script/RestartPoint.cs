using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartPoint : MonoBehaviour {
	
	[SerializeField] private GameObject[] enemySpawner;
	private bool isPlayerReached = false;

	void Start () {
		
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerReached = true;
			gameObject.SetActive(false);
		}
	}

	public void TurnOnSpawner()
	{
		if (!isPlayerReached)
		{
			for(int i = 0; i < enemySpawner.Length; i++)
			{
				enemySpawner[i].SetActive(true);
			}
		}
	}
	
}
