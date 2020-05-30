using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEvent : MonoBehaviour {

    [SerializeField] private PlayerController _playercontroller;    // プレイヤーコントローラー
    [SerializeField] private CameraController _cameracontroller;    // カメラコントローラー
    [SerializeField, CustomLabel("ズーム前の時間")] private float _firstWaitTime = 0.5f;    // ズーム前の待機時間　設定用
    [SerializeField, CustomLabel("カメラズーム時間")] private float _cameraZoomTime = 0.5f;    // カメラズーム時間　設定用
    [SerializeField, CustomLabel("カメラズーム位置")] private Vector2 _cameraZoomPosition;    // カメラズーム位置
    [SerializeField, CustomLabel("カメラズームサイズ")] private float _cameraZoomSize = 8f;   // カメラズームサイズ
    private BoxCollider2D boxCollider;      // このオブジェクトーのコライダー

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            boxCollider.enabled = false;
            StartCoroutine("Event");
            Debug.Log("イベント開始");
        }
    }

    private IEnumerator Event()
    {
        PlayerStop();

        yield return new WaitForSeconds(_firstWaitTime);
        CameraZoomIn();

        yield return new WaitForSeconds(_cameraZoomTime);
        CameraZoomOut();
        PlayerControllerSetActive();
    }

    private void PlayerStop()
    {
        _playercontroller.AnimStop();
        _playercontroller.enabled = false;
        Debug.Log("プレイヤーストップ");
    }

    private void CameraZoomIn()
    {
        _cameracontroller.EventCamera(_cameraZoomPosition, _cameraZoomSize, _cameraZoomTime);
        Debug.Log("カメラズームイン");
    }

    private void CameraZoomOut()
    {
        _cameracontroller.EventCameraEnd(_cameraZoomTime);
        Debug.Log("カメラズームアウト");
    }

    private void PlayerControllerSetActive()
    {
        _playercontroller.enabled = true;
        Debug.Log("プレイヤーコントローラーアクティブ");
    }
}
