using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{

	private PlayerController pc;
	private CameraController cc;
	private bool isPlayerEnter = false;

	void Start ()
	{
		pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		cc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
	}

	private void FixedUpdate()
	{
		if (isPlayerEnter)
		{
			transform.Translate(new Vector3(0, 0.1f, 0));
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			StartCoroutine("PlayerEnter");
		}
	}

	private IEnumerator PlayerEnter()
	{
		pc.AnimStop();
		pc.enabled = false;
		yield return new WaitForSeconds(1f);
		cc.enabled = false;
		isPlayerEnter = true;
		yield return  new WaitForSeconds(1.5f);
		if (SceneManager.GetActiveScene().name == "Stage2")
		{
			FadeManager.Instance.LoadScene("Stage3", 1.5f);
		}
		else
		{
			FadeManager.Instance.LoadScene("Stage2", 1f);
		}
		SaveManager.Instance.Save(pc.Hp, 2);
	}
}
