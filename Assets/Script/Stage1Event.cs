using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Stage1Event : MonoBehaviour {

    [SerializeField] private PlayerController _playercontroller;    // プレイヤーコントローラー
    [SerializeField] private CameraController _cameracontroller;    // カメラコントローラー
    [SerializeField] private Explodable _explodable;         // 通気口の破壊スクリプト
    [SerializeField] private Transform _vent;               // 通気口
    [SerializeField] private GameObject[] _enemys;          // 敵
    [SerializeField] private GameObject _tutorial;          // チュートリアル
    [SerializeField, CustomLabel("通気口揺らす時間")] private float _ventShakeTime;     // 通気口が揺れる時間　設定用
    [SerializeField, CustomLabel("カメラズーム時間")] private float _cameraZoomTime;    // カメラズーム時間　設定用
    [SerializeField, CustomLabel("敵出現間隔")] private float _enemyEnableTime;          // 敵出現間隔　設定用
    [SerializeField, CustomLabel("カメラズーム位置")] private Vector2 _cameraZoomPosition;    // カメラズーム位置
    [SerializeField, CustomLabel("カメラズームサイズ")] private float _cameraZoomSize;   // カメラズームサイズ
    private float ventshaketime;            // 通気口が揺れる時間　分岐用
    private float camerazoomtime;           // カメラズーム時間　分岐用
    private float enemyenabletime;          // 敵出現間隔　分岐用
    private int enemycount;                 // 出現した敵カウント
    private BoxCollider2D boxCollider;      // このオブジェクトーのコライダー
    private Vector3 ventstartposition;      // 通気口初期位置
    private bool isShakeVent;               // 通気口を揺らしたか。

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        // 開始時にチュートリアルを非アクティブにする
        _tutorial.SetActive(false);
        ventstartposition = _vent.position;
    }

    private void FixedUpdate()
    {
        if(0 <= ventshaketime) {
            ventshaketime -= Time.deltaTime;
            if (isShakeVent) VentRestore();
            else VentShake();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            boxCollider.enabled = false;
            StartCoroutine("Event");
            Debug.Log("イベント開始");
        }
    }

    private IEnumerator Event()
    {
        PlayerStop();
        yield return new WaitForSeconds(0.5f);
        CameraZoomIn();
        yield return new WaitForSeconds(_cameraZoomTime);
        ventshaketime += _ventShakeTime;
        SoundManagerV2.Instance.PlaySE(42);
        Debug.Log("通気口ゆれる");
        yield return new WaitForSeconds(_ventShakeTime);
        _explodable.explode();
        ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
        ef.doExplosion(_explodable.gameObject.transform.position);
        SoundManagerV2.Instance.PlaySE(8);
        StartCoroutine("EnemySetActive");
        CameraZoomOut();
        yield return new WaitForSeconds(0.5f);
        PlayerControllerSetActive();
        TutorialActive();
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

    private void VentShake()
    {
        _vent.position += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        isShakeVent = true;
    }

    private void VentRestore()
    {
        _vent.position = ventstartposition;
        isShakeVent = false;
    }

    private IEnumerator EnemySetActive()
    {
        _enemys[enemycount++].SetActive(true);
        SoundManagerV2.Instance.PlaySE(25);
        Debug.Log("敵" + enemycount + "体目スポーン");
        yield return new WaitForSeconds(_enemyEnableTime);
        if (enemycount < _enemys.Length) {
            StartCoroutine("EnemySetActive");
        }
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

    private void TutorialActive()
    {
        _tutorial.SetActive(true);
        Debug.Log("チュートリアルアクティブ");
    }
}
