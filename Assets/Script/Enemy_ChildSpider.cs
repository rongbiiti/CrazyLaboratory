using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ChildSpider : MonoBehaviour {

    [SerializeField] private float _HP;
    [SerializeField] private float _HitDamage;
    [SerializeField] private float _PlayerDamage;
    [SerializeField] private float _PlayerDamageRate;
    [SerializeField] private float _nockBuckPower = 300f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
    private float _PlayerDamageTime;
    [SerializeField] private float _MoveSpeed;
    //[SerializeField] private bool _directionChange;     //false:LEFT true:RIGHT
    private int _direction;
    [SerializeField] private byte _AttackWait;
    [SerializeField] private float _AttackTime;
    [SerializeField] private float _stanTime = 3f;
    private float stanTimeRemain = 0;
    private float nowHP;

    [SerializeField] private GameObject[] PatrolPoint;
    private Vector3[] PatrolPointPosition;
    private byte patroltype;    //0：次のパトロール位置を取得する待ち    1:取得した後、硬直する 3：動く
    private int PointCount;    //パトロールポイントのカウント PatrolPointの配列の数が最大値
    [SerializeField] private float pointWaitRate;      //パトロール後の硬直
    private float pointWaitTime;      //パトロール後の硬直

    private Vector3 Point_Position;     //パトロールポイントの座標の格納用

    //private Vector3 PointB_Position;
    private byte AttackPhase;
    private float Count;
    private EnemyHpbar enemyHpbar;
    private bool isZeroHP;
    [SerializeField] private float destroyTime = 2f;
    private Vector2 startScale;

    // Use this for initialization
    void Start()
    {
        startScale = transform.localScale;
        PatrolPointPosition = new Vector3[PatrolPoint.Length];
        Debug.Log(PatrolPoint.Length);
        for (int i = 0; i < PatrolPoint.Length; i++)
        {
            PatrolPointPosition[i] = PatrolPoint[i].gameObject.transform.position;
            PatrolPoint[i].SetActive(false);
        }
        Point_Position = PatrolPointPosition[PointCount];     //最初のパトロールポイントの座標を格納
        patroltype = 2;
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
        //if (_directionChange)
        //{
        //    _direction = 1;
        //    gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        //}
        //else
        //{
        //    _direction = -1;
        //    gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        //}
        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, nowHP);
    }

    void FixedUpdate()
    {
        if (isZeroHP)
        {
            if (0 < transform.localScale.x)
            {
                transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            }
            else if (transform.localScale.x < 0)
            {
                transform.localScale -= new Vector3(-startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
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

            if (0 < pointWaitTime)
            {
                pointWaitTime -= Time.deltaTime;
            }
           else if(patroltype == 1 && 0 >= pointWaitTime)
            {
                patroltype = 2;
            }

            if (patroltype == 0)
            {
                if (PointCount++ > PatrolPoint.Length - 1) PointCount = 0;  //配列の最大数に到達したら0に戻す

                Point_Position = PatrolPointPosition[PointCount];     //パトロールポイントの座標を格納
                Debug.Log(PointCount);
                Debug.Log(Point_Position.x);
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

                patroltype = 1;     //硬直へ
                pointWaitTime += pointWaitRate;
            }
            



            //if (Point_Position.x >= transform.position.x && AttackPhase == 0 && !_directionChange && stanTimeRemain <= 0)
            //{
            //    _direction = -1;
            //    _directionChange = true;
            //    gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * _direction, gameObject.transform.localScale.y);
            //}
            //else if (Point_Position.x <= transform.position.x && AttackPhase == 0 && _directionChange && stanTimeRemain <= 0)
            //{
            //    _direction = 1;
            //    _directionChange = false;
            //    gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * _direction, gameObject.transform.localScale.y);
            //}

            // transformを取得
            Transform myTransform = this.transform;
            

            if (AttackPhase == 0 && stanTimeRemain <= 0)        //パトロール中
            {
                if (patroltype != 2) return;

                // 現在の座標からのxyz を _MoveSpeed ずつ加算して移動
                myTransform.Translate(_MoveSpeed * _direction, 0.0f, 0.0f, Space.World);
                Debug.Log(_direction);

                

                //パトロールポイントを超えたら待機タイプに変える
                if(_direction == -1 && gameObject.transform.position.x <= Point_Position.x)
                {
                    patroltype = 0;
                }
                else if(_direction == 1 && gameObject.transform.position.x >= Point_Position.x)
                {
                    patroltype = 0;
                }

            }
            else if (AttackPhase == 1 && stanTimeRemain <= 0)   //敵を捉えた時 攻撃までの硬直
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
                myTransform.Translate(0.2f * gameObject.transform.localScale.x * -1, 0.0f, 0.0f, Space.World);
                //AttackPhase = 0;
                Count += Time.deltaTime;
                if (Count >= _AttackTime)
                {
                    AttackPhase = 0;
                    Count = 0;
                    stanTimeRemain += 2;
                }
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
            Destroy(collision.gameObject);
            Debug.Log(gameObject.name + "の弱点にヒット");
            enemyHpbar.SetBarValue(_HP, nowHP);
            if (nowHP <= 0)
            {
                isZeroHP = true;
            }
        }
        if (collision.CompareTag("Player") && AttackPhase == 0 && stanTimeRemain <= 0)
        {
            AttackPhase = 1;
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
            Destroy(collision.gameObject);
            Debug.Log(gameObject.name + "の非弱点にヒット");
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
