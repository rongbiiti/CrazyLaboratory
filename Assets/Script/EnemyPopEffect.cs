using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Rendering;

public class EnemyPopEffect : MonoBehaviour {

    [SerializeField, CustomLabel("出現エフェクトさせる")] private bool _isTruePopEffect;
    [SerializeField, CustomLabel("出現エフェクト")] private GameObject _popEffect;
    [SerializeField, CustomLabel("通気口")] private GameObject _tuukikou;
    [SerializeField, CustomLabel("破片プレハブ")] private GameObject _fragmentTuukikou;
    [SerializeField, CustomLabel("検知距離")] private float _distance = 1f;
    private GameObject instantiatedPopEffect;   // 生成した出現エフェクトを参照するための変数
    private bool isPopEffectInstantiated;
    private GameObject player;
    private Enemy_ChildSpiderAnimTest enemy_ChildSpiderAnimTest;
    private EnemyHpbar enemyHpbar;
    private CubismRenderController cubismRender;
    private Rigidbody2D rb;
    private Explodable explodable;
    private Vector3 fragmentPosition;

    private void OnEnable()
    {
        enemy_ChildSpiderAnimTest.enabled = false;
        
        cubismRender.Opacity = 0f;
        rb.Sleep();
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
            //instantiatedPopEffect = Instantiate(_popEffect, transform.position, Quaternion.identity);
            enemy_ChildSpiderAnimTest.enabled = true;
            enemyHpbar.hpbar.gameObject.SetActive(true);
            cubismRender.Opacity = 1f;
            rb.WakeUp();
            SoundManagerV2.Instance.PlaySE(8);
            isPopEffectInstantiated = true;
            explodable.explode();
            ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
            ef.doExplosion(transform.position);
        } else if (!isPopEffectInstantiated) {
            enemyHpbar.hpbar.gameObject.SetActive(false);
        }
    }


}
