using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RestartPoint : MonoBehaviour {
	
	
	[SerializeField, CustomLabel("落ちるガレキプレハブ")] private GameObject _fallBlockPrefab;
	[SerializeField, CustomLabel("落ちる橋プレハブ")] private GameObject _fallBridgePrefab;
	[SerializeField, CustomLabel("回復薬プレハブ")] private GameObject _medkitPrefab;
	[SerializeField, CustomLabel("酸の容器プレハブ")] private GameObject _beakerPrefab;
	
	[SerializeField, CustomLabel("敵スポナー")] private GameObject[] _enemySpawner;
	
	[SerializeField, CustomLabel("落ちるガレキ")] private GameObject[] _fallBlockTransform;
	[SerializeField, CustomLabel("落ちる橋")] private GameObject[] _fallBridgeTransform;
	[SerializeField, CustomLabel("回復薬")] private GameObject[] _medkitTransform;
	[SerializeField, CustomLabel("酸の容器")] private GameObject[] _beakerTransform;

	private GameObject[] fallBlockStartTrf;
	private GameObject[] fallBridgeStartTrf;
	private GameObject[] medkitStartTrf;
	private GameObject[] beakerStartTrf;
	
	private bool isPlayerReached = false;

	void Start ()
	{
		fallBlockStartTrf = _fallBlockTransform;
		fallBridgeStartTrf = _fallBridgeTransform;
		medkitStartTrf = _medkitTransform;
		beakerStartTrf = _beakerTransform;
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
		int i = 0;
		if (isPlayerReached) return;
		foreach (var t in _enemySpawner)
		{
			t.SetActive(true);
		}

		foreach (var fb in fallBlockStartTrf)
		{
			Destroy(_fallBlockTransform[i++].gameObject);
			Instantiate(_fallBlockPrefab, fb.transform.position, fb.transform.rotation);
		}
		Debug.Log("あ");
		i = 0;

		foreach (var fbr in fallBridgeStartTrf)
		{
			Destroy(_fallBridgeTransform[i++].gameObject);
			Instantiate(_fallBridgePrefab, fbr.transform.position, fbr.transform.rotation);
		}
		i = 0;
		Debug.Log("い");

		foreach (var mt in medkitStartTrf)
		{
			Destroy(medkitStartTrf[i++]);
			Instantiate(_medkitPrefab, mt.transform.position, mt.transform.rotation);
		}
		Debug.Log("う");
		i = 0;

		foreach (var bt in beakerStartTrf)
		{
			Destroy(beakerStartTrf[i++]);
			Instantiate(_beakerPrefab, bt.transform.position, bt.transform.rotation);
		}
		Debug.Log("え");
	}
	
}
