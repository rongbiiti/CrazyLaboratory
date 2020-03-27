using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bee : MonoBehaviour
{

    [SerializeField] private float _HP = 1f;
    [SerializeField] private float _HitDamage = 1f;
    [SerializeField] private float _PlayerDamage = 1000f;
    [SerializeField] private float _PlayerDamageRate = 3f;
    [SerializeField] private float _nockBuckPower = 150f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
    private float _PlayerDamageTime;
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _moveDashSpeed = 40f;

    //[SerializeField] private bool _directionChange;     //false:LEFT true:RIGHT
    private int _direction;
    [SerializeField] private float _stanTime = 3f;
    private float stanTimeRemain = 0;
    private float nowHP;

    [SerializeField] private GameObject[] _PatrolPoint;
    [SerializeField] private GameObject _waitPosition;  //待機場所
    private Vector3[] PatrolPointPosition;
    [SerializeField] private GameObject _player_Hit_Patrol;  //待機中にプレイヤーが入ったら巡回に行くためのオブジェクト
    private int PointCount;    //パトロールポイントのカウント PatrolPointの配列の数が最大値
    [SerializeField] private float _attackWaitRate = 2f;      //攻撃前　硬直　指定用
    private float attackWaitTime;      //攻撃前　硬直
    [SerializeField] private float _attackafterRate = 2f;      //攻撃後　硬直　指定用
    private float _attackafterTime;      //攻撃後　硬直



    private Vector3 Point_Position;     //パトロールポイントの座標の格納用

    //private Vector3 PointB_Position;
    private byte AttackPhase;
    private float Count;
    private EnemyHpbar enemyHpbar;
    private bool isZeroHP;
    [SerializeField] private float _destroyTime = 2f;
    private Vector2 startScale;
    private int patrolType = 0;     //0:パトロール 1:追尾 3:攻撃
    private GameObject playerObject;  //playerのオブジェクトを格納
    private Rigidbody2D rb;
    private Vector3 targetPosition;
    private bool isReachTargetPosition;
    private bool istargetPointA;

    private Vector3 waitingPosition;    //待機場所のtransform格納用
    private Vector3 waitingRotion;    //待機場所のtransform格納用
    private bool waitType = true;  //false:待機してない true:待機中
    [SerializeField] private int _childNullCount = 4;   //子を離す数



    // Use this for initialization
    void Start()
    {
        startScale = transform.localScale;
        PatrolPointPosition = new Vector3[_PatrolPoint.Length];
        for (int i = 0; i < _PatrolPoint.Length; i++)
        {
            PatrolPointPosition[i] = _PatrolPoint[i].gameObject.transform.position;
        }
        _waitPosition.SetActive(false);
        _PatrolPoint[PointCount].SetActive(true);               //最初のパトロールポイントのアクティブをtrueにする
        //Point_Position = PatrolPointPosition[PointCount];       //最初のパトロールポイントの座標を格納
        AttackPhase = 0;
        Count = 0;
        nowHP = _HP;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        waitingPosition = gameObject.transform.position;
        waitingRotion = gameObject.transform.eulerAngles;
        if (playerObject.transform.position.x <= gameObject.transform.position.x)
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
        targetPosition = PatrolPointPosition[PointCount];
        rb = GetComponent<Rigidbody2D>();
        var count = transform.childCount - 1;
        for (int i = count; i > count - _childNullCount; i--) 
        {
            Debug.Log(i);
            transform.GetChild(i).gameObject.SetActive(false);
            transform.GetChild(i).transform.parent = null;
            //if (i >= _PatrolPoint.Length) continue;
            //_PatrolPoint[i].SetActive(false);
            
        }
        //transform.DetachChildren();
        //transform.GetChild(0).gameObject.SetActive(true);

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
            }

            if (0 < attackWaitTime)
            {
                attackWaitTime -= Time.deltaTime;
            }

            Transform myTransform = this.transform;
            switch (patrolType)
            {
                case 0:
                    Debug.Log("待機中");
                    
                    if (waitType == true)
                    {
                        if (playerObject.transform.position.x >= gameObject.transform.position.x)
                        {
                            gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        }
                        else
                        {
                            gameObject.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
                        }
                        break;
                    }

                    if (waitType == true) break;

                    Debug.Log(waitType);
                    //DecideTargetPotision();
                    // 巡回ポイントの位置を取得
                    Vector2 targetPos = waitingPosition;
                    // 巡回ポイントのx座標
                    float x = targetPos.x;
                    // ENEMYは、地面を移動させるので座標は常に0とする
                    float y = targetPos.y;
                    // 移動を計算させるための２次元のベクトルを作る
                    Vector2 direction = new Vector2(x - transform.position.x, y - transform.position.y).normalized;
                    // ENEMYのRigidbody2Dに移動速度を指定する
                    rb.velocity = direction * _moveSpeed;
                    

                    break;

                case 1:
                    Debug.Log("巡回中");

                    //DecideTargetPotision();
                    // 巡回ポイントの位置を取得
                    targetPos = targetPosition;
                    // 巡回ポイントのx座標
                    x = targetPos.x;
                    // ENEMYは、地面を移動させるので座標は常に0とする
                    y = targetPos.y;
                    // 移動を計算させるための２次元のベクトルを作る
                    direction = new Vector2(x - transform.position.x, y - transform.position.y).normalized;
                    // ENEMYのRigidbody2Dに移動速度を指定する
                    rb.velocity = direction * _moveSpeed;

                    Debug.Log(targetPos);
                    break;
                case 2:
                    
                    if (AttackPhase == 1 && stanTimeRemain <= 0)   //敵を捉えた時 攻撃までの硬直
                    {

                        if (attackWaitTime > 0)
                        {
                            _PatrolPoint[PointCount].transform.position = playerObject.transform.position;
                            break;
                        }
                        Debug.Log("攻撃");
                        // 現在の座標からのxyz を1ずつ加算して移動
                        //myTransform.Translate(0.001f * gameObject.transform.localScale.x, 0.0f, 0.0f, Space.World);

                        targetPos = _PatrolPoint[PointCount].transform.position;
                        // 巡回ポイントのx座標
                        x = targetPos.x;
                        // ENEMYは、地面を移動させるので座標は常に0とする
                        y = targetPos.y;
                        // 移動を計算させるための２次元のベクトルを作る
                        direction = new Vector2(x - transform.position.x, y - transform.position.y).normalized;
                        // ENEMYのRigidbody2Dに移動速度を指定する
                        rb.velocity = direction * _moveDashSpeed;

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

        if (++PointCount > _PatrolPoint.Length) PointCount = 0;  //配列の最大数に到達したら0に戻す
        Debug.Log(PointCount);

        if (PointCount == _PatrolPoint.Length)  //_patrolPoint配列の最後のオブジェクトを利用してプレイヤーの座標にオブジェクトを移動させる
        {
            targetPosition = playerObject.transform.position;
            //_PatrolPoint[PointCount - 1].SetActive(false);
            PointCount = _PatrolPoint.Length - 1;
            //_PatrolPoint[PointCount].transform.position = playerObject.transform.position;
            patrolType = 2;
            AttackPhase = 1;
            attackWaitTime += _attackWaitRate;
            rb.velocity = new Vector2(0.0f, 0.0f);
            return;
        }

        targetPosition = PatrolPointPosition[PointCount];
        //istargetPointA = false;
        _PatrolPoint[PointCount].SetActive(true);
        if (PointCount == 0)
        {
            _PatrolPoint[PatrolPointPosition.Length - 1].SetActive(false);
        }
        else
        {
            _PatrolPoint[PointCount - 1].SetActive(false);
            
        }
        //_pointA.SetActive(true);
        //_pointB.SetActive(false);

        if (gameObject.transform.position.x >= targetPosition.x)     //現在のポジションからポイントの座標を見て　設定する
        {
            if (_direction == 1) gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            _direction = -1; //左

        }
        else if (gameObject.transform.position.x <= targetPosition.x)
        {
            if (_direction == -1) gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            _direction = 1; //右
        }

        isReachTargetPosition = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

        if (collision.gameObject.CompareTag("PatrolPoint"))
        {
            
            if(patrolType == 2 && AttackPhase == 1)
            {
                patrolType = 0;
                _PatrolPoint[PointCount].transform.position = PatrolPointPosition[PointCount];
                _PatrolPoint[PointCount].gameObject.SetActive(false);
                _waitPosition.SetActive(true);
                PointCount = 0;
                waitType = false;
                return;
            }

            isReachTargetPosition = true;
            DecideTargetPotision();
        }

        if (collision.gameObject.CompareTag("WaitingPoint"))
        {
            if (patrolType == 0 && waitType == false)
            {
                waitType = true;
                gameObject.transform.position = waitingPosition;
                gameObject.transform.eulerAngles = waitingRotion;
                rb.velocity = new Vector2(0.0f,0.0f);
                _player_Hit_Patrol.SetActive(true);
                return;
            }
        }

        if (collision.CompareTag("Player") && patrolType == 0 && waitType)   //パトロール中にplayerを見つけた時
        {
            patrolType = 1;     //巡回モード
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, 1.0f);
            gameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            _PatrolPoint[0].SetActive(true);
            targetPosition = _PatrolPoint[0].transform.position;
            isReachTargetPosition = true;
            _player_Hit_Patrol.SetActive(false);
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


        if (collision.gameObject.CompareTag("Gareki"))
        {
            stanTimeRemain += _stanTime;
            AttackPhase = 0;
            Count = 0;
            Debug.Log(gameObject.name + "にガレキがヒットしてスタンした");
        }
    }
}
