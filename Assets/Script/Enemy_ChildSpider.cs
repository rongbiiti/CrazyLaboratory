using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ChildSpider : MonoBehaviour {

    [SerializeField] private float _HP = 1f;
    [SerializeField] private float _HitDamage = 1f;
    [SerializeField] private float _PlayerDamage = 1000f;
    [SerializeField] private float _PlayerDamageRate = 3f;
    [SerializeField] private float _nockBuckPower = 150f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
    private float _PlayerDamageTime;
    [SerializeField] private float _MoveSpeed = 0.1f;
    //[SerializeField] private bool _directionChange;     //false:LEFT true:RIGHT
    private int _direction;
    [SerializeField] private byte _AttackWait = 1;
    [SerializeField] private float _AttackTime = 0.45f;
    [SerializeField] private float _stanTime = 3f;
    private float stanTimeRemain = 0;
    private float nowHP;

    [SerializeField] private GameObject[] _PatrolPoint;
    private Vector3[] PatrolPointPosition;
    private byte movetype;    //0：次のパトロール位置を取得する待ち    1:取得した後、硬直する 3：動く
    private int PointCount;    //パトロールポイントのカウント PatrolPointの配列の数が最大値
    [SerializeField] private float _pointWaitRate = 2f;      //パトロール後の硬直
    private float pointWaitTime;      //パトロール後の硬直

    private Vector3 Point_Position;     //パトロールポイントの座標の格納用

    //private Vector3 PointB_Position;
    private byte AttackPhase;
    private float Count;
    private EnemyHpbar enemyHpbar;
    private bool isZeroHP;
    [SerializeField] private float _destroyTime = 2f;
    private Vector2 startScale;
    private int patrolType;     //0:パトロール 1:追尾 3:攻撃
    [SerializeField] private float _tracking = 30f;     //エネミーの追跡範囲
    private GameObject playerObject;  //playerのオブジェクトを格納

    Animator animator;

    // Use this for initialization
    void Start()
    {
        startScale = transform.localScale;
        PatrolPointPosition = new Vector3[_PatrolPoint.Length];
        for (int i = 0; i < _PatrolPoint.Length; i++)
        {
            PatrolPointPosition[i] = _PatrolPoint[i].gameObject.transform.position;
            _PatrolPoint[i].SetActive(false);
        }
        Point_Position = PatrolPointPosition[PointCount];     //最初のパトロールポイントの座標を格納
        movetype = 2;
       
        AttackPhase = 0;
        Count = 0;
        nowHP = _HP;
        
        if (Point_Position.x <= gameObject.transform.position.x)
        {
            _direction = -1;     //左
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        }
        else
        {
            _direction = 1;    //右
            gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        }
        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, nowHP);
        playerObject = GameObject.FindGameObjectWithTag("Player");

        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (isZeroHP)
        {
            transform.localScale = startScale;
            nowHP = _HP;
            enemyHpbar.SetBarValue(_HP,nowHP);
            enemyHpbar.hpbar.gameObject.SetActive(true);
            isZeroHP = false;
            if (Point_Position.x <= gameObject.transform.position.x)
            {
                _direction = -1;     //左
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            }
            else
            {
                _direction = 1;    //右
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            }
        }
        
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
                gameObject.SetActive(false);
                
                enemyHpbar.hpbar.gameObject.SetActive(false);
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
            }

            if (0 < pointWaitTime)
            {
                pointWaitTime -= Time.deltaTime;
            }
           else if(movetype == 1 && 0 >= pointWaitTime)
            {
                movetype = 2;
                animator.SetBool("Walk", true);
                animator.SetBool("Stand", false);
                animator.SetBool("Stun", false);
            }

            if (movetype == 0)
            {
                animator.SetBool("Stand", true);
                animator.SetBool("Walk", false);
                animator.SetBool("Stun", false);

                if (++PointCount > _PatrolPoint.Length - 1) PointCount = 0;  //配列の最大数に到達したら0に戻す
                Point_Position = PatrolPointPosition[PointCount];     //パトロールポイントの座標を格納
                if (gameObject.transform.position.x >= Point_Position.x)     //現在のポジションからポイントの座標を見て　設定する
                {
                    if(_direction == 1) gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    _direction = -1; //左
                    
                }
                else if (gameObject.transform.position.x <= Point_Position.x)
                {
                    if (_direction == -1) gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    _direction = 1; //右
                }

                movetype = 1;     //硬直へ
                pointWaitTime += _pointWaitRate;
            }
            




            // transformを取得
            Transform myTransform = this.transform;

            switch (patrolType)
            {
                case 0:
                    if (AttackPhase == 0 && stanTimeRemain <= 0)        //パトロール中
                    {
                        if (movetype != 2) return;  //パトロールついて硬直中は動かない

                        // 現在の座標からのxyz を _MoveSpeed ずつ加算して移動
                        myTransform.Translate(_MoveSpeed * _direction, 0.0f, 0.0f, Space.World);

                        //パトロールポイントを超えたら待機タイプに変える
                        if (_direction == -1 && gameObject.transform.position.x <= Point_Position.x)
                        {            
                            movetype = 0;
                        }
                        else if (_direction == 1 && gameObject.transform.position.x >= Point_Position.x)
                        {
                            movetype = 0;
                        }

                    }
                    break;

                case 1:
                    if (playerObject.transform.position.x >= transform.position.x && _direction == -1 && AttackPhase == 0 && stanTimeRemain <= 0)
                    {
                        _direction *= -1;
                        gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    }
                    else if (playerObject.transform.position.x <= transform.position.x && _direction == 1 && AttackPhase == 0 && stanTimeRemain <= 0)
                    {
                        _direction *= -1;
                        gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    }



                    if (AttackPhase == 0 && stanTimeRemain <= 0)
                    {
                        // 現在の座標からのxyz を _MoveSpeed ずつ加算して移動
                        myTransform.Translate(_MoveSpeed * _direction, 0.0f, 0.0f, Space.World);
                    }

                    var difference = playerObject.transform.position.x - gameObject.transform.position.x;
                    if (difference < 0)
                    {
                        difference *= -1;
                    }

                    if (difference >= _tracking)
                    {
                        patrolType = 0;
                    }
                    break;

                case 2:
                    if (AttackPhase == 1 && stanTimeRemain <= 0)   //敵を捉えた時 攻撃までの硬直
                    {
                        // 現在の座標からのxyz を1ずつ加算して移動
                        //myTransform.Translate(0.001f * gameObject.transform.localScale.x, 0.0f, 0.0f, Space.World);
                        Count += Time.deltaTime;
                        if (Count >= _AttackWait)
                        {
                            AttackPhase = 2;
                            Count = 0;
                        }
                    }
                    else if (AttackPhase == 2 && stanTimeRemain <= 0)   //敵に攻撃
                    {
                        myTransform.Translate(0.2f * _direction, 0.0f, 0.0f, Space.World);
                        //AttackPhase = 0;
                        Count += Time.deltaTime;
                        if (Count >= _AttackTime)
                        {
                            AttackPhase = 0;
                            Count = 0;
                            stanTimeRemain += 2;
                            patrolType = 1;
                        }
                    }
                    break;
            }

            
            
        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.CompareTag("PatrolPoint"))
        {
            isReachTargetPosition = true;
            DecideTargetPotision();
        }
        */
        // 弱点のみ、IsTriggerをオンにしている。
        if (collision.CompareTag("AcidFlask"))
        {
            nowHP -= _HitDamage;
            Debug.Log(gameObject.name + "の弱点にヒット");
            enemyHpbar.SetBarValue(_HP, nowHP);
            if (nowHP <= 0)
            {
                isZeroHP = true;
            }
        }

        if (collision.CompareTag("Player") && patrolType == 0)   //パトロール中にplayerを見つけた時
        {
            patrolType = 1;     //敵を見つけて追いかけるモード
        }

        if (collision.CompareTag("Player") && patrolType == 1 && AttackPhase == 0 && stanTimeRemain <= 0)
        {
            AttackPhase = 1;
            patrolType = 2;
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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AcidFlask"))
        {
            Debug.Log(gameObject.name + "の非弱点にヒット");
            if (patrolType == 0)   //パトロール中にplayerを見つけた時
            {
                patrolType = 1;     //敵を見つけて追いかけるモード
            }
        }

        


        if (collision.gameObject.CompareTag("Gareki"))
        {
            animator.SetBool("Stun", true);
            animator.SetBool("Stand", false);
            animator.SetBool("Walk", false);
            stanTimeRemain += _stanTime;
            AttackPhase = 0;
            Count = 0;
            Debug.Log(gameObject.name + "にガレキがヒットしてスタンした");
        }
    }
}
