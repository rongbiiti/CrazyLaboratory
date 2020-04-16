using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartPoint : MonoBehaviour {
	
	[SerializeField, CustomLabel("落ちるガレキプレハブ")] private GameObject _fallBlockPrefab;
	[SerializeField, CustomLabel("回復薬入り箱プレハブ")] private GameObject _fallMedkitPrefab;
	[SerializeField, CustomLabel("落ちる橋プレハブ")] private GameObject _fallBridgePrefab;
	[SerializeField, CustomLabel("回復薬プレハブ")] private GameObject _medkitPrefab;
	[SerializeField, CustomLabel("酸の容器プレハブ")] private GameObject _beakerPrefab;
	
	[SerializeField, CustomLabel("敵スポナー")] private GameObject[] _enemySpawner;
	
	[SerializeField, CustomLabel("落ちるガレキ")] private GameObject[] _fallBlockTransform;
	[SerializeField, CustomLabel("回復薬入り箱")] private GameObject[] _fallMedkitTransform;
	[SerializeField, CustomLabel("落ちる橋")] private GameObject[] _fallBridgeTransform;
	[SerializeField, CustomLabel("回復薬")] private GameObject[] _medkitTransform;
	[SerializeField, CustomLabel("酸の容器")] private GameObject[] _beakerTransform;

	private GameObject[] fallBlockStartTrf;
	private GameObject[] fallMedkitStartTrf;
	private GameObject[] fallBridgeStartTrf;
	private GameObject[] medkitStartTrf;
	private GameObject[] beakerStartTrf;
	private Quaternion[] beakerRotation;
	
	private bool isPlayerReached;

	void Start ()
	{
		fallBlockStartTrf = _fallBlockTransform;
		fallMedkitStartTrf = _fallMedkitTransform;
		fallBridgeStartTrf = _fallBridgeTransform;
		medkitStartTrf = _medkitTransform;
		beakerStartTrf = _beakerTransform;
		int i = 0;
		foreach (var rot in _beakerTransform)
		{
			beakerRotation[i++] = rot.transform.GetChild(0).transform.rotation;
		}
		
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

		foreach (var obj in fallBlockStartTrf)
		{
			Destroy(_fallBlockTransform[i++].gameObject);
			Instantiate(_fallBlockPrefab, obj.transform.position, obj.transform.rotation);
		}
		i = 0;
		
		foreach (var obj in fallMedkitStartTrf)
		{
			Destroy(_fallMedkitTransform[i++].gameObject);
			Instantiate(_fallMedkitPrefab, obj.transform.position, obj.transform.rotation);
		}
		Debug.Log("ガレキと回復箱再生完了");
		i = 0;

		foreach (var obj in fallBridgeStartTrf)
		{
			Destroy(_fallBridgeTransform[i++].gameObject);
			Instantiate(_fallBridgePrefab, obj.transform.position, obj.transform.rotation);
		}
		i = 0;
		Debug.Log("落ちる橋再生完了");

		foreach (var obj in medkitStartTrf)
		{
			Destroy(medkitStartTrf[i++]);
			Instantiate(_medkitPrefab, obj.transform.position, obj.transform.rotation);
		}
		Debug.Log("回復薬再生完了");
		i = 0;

		foreach (var obj in beakerStartTrf)
		{
			Destroy(beakerStartTrf[i]);
			Instantiate(_beakerPrefab, obj.transform.position, beakerRotation[i++]);
		}
		Debug.Log("酸の容器再生完了");
	}
	
}
