using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Rendering;

public class EnemyPopEffect : MonoBehaviour {

    [SerializeField, CustomLabel("出現エフェクトさせる")] private bool _isTruePopEffect;
    [SerializeField, CustomLabel("出現エフェクト")] private GameObject _popEffect;
    [SerializeField, CustomLabel("検知距離")] private float _distance = 1f;
    private GameObject instantiatedPopEffect;   // 生成した出現エフェクトを参照するための変数
    private bool isPopEffectInstantiated;
    private GameObject player;
    private Enemy_ChildSpiderAnimTest enemy_ChildSpiderAnimTest;
    private EnemyHpbar enemyHpbar;
    private CubismRenderController cubismRender;
    private Rigidbody2D rb;

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
	}

    private void FixedUpdate()
    {
        if(!isPopEffectInstantiated && Vector3.Distance(player.transform.position, transform.position) <= _distance) {
            instantiatedPopEffect = Instantiate(_popEffect, transform.position, Quaternion.identity);
            enemy_ChildSpiderAnimTest.enabled = true;
            enemyHpbar.hpbar.gameObject.SetActive(true);
            cubismRender.Opacity = 1f;
            rb.WakeUp();
            SoundManagerV2.Instance.PlaySE(8);
            isPopEffectInstantiated = true;
        } else if (!isPopEffectInstantiated) {
            enemyHpbar.hpbar.gameObject.SetActive(false);
        }
    }


}
