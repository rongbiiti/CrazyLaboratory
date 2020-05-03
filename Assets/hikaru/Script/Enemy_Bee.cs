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

    [SerializeField] private bool _directionChange;     //false:LEFT true:RIGHT
    //private int _direction;
    private float Start_Rotation_Z;   //Zの回転情報
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
    private Vector2 startPosition;
    private Vector3 startRotation;
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
    Animator animator;

    // Use this for initialization
    void Start()
    {
        startScale = transform.localScale;
        startPosition = transform.position;
        startRotation = transform.rotation.eulerAngles;
        Start_Rotation_Z = transform.rotation.eulerAngles.z;
        PatrolPointPosition = new Vector3[_PatrolPoint.Length];
        for (int i = 0; i < _PatrolPoint.Length; i++)
        {
            PatrolPointPosition[i] = _PatrolPoint[i].gameObject.transform.position;
        }
        _waitPosition.SetActive(false);
        //_PatrolPoint[PointCount].SetActive(true);               //最初のパトロールポイントのアクティブをtrueにする
        //Point_Position = PatrolPointPosition[PointCount];       //最初のパトロールポイントの座標を格納
        AttackPhase = 0;
        Count = 0;
        nowHP = _HP;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        waitingPosition = gameObject.transform.position;
        waitingRotion = gameObject.transform.eulerAngles;

        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, nowHP);
        targetPosition = PatrolPointPosition[PointCount];
        rb = GetComponent<Rigidbody2D>();
        var count = transform.childCount - 1;
        for (int i = count; i > count - _childNullCount; i--) 
        {
            transform.GetChild(i).gameObject.SetActive(false);
            transform.GetChild(i).transform.parent = null;
        }

        animator = GetComponent<Animator>();
        int z = (int)Start_Rotation_Z;
        switch (z)
        {
            case 0:
            case 90:
                _directionChange = false;
                break;
            case 180:
            case 270:
                _directionChange = true;
                break;
        }

        Direction(playerObject.transform.position);

    }

    private void OnEnable()
    {
        if (isZeroHP) 
        {
            transform.position = startPosition;
            transform.localScale = startScale;
            gameObject.transform.eulerAngles = startRotation;
            AttackPhase = 0;
            Count = 0;
            _waitPosition.SetActive(false);
            targetPosition = PatrolPointPosition[PointCount];
            nowHP = _HP;
            enemyHpbar.SetBarValue(_HP, nowHP);
            enemyHpbar.hpbar.gameObject.SetActive(true);
            isZeroHP = false;
            gameObject.transform.GetChild(0).transform.GetComponent<Collider2D>().enabled = true;
            gameObject.transform.GetChild(1).transform.GetComponent<Collider2D>().enabled = true;
            var count = transform.childCount - 1;
            for (int i = count; i > count - _childNullCount; i--)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }


            int z = (int)Start_Rotation_Z;
            switch (z)
            {
                case 0:
                case 90:
                    _directionChange = false;
                    break;
                case 180:
                case 270:
                    _directionChange = true;
                    break;
            }

            Direction(playerObject.transform.position);

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
                    animator.SetBool("Stand", true);
                    animator.SetBool("Fly", false);
                    animator.SetBool("Sting", false);
                    animator.SetBool("Stun", false);

                    if (waitType == true)
                    {
                        //Direction(playerObject.transform.position);
                    }

                    if (waitType == true) break;

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

                    animator.SetBool("Stand", false);
                    animator.SetBool("Fly", true);
                    animator.SetBool("Sting", false);
                    animator.SetBool("Stun", false);

                    break;

                case 1:
                    animator.SetBool("Stand", false);
                    animator.SetBool("Fly", true);
                    animator.SetBool("Sting", false);
                    animator.SetBool("Stun", false);

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

                    break;
                case 2:                    

                    if (AttackPhase == 1 && stanTimeRemain <= 0)   //敵を捉えた時 攻撃までの硬直
                    {

                        if (attackWaitTime > 0)
                        {
                            animator.SetBool("Stand", false);
                            animator.SetBool("Fly", false);
                            animator.SetBool("Sting", true);
                            animator.SetBool("Stun", false);

                            _PatrolPoint[PointCount].transform.position = playerObject.transform.position;

                            break;
                        }
                        
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

        if (PointCount == _PatrolPoint.Length)  //_patrolPoint配列の最後のオブジェクトを利用してプレイヤーの座標にオブジェクトを移動させる
        {
            targetPosition = playerObject.transform.position;
            PointCount = _PatrolPoint.Length - 1;
            patrolType = 2;
            AttackPhase = 1;
            attackWaitTime += _attackWaitRate;
            rb.velocity = new Vector2(0.0f, 0.0f);
            Direction(targetPosition);
            return;
        }

        targetPosition = PatrolPointPosition[PointCount];
        _PatrolPoint[PointCount].SetActive(true);
        if (PointCount == 0)
        {
            _PatrolPoint[PatrolPointPosition.Length - 1].SetActive(false);
        }
        else
        {
            _PatrolPoint[PointCount - 1].SetActive(false);
            
        }

        Direction(targetPosition);

        isReachTargetPosition = false;
    }

    private void Direction(Vector2 target_Position)    //回転のｚの数値によって反転を変える
    {

        if (patrolType != 0)
        {
            if (_directionChange == true && gameObject.transform.position.x >= playerObject.transform.position.x)     //現在のポジションからポイントの座標を見て　設定する
            {
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                _directionChange = false;
            }
            else if (_directionChange == false && gameObject.transform.position.x < playerObject.transform.position.x)
            {
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                _directionChange = true;
            }
            return;
        }

        int z = (int)Start_Rotation_Z;
        switch (z)
        {
            case 0:
            case 180:
                if (_directionChange == true && gameObject.transform.position.x >= playerObject.transform.position.x)     //現在のポジションからポイントの座標を見て　設定する
                {
                    gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    _directionChange = false;
                }
                else if (_directionChange == false && gameObject.transform.position.x < playerObject.transform.position.x)
                {
                    gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    _directionChange = true;
                }
                break;

            case 90:
            case 270:
                if (_directionChange == true && gameObject.transform.position.y >= playerObject.transform.position.y)     //現在のポジションからポイントの座標を見て　設定する
                {
                    gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    _directionChange = false;
                }
                else if (_directionChange == false && gameObject.transform.position.y < playerObject.transform.position.y)
                {
                    gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    _directionChange = true;
                }
                break;
        }

        return;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isZeroHP) return;

        // 弱点のみ、IsTriggerをオンにしている。
        if (collision.CompareTag("AcidFlask"))
        {
            nowHP -= _HitDamage;
            Debug.Log(gameObject.name + "の弱点にヒット");
            enemyHpbar.SetBarValue(_HP, nowHP);
            rb.velocity = new Vector2(0.0f, 0.0f);
            if (nowHP <= 0)
            {
                isZeroHP = true;
                ScoreManager.Instance.KillCnt++;
                ScoreManager.Instance.TotalKillCnt++;
                gameObject.transform.GetChild(0).transform.GetComponent<Collider2D>().enabled = false;
                gameObject.transform.GetChild(1).transform.GetComponent<Collider2D>().enabled = false;
            }
        }

        if (collision.gameObject.CompareTag("PatrolPoint"))
        {
            if(patrolType == 0)
            {
                return;
            }
            
            if(patrolType == 2 && AttackPhase == 1)
            {
                animator.SetBool("Stand", false);
                animator.SetBool("Fly", true);
                animator.SetBool("Sting", false);
                animator.SetBool("Stun", false);

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
                gameObject.transform.localScale = startScale;
                rb.velocity = new Vector2(0.0f,0.0f);
                _player_Hit_Patrol.SetActive(true);

                int z = (int)Start_Rotation_Z;
                switch (z)
                {
                    case 0:
                    case 90:
                        _directionChange = false;
                        break;
                    case 180:
                    case 270:
                        _directionChange = true;
                        break;
                }
                //Direction(playerObject.transform.position);
                return;
            }
        }

        if (collision.CompareTag("Player") && patrolType == 0 && waitType)   //パトロール中にplayerを見つけた時
        {
            patrolType = 1;     //巡回モード
            _directionChange = false;

            var ls = transform.localScale;
            gameObject.transform.localScale = startScale;
            gameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            _PatrolPoint[0].SetActive(true);
            targetPosition = _PatrolPoint[0].transform.position;
            isReachTargetPosition = true;
            _player_Hit_Patrol.SetActive(false);
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isZeroHP) return;

        if (collision.CompareTag("Player") && AttackPhase == 2 && _PlayerDamageTime <= 0 && stanTimeRemain <= 0)
        {
            _PlayerDamageTime += _PlayerDamageRate;
            collision.gameObject.GetComponent<PlayerController>().Damage(_PlayerDamage);
            Rigidbody2D prb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 targetPos = collision.gameObject.transform.position;
            float y = _nockBuckUpperPower;
            float x = targetPos.x;
            Vector2 direction = new Vector2(x - transform.position.x, y).normalized;
            if (!collision.gameObject.GetComponent<PlayerController>().IsNotNockBack)
            {
                prb.velocity = direction * _nockBuckPower;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isZeroHP) return;

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
            if (!collision.gameObject.GetComponent<PlayerController>().IsNotNockBack)
            {
                prb.velocity = direction * _nockBuckPower;
                SoundManagerV2.Instance.PlaySE(2);
            }
        }


        if (collision.gameObject.CompareTag("Gareki"))
        {
            animator.SetBool("Stand", false);
            animator.SetBool("Fly", false);
            animator.SetBool("Sting", false);
            animator.SetBool("Stun", true);

            stanTimeRemain += _stanTime;
            ScoreManager.Instance.StunCnt++;
            ScoreManager.Instance.TotalStunCnt++;
            AttackPhase = 0;
            Count = 0;
            Debug.Log(gameObject.name + "にガレキがヒットしてスタンした");
        }
    }
}
