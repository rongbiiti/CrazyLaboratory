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
        SoundManagerV2.Instance.PlaySE(40);
		pc.AnimStop();
		pc.enabled = false;
		yield return new WaitForSeconds(1f);
        SoundManagerV2.Instance.PlaySE(39);
        cc.enabled = false;
		isPlayerEnter = true;
		yield return  new WaitForSeconds(1.5f);
		if (SceneManager.GetActiveScene().name == "Stage2")
		{
			SaveManager.Instance.Save(pc.Hp, 3);
			ScoreManager.Instance.SceneName = SceneManager.GetActiveScene().name;
			FadeManager.Instance.LoadScene("HayatoScene_16", 1.5f);
		}
		else
		{
			SaveManager.Instance.Save(pc.Hp, 2);
			ScoreManager.Instance.SceneName = SceneManager.GetActiveScene().name;
			FadeManager.Instance.LoadScene("HayatoScene_16", 1f);
		}
		
	}
}
