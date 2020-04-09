using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Kama : MonoBehaviour {


    [SerializeField] private float _HP = 10f;
    [SerializeField] private float _HitDamage = 2f;
    [SerializeField] private float _PlayerDamage = 3000f;
    [SerializeField] private float _PlayerDamageRate = 3f;
    [SerializeField] private float _nockBuckPower = 300f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
    [SerializeField] private float _collisionDisplacePosition;  //エネミーの中心から後ろに、いくつ離れるか。
    private float _PlayerDamageTime;
    [SerializeField] private float _moveSpeed = 30f;
    [SerializeField] private float _AttackRate = 1f; //攻撃前の硬直時間
    private float AttackTime;  
    [SerializeField] private float _afterAttackRate = 1f;    //攻撃後の硬直時間
    private float AfterAttackTime;    //攻撃後の時間の格納用
    [SerializeField] private float _stanTime = 3f;
    [SerializeField] private bool _directionChange = false;
    private float stanTimeRemain = 0;
    private float nowHP;
    private Vector3 PointA_Position;
    private Vector3 PointB_Position;
    private byte AttackPhase;
    private EnemyHpbar enemyHpbar;
    private bool isZeroHP;
    [SerializeField] private float _destroyTime = 2f;
    private Vector2 startScale;
    private Vector2 startposition;
    private int patrolType;     //0:パトロール 1:追尾 3:攻撃
    private GameObject playerObject;  //playerのオブジェクトを格納
    [SerializeField] private float _jumpRate = 2f;   //ジャンプの時間の入力
    private float JumpTime;    //ジャンプの時間を格納
    [SerializeField] private float _jumpPower = 2000f;  //ジャンプの力
    [SerializeField] private GameObject _AttackPosition;
    [SerializeField] private GameObject _WaitPosition;
    public Vector3 targetPosition;
    private bool isReachTargetPosition;
    private bool istargetPointA;
    Rigidbody2D rb;

    Animator animator;

    // Use this for initialization
    void Start()
    {
        
        startScale = transform.localScale;
        startposition = transform.position;
        AttackPhase = 0;
        nowHP = _HP;
        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, nowHP);
        patrolType = 0;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        targetPosition = _AttackPosition.transform.position;

        var po = transform.position;
        _collisionDisplacePosition = transform.position.x - _WaitPosition.transform.position.x;
        
        if (_directionChange == false)
        {
            _WaitPosition.transform.position = new Vector2(po.x + Mathf.Abs(_collisionDisplacePosition), po.y);
        }
        else
        {
            var scale = transform.localScale;
            transform.localScale = new Vector2(-scale.x, scale.y);
            _WaitPosition.transform.position = new Vector2(po.x - Mathf.Abs(_collisionDisplacePosition), po.y);
        }
        _AttackPosition.transform.parent = null;
        _AttackPosition.SetActive(false);
        _WaitPosition.transform.parent = null;
        _WaitPosition.SetActive(false);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();


    }

    void FixedUpdate()
    {
        if (isZeroHP)
        {
            if (0 < transform.localScale.x)
            {
                transform.localScale -= new Vector3(startScale.x / _destroyTime * Time.deltaTime, startScale.y / _destroyTime * Time.deltaTime);
            }
            else if (transform.localScale.x < 0)
            {
                transform.localScale -= new Vector3(-startScale.x / _destroyTime * Time.deltaTime, startScale.y / _destroyTime * Time.deltaTime);
            }
            if (Mathf.Abs(transform.localScale.x) <= startScale.x / 95)
            {
                Destroy(enemyHpbar.hpbar.gameObject);
                Destroy(gameObject);
            }

        }
        else
        {
            if (0 < _PlayerDamageTime)
            {
                _PlayerDamageTime -= Time.deltaTime;
            }

            if (0 < stanTimeRemain)
            {
                stanTimeRemain -= Time.deltaTime;
                if (stanTimeRemain <= 0)
                {
                    //animator.SetBool("Walk", true);
                    //animator.SetBool("Stun", false);
                }
            }
            if(0 < AttackTime)
            {
                AttackTime -= Time.deltaTime;
            }

            if (0 < AfterAttackTime)
            {
                AfterAttackTime -= Time.deltaTime;
            }

            if (0 < JumpTime)
            {
                JumpTime -= Time.deltaTime;
            }

            // transformを取得
            Transform myTransform = this.transform;

            switch (patrolType)
            {
                case 0:     //パトロールの動き

                    break;

                case 1: //攻撃

                    if(AttackTime > 0)  return; 

                    //DecideTargetPotision();
                    // 巡回ポイントの位置を取得
                    Vector2 targetPos = _AttackPosition.transform.position;
                    // 巡回ポイントのx座標
                    float x = targetPos.x;
                    // ENEMYは、地面を移動させるので座標は常に0とする
                    float y = 0;
                    // 移動を計算させるための２次元のベクトルを作る
                    Vector2 direction = new Vector2(x - transform.position.x, y).normalized;
                    // ENEMYのRigidbody2Dに移動速度を指定する
                    rb.velocity = direction * _moveSpeed;

                    if(JumpTime > 0)
                    {
                        Vector2 force = new Vector2(0, _jumpPower);

                        rb.AddForce(force);
                    }
                    
                    break;

                case 2: //戻る

                    if(AfterAttackTime >= 0) return;

                    //DecideTargetPotision();
                    // 巡回ポイントの位置を取得
                    targetPos = _WaitPosition.transform.position;
                    // 巡回ポイントのx座標
                    x = targetPos.x;
                    // ENEMYは、地面を移動させるので座標は常に0とする
                    y = 0;
                    // 移動を計算させるための２次元のベクトルを作る
                    direction = new Vector2(x - transform.position.x, y).normalized;
                    // ENEMYのRigidbody2Dに移動速度を指定する
                    rb.velocity = direction * _moveSpeed;

                    if (JumpTime > 0)
                    {
                        Vector2 force = new Vector2(0, _jumpPower);

                        rb.AddForce(force);
                    }

                    break;
            }


        }


    }

    private void DecideTargetPotision()
    {
        // まだ目的地についてなかったら（移動中なら）目的地を変えない
        if (!isReachTargetPosition)
        {
            return;
        }

        // 目的地に着いていたら目的地を再設定する
        if (istargetPointA)
        {
            targetPosition = _AttackPosition.transform.position;
            istargetPointA = false;
            _AttackPosition.SetActive(true);
            _AttackPosition.SetActive(false);
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y);
        }
        else
        {
            targetPosition = _WaitPosition.transform.position;
            istargetPointA = true;
            _AttackPosition.SetActive(false);
            _WaitPosition.SetActive(true);
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y);
        }
        isReachTargetPosition = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (patrolType == 0 && collision.CompareTag("Player"))
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            _AttackPosition.SetActive(true);
            _AttackPosition.transform.position = collision.transform.position;
            AttackTime = _AttackRate;
            JumpTime = _jumpRate;
            patrolType = 1; //攻撃モード
        }

        if (collision.CompareTag("PatrolPoint"))
        {
            //isReachTargetPosition = true;
            //DecideTargetPotision();
            

            if(patrolType == 1) //攻撃中にpatrolPointに当たった時に　もどる行動へ
            {
                _AttackPosition.SetActive(false);
                _WaitPosition.SetActive(true);
                rb.velocity = new Vector2(0f, 0f);
                AfterAttackTime = _afterAttackRate;
                JumpTime = _jumpRate;
                patrolType = 2;
            }

            if (patrolType == 2 && 0 >= AfterAttackTime) //戻ってる最中にpatrolpointに当たった時に　待機行動へ
            {
                _WaitPosition.SetActive(false);
                rb.velocity = new Vector2(0f, 0f);
                //transform.position = startposition;
                patrolType = 0;
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
        }


        

        // 弱点のみ、IsTriggerをオンにしている。
        if (collision.CompareTag("AcidFlask"))
        {
            nowHP -= _HitDamage;
            Debug.Log(gameObject.name + "の弱点にヒット");
            SoundManagerV2.Instance.PlaySE(4);
            enemyHpbar.SetBarValue(_HP, nowHP);
            if (nowHP <= 0)
            {
                isZeroHP = true;
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {


        if (collision.CompareTag("Player") && AttackPhase == 2 && _PlayerDamageTime <= 0 && stanTimeRemain <= 0)
        {
            _PlayerDamageTime += _PlayerDamageRate;
            collision.gameObject.GetComponent<PlayerController>().Damage(_PlayerDamage);
            Rigidbody2D prb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 targetPos = collision.gameObject.transform.position;
            float y = _nockBuckUpperPower;
            float x = targetPos.x;
            Vector2 direction = new Vector2(x - transform.position.x, y).normalized;
            prb.velocity = direction * _nockBuckPower;
            SoundManagerV2.Instance.PlaySE(2);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AcidFlask"))
        {
            Debug.Log(gameObject.name + "の非弱点にヒット");
            SoundManagerV2.Instance.PlaySE(7);

        }

        if (collision.gameObject.CompareTag("Gareki"))
        {
            //animator.SetBool("Walk", false);
            //animator.SetBool("Stun", true);

            stanTimeRemain += _stanTime;
            AttackPhase = 0;
            Debug.Log(gameObject.name + "にガレキがヒットしてスタンした");
            SoundManagerV2.Instance.PlaySE(3);
        }

        if (collision.gameObject.CompareTag("Player") && stanTimeRemain <= 0)
        {
            collision.gameObject.GetComponent<PlayerController>().Damage(_PlayerDamage);
            Rigidbody2D prb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 targetPos = collision.gameObject.transform.position;
            float y = _nockBuckUpperPower;
            float x = targetPos.x;
            Vector2 direction = new Vector2(x - transform.position.x, y).normalized;
            prb.velocity = direction * _nockBuckPower;
            SoundManagerV2.Instance.PlaySE(2);
        }
    }


}
