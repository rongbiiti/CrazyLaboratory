using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartPoint : MonoBehaviour {

    [SerializeField, CustomLabel("番号")] private int _num;

    private void Start()
    {
        if(_num <= ScoreManager.Instance.RestartPosNum) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
            other.GetComponent<PlayerController>().Stage1RestartValueSave();
            ScoreManager.Instance.S1_RestartPos = transform.position;
            ScoreManager.Instance.S1_RestartCamPos = new Vector3(transform.position.x, transform.position.y + 6.85f, -10f);
            ScoreManager.Instance.RestartPosNum = _num;
            gameObject.SetActive(false);
		}
	}
	
}
