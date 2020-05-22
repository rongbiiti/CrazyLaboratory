using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Rendering;

public class EnemyPopEffect : MonoBehaviour {

    [SerializeField, CustomLabel("出現エフェクトさせる")] private bool _isTruePopEffect;
    [SerializeField, CustomLabel("出現エフェクト")] private GameObject _popEffect;
    [SerializeField, CustomLabel("ステージ上の通気口")] private GameObject _tuukikou;
    [SerializeField, CustomLabel("破片プレハブ")] private GameObject _fragmentTuukikou;
    [SerializeField, CustomLabel("検知距離")] private float _distance = 1f;
    [SerializeField, CustomLabel("通気口揺らす時間")] private float _ventShakeTime = 1.5f;     // 通気口が揺れる時間　設定用
    private GameObject instantiatedPopEffect;   // 生成した出現エフェクトを参照するための変数
    private bool isPopEffectInstantiated;
    private GameObject player;
    private Enemy_ChildSpiderAnimTest enemy_ChildSpiderAnimTest;
    private EnemyHpbar enemyHpbar;
    private CubismRenderController cubismRender;
    private Rigidbody2D rb;
    private Explodable explodable;
    private Vector3 fragmentPosition;
    private float ventshaketime;            // 通気口が揺れる時間　分岐用
    private bool isShakeVent;               // 通気口を揺らしたか。
    private Vector3 ventstartposition;      // 通気口初期位置

    private void OnEnable()
    {
        enemy_ChildSpiderAnimTest.enabled = false;
        cubismRender.Opacity = 0f;
        rb.Sleep();
        enemy_ChildSpiderAnimTest.AllColliderDisable();
        if (isPopEffectInstantiated) {
            Destroy(instantiatedPopEffect);
        }
    }

    private void Awake()
    {
        if (!_isTruePopEffect) {
            enabled = false;
            return;
        }
        enemy_ChildSpiderAnimTest = GetComponent<Enemy_ChildSpiderAnimTest>();
        cubismRender = GetComponent<CubismRenderController>();
        enemyHpbar = GetComponent<EnemyHpbar>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start () {
        
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        explodable = _tuukikou.transform.GetChild(0).GetComponent<Explodable>();
        fragmentPosition = _tuukikou.transform.GetChild(0).transform.position;
    }

    private void FixedUpdate()
    {
        if(!isPopEffectInstantiated && Vector3.Distance(player.transform.position, transform.position) <= _distance) {
            StartCoroutine("VentBrake");
            
            
        } else if (!isPopEffectInstantiated) {
            enemyHpbar.hpbar.gameObject.SetActive(false);
        }

        if (0 <= ventshaketime) {
            ventshaketime -= Time.deltaTime;
            if (isShakeVent) VentRestore();
            else VentShake();
        }
    }

    private void VentShake()
    {
        _tuukikou.transform.GetChild(0).transform.position += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        isShakeVent = true;
    }

    private void VentRestore()
    {
        _tuukikou.transform.GetChild(0).transform.position = fragmentPosition;
        isShakeVent = false;
    }

    private IEnumerator VentBrake()
    {
        ventshaketime += _ventShakeTime;
        SoundManagerV2.Instance.PlaySE(42);
        isPopEffectInstantiated = true;
        yield return new WaitForSeconds(_ventShakeTime);
        //instantiatedPopEffect = Instantiate(_popEffect, transform.position, Quaternion.identity);
        SoundManagerV2.Instance.PlaySE(8);
        explodable.explode();
        ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
        ef.doExplosion(transform.position);
        enemy_ChildSpiderAnimTest.enabled = true;
        enemyHpbar.hpbar.gameObject.SetActive(true);
        cubismRender.Opacity = 1f;
        rb.WakeUp();
        enemy_ChildSpiderAnimTest.AllColliderEnable();
    }


}
