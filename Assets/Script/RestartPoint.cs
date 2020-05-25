using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartPoint : MonoBehaviour {
	
	[SerializeField, CustomLabel("落ちるガレキプレハブ")] private GameObject _fallBlockPrefab;
	[SerializeField, CustomLabel("回復薬入り箱プレハブ")] private GameObject _fallMedkitPrefab;
	[SerializeField, CustomLabel("落ちる橋プレハブ1")] private GameObject _fallBridgePrefab1;
    [SerializeField, CustomLabel("落ちる橋プレハブ2")] private GameObject _fallBridgePrefab2;
    [SerializeField, CustomLabel("回復薬プレハブ")] private GameObject _medkitPrefab;
	[SerializeField, CustomLabel("酸の容器プレハブ1")] private GameObject _beakerPrefab1;
    [SerializeField, CustomLabel("酸の容器プレハブ2")] private GameObject _beakerPrefab2;

    [SerializeField, CustomLabel("敵スポナー")] private GameObject[] _enemySpawner;
	
	[SerializeField, CustomLabel("ステージ上の落ちるガレキ")] private GameObject[] _fallBlockTransform;
	[SerializeField, CustomLabel("ステージ上の回復薬入り箱")] private GameObject[] _fallMedkitTransform;
	[SerializeField, CustomLabel("ステージ上の落ちる橋1")] private GameObject _fallBridgeTransform1;
    [SerializeField, CustomLabel("ステージ上の落ちる橋2")] private GameObject _fallBridgeTransform2;
    [SerializeField, CustomLabel("ステージ上の回復薬")] private GameObject[] _medkitTransform;
	[SerializeField, CustomLabel("ステージ上の酸の容器1")] private GameObject _beakerTransform1;
    [SerializeField, CustomLabel("ステージ上の酸の容器2")] private GameObject _beakerTransform2;

    private GameObject[] fallBlockStartTrf;
	private GameObject[] fallMedkitStartTrf;
	private GameObject fallBridgeStartTrf1;
    private GameObject fallBridgeStartTrf2;
    private GameObject[] medkitStartTrf;
	private GameObject beakerStartTrf1;
    private GameObject beakerStartTrf2;

    private bool isPlayerReached;

	void Start ()
	{
		fallBlockStartTrf = _fallBlockTransform;
		fallMedkitStartTrf = _fallMedkitTransform;
		fallBridgeStartTrf1 = _fallBridgeTransform1;
        fallBridgeStartTrf2 = _fallBridgeTransform2;
        medkitStartTrf = _medkitTransform;
		beakerStartTrf1 = _beakerTransform1;
        beakerStartTrf2 = _beakerTransform2;
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
		if (isPlayerReached) return;

        int i = 0;

        foreach (var spawner in _enemySpawner)
		{
            spawner.SetActive(true);
		}

        // 吊るされたガレキ
		foreach (var obj in fallBlockStartTrf)  // 吊るされたガレキ
        {
			Destroy(_fallBlockTransform[i].gameObject);
            _fallBlockTransform[i++] = Instantiate(_fallBlockPrefab, obj.transform.position, obj.transform.rotation);
		}
		i = 0;

        // 吊るされた回復箱
        foreach (var obj in fallMedkitStartTrf)  // 吊るされた回復箱
        {
			Destroy(_fallMedkitTransform[i].gameObject);
            _fallMedkitTransform[i++] = Instantiate(_fallMedkitPrefab, obj.transform.position, obj.transform.rotation);
		}
		Debug.Log(gameObject.name + "吊るされたガレキと吊るされた回復箱再生完了");
		i = 0;

        // 落ちる橋1
        if(_fallBridgeTransform1 != null) {
            Destroy(_fallBridgeTransform1.gameObject);
            _fallBridgeTransform1 = Instantiate(_fallBridgePrefab1, fallBridgeStartTrf1.transform.position, fallBridgeStartTrf1.transform.rotation);
        }

        if (_fallBridgeTransform2 != null) {
            // 落ちる橋2
            Destroy(_fallBridgeTransform2.gameObject);
            _fallBridgeTransform2 = Instantiate(_fallBridgePrefab2, fallBridgeStartTrf2.transform.position, fallBridgeStartTrf2.transform.rotation);
        }

		Debug.Log(gameObject.name + "落ちる橋再生完了");

        // 床に置いた回復薬
		foreach (var obj in medkitStartTrf) // 床に置いた回復薬
        {
			Destroy(_medkitTransform[i]);
            _medkitTransform[i++] = Instantiate(_medkitPrefab, obj.transform.position, obj.transform.rotation);
		}
		Debug.Log(gameObject.name + "回復薬再生完了");
		i = 0;

        if (_beakerTransform1 != null) {
            // ビーカー1
            Destroy(_beakerTransform1);
            _beakerTransform1 = Instantiate(_beakerPrefab1, beakerStartTrf1.transform.position, beakerStartTrf1.transform.rotation);
        }


        if (_beakerTransform2 != null) {
            // ビーカー2
            Destroy(_beakerTransform2);
            _beakerTransform2 = Instantiate(_beakerPrefab2, beakerStartTrf2.transform.position, beakerStartTrf2.transform.rotation);
        }

        Debug.Log(gameObject.name + "酸の容器再生完了");

        fallBlockStartTrf = _fallBlockTransform;
        fallMedkitStartTrf = _fallMedkitTransform;
        fallBridgeStartTrf1 = _fallBridgeTransform1;
        fallBridgeStartTrf2 = _fallBridgeTransform2;
        medkitStartTrf = _medkitTransform;
        beakerStartTrf1 = _beakerTransform1;
        beakerStartTrf2 = _beakerTransform2;

    }
	
}
