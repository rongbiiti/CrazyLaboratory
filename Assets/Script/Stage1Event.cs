using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Stage1Event : MonoBehaviour {

    [SerializeField] private PlayerController _playercontroller;    // プレイヤーコントローラー
    [SerializeField] private CameraController _cameracontroller;    // カメラコントローラー
    [SerializeField] private Explodable _explodable;         // 通気口の破壊スクリプト
    [SerializeField, CustomLabel("通気口")] private Transform _vent;               // 通気口
    [SerializeField] private GameObject[] _enemys;          // 敵
    [SerializeField, CustomLabel("チュートリアル")] private GameObject _tutorial;          // チュートリアル
    [SerializeField, CustomLabel("銃入手直後待機時間")] private float _firstWaitTime = 0.5f;    // 銃入手直後待機時間　設定用
    [SerializeField, CustomLabel("通気口揺らす時間")] private float _ventShakeTime = 1.5f;     // 通気口が揺れる時間　設定用
    [SerializeField, CustomLabel("最初の敵出現後待機時間")] private float _enemySpawnAfterWaitTime = 0.5f;    // 最初の敵出現後待機時間　設定用
    [SerializeField, CustomLabel("カメラズーム時間")] private float _cameraZoomTime = 0.5f;    // カメラズーム時間　設定用
    [SerializeField, CustomLabel("敵出現間隔")] private float _enemyEnableTime = 1.5f;          // 敵出現間隔　設定用
    [SerializeField, CustomLabel("カメラズーム位置")] private Vector2 _cameraZoomPosition;    // カメラズーム位置
    [SerializeField, CustomLabel("カメラズームサイズ")] private float _cameraZoomSize = 8f;   // カメラズームサイズ
    private float ventshaketime;            // 通気口が揺れる時間　分岐用
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

        yield return new WaitForSeconds(_firstWaitTime);
        CameraZoomIn();

        yield return new WaitForSeconds(_cameraZoomTime);
        ventshaketime += _ventShakeTime;
        SoundManagerV2.Instance.PlaySE(42);
        Debug.Log("通気口ゆれる");

        yield return new WaitForSeconds(_ventShakeTime);
        VentBrake();
        StartCoroutine("EnemySetActive");

        yield return new WaitForSeconds(_enemySpawnAfterWaitTime);
        CameraZoomOut();
        PlayerControllerSetActive();
        TutorialActive();
    }

    private void PlayerStop()
    {
        _playercontroller.AnimStop();
        _playercontroller.IsEvent = true;
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

    private void VentBrake()
    {
        //_explodable.explode();
        //ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
        //ef.doExplosion(_explodable.gameObject.transform.position);
        Rigidbody2D rb = _vent.GetChild(0).GetComponent<Rigidbody2D>();
        rb.gravityScale = 2;
        rb.velocity = new Vector2(0, 6);
        _vent.GetChild(0).gameObject.AddComponent<AutoDestroy>().time = 5f;
        SoundManagerV2.Instance.PlaySE(8);
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
        _playercontroller.IsEvent = false;
        Debug.Log("プレイヤーコントローラーアクティブ");
    }

    private void TutorialActive()
    {
        _tutorial.SetActive(true);
        Debug.Log("チュートリアルアクティブ");
    }
}
