using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ChildSpider_Ceiling : MonoBehaviour {

    private enum Child
    {
        PointA,
        PointB,
        PointC,
        PointD,
        PlayerHitBox,
        AttackObject,
        Hit_WeakPoint,
        count,
    }

    private enum PATROL_TYPE
    {
        crawl,      //巡回
        tracking,   //追跡
        Attack,     //攻撃
    }

    [SerializeField, CustomLabel("死亡時煙エフェクト")] private GameObject _smokeEffect;
    [SerializeField, CustomLabel("死亡時血しぶきエフェクト1")] private GameObject _bloodSplashEffect1;
    [SerializeField, CustomLabel("死亡時血しぶきエフェクト2")] private GameObject _bloodSplashEffect2;
    [SerializeField, CustomLabel("死亡時血しぶきエフェクト3")] private GameObject _bloodSplashEffect3;
    [SerializeField] private float _HP = 1f;
    [SerializeField] private float _HitDamage = 1f;
    [SerializeField] private float _PlayerDamage = 1000f;
    [SerializeField] private float _PlayerDamageRate = 3f;
    [SerializeField] private float _nockBuckPower = 150f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
    private float _PlayerDamageTime;
    [SerializeField] private float _MoveSpeed = 0.1f;
    private bool directionChange;     //false:LEFT true:RIGHT
    private int _direction;
    [SerializeField] private float _directionRate = 3f;  //追跡中の反転時間
    private float directionTime;                    //追跡中の反転時間の格納用
    private bool directionChangeFlag;              //追跡中の反転するかしないか
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
    private Vector2 startPosition;
    private Vector2 startScale;
    private Vector3 startRotation;
    private int patrolType;     //0:パトロール 1:追尾 3:攻撃
    [SerializeField, Range(0f, 9999f), CustomLabel("酸に触れたときの被ダメージ")] private float _acidDamage = 1f;
    [SerializeField, Range(0.0167f, 10f), CustomLabel("酸の被ダメージレート")] private float _acidDamageRate = 0.5f;
    private float acidDamageTime;
    private GameObject playerObject;  //playerのオブジェクトを格納
    [SerializeField] private bool _trackingStart;   //追いかけることから始めるかどうか
    [SerializeField] private Vector2 _difference = new Vector2(30f,10f);    //プレイヤーとエネミーのｘとｙの差分を使ってどこまで追いかけるか、に使う

    private BoxCollider2D bodyCollider;
    private BoxCollider2D playerHitBox;
    private BoxCollider2D AttackObject;
    private BoxCollider2D WeakPointHitBox;


    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        bodyCollider = GetComponent<BoxCollider2D>();
        playerHitBox = transform.GetChild((int)Child.PlayerHitBox).GetComponent<BoxCollider2D>();
        AttackObject = transform.GetChild((int)Child.AttackObject).GetComponent<BoxCollider2D>();
        WeakPointHitBox = transform.GetChild((int)Child.Hit_WeakPoint).GetComponent<BoxCollider2D>();
    }


    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        startRotation = transform.rotation.eulerAngles;
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
            directionChange = false;
            gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        }
        else
        {
            _direction = 1;    //右
            directionChange = true;
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        }
        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, nowHP);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        AttackObject.enabled = false;

        animator = GetComponent<Animator>();

        if (_trackingStart)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            gameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            patrolType = (byte)PATROL_TYPE.tracking;     //敵を見つけて追いかけるモード
            //trackingTime = _trackingRate;
            transform.GetChild((int)Child.PlayerHitBox).gameObject.SetActive(false);
            if (playerObject.transform.position.x <= gameObject.transform.position.x)
            {
                directionChange = false;
                gameObject.transform.localScale = new Vector2(startScale.x, startScale.y);
                _direction = -1;     //左
            }
            else
            {
                directionChange = true;
                gameObject.transform.localScale = new Vector2(-startScale.x, startScale.y);
                _direction = 1;    //右
            }
            AttackObject.enabled = true;
            playerHitBox.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (isZeroHP)
        {
            transform.position = startPosition;
            transform.localScale = startScale;
            gameObject.transform.eulerAngles = startRotation;
            nowHP = _HP;
            enemyHpbar.SetBarValue(_HP, nowHP);
            enemyHpbar.hpbar.gameObject.SetActive(true);
            isZeroHP = false;
            Point_Position = PatrolPointPosition[PointCount];     //最初のパトロールポイントの座標を格納
            movetype = 2;
            AttackPhase = 0;
            Count = 0;
            AllColliderEnable();
            transform.GetChild((int)Child.PlayerHitBox).GetComponent<Collider2D>().enabled = true;
            transform.GetChild((int)Child.Hit_WeakPoint).GetComponent<Collider2D>().enabled = true;
            transform.GetChild((int)Child.PlayerHitBox).gameObject.SetActive(true);
            animator.SetBool("Walk", false);
            animator.SetBool("Stand", false);
            animator.SetBool("Stun", false);
            rb.bodyType = RigidbodyType2D.Kinematic;

            for (int i = 0; i < _PatrolPoint.Length; i++)
            {
                _PatrolPoint[i].SetActive(false);
            }

            if (Point_Position.x <= gameObject.transform.position.x)
            {
                _direction = -1;     //左
                directionChange = false;
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            }
            else
            {
                _direction = 1;    //右
                directionChange = true;
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            }

            if (_trackingStart)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                gameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                patrolType = (byte)PATROL_TYPE.tracking;     //敵を見つけて追いかけるモード
                                                             //trackingTime = _trackingRate;
                transform.GetChild((int)Child.PlayerHitBox).gameObject.SetActive(false);
                if (playerObject.transform.position.x <= gameObject.transform.position.x)
                {
                    directionChange = false;
                    gameObject.transform.localScale = new Vector2(-startScale.x, startScale.y);
                    _direction = -1;     //左
                }
                else
                {
                    directionChange = true;
                    gameObject.transform.localScale = new Vector2(startScale.x, startScale.y);
                    _direction = 1;    //右
                }
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

            if (0 < acidDamageTime)
            {
                acidDamageTime -= Time.deltaTime;
            }

            if (0 < pointWaitTime)
            {
                pointWaitTime -= Time.deltaTime;
            }
            else if (movetype == 1 && 0 >= pointWaitTime)
            {
                movetype = 2;
                animator.SetBool("Walk", true);
                animator.SetBool("Stand", false);
                animator.SetBool("Stun", false);
            }


            if (directionChangeFlag && 0 < directionTime)
            {
                directionTime -= Time.deltaTime;

                if (directionTime <= 0)
                {
                    directionChangeFlag = false;
                    _direction *= -1;
                    gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                }
            }

            if (patrolType == (byte)PATROL_TYPE.crawl && movetype == 0)
            {
                animator.SetBool("Stand", true);
                animator.SetBool("Walk", false);
                animator.SetBool("Stun", false);

                if (++PointCount > _PatrolPoint.Length - 1) PointCount = 0;  //配列の最大数に到達したら0に戻す
                Point_Position = PatrolPointPosition[PointCount];     //パトロールポイントの座標を格納
                if (gameObject.transform.position.x >= Point_Position.x)     //現在のポジションからポイントの座標を見て　設定する
                {
                    if (_direction == 1) gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    _direction = -1; //左
                    directionChange = false;
                }
                else if (gameObject.transform.position.x <= Point_Position.x)
                {
                    if (_direction == -1) gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    _direction = 1; //右
                    directionChange = true;
                }

                movetype = 1;     //硬直へ
                pointWaitTime += _pointWaitRate;
            }

            // transformを取得
            Transform myTransform = this.transform;

            switch (patrolType)
            {
                case (byte)PATROL_TYPE.crawl:
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

                case (byte)PATROL_TYPE.tracking:

                    var diPx = playerObject.transform.position.x - transform.position.x;
                    var diPy = playerObject.transform.position.y - transform.position.y;
                    if (diPx < 0) diPx *= -1;
                    if (diPy < 0) diPy *= -1;
                    if (_difference.x <= diPx || _difference.y <= diPy)
                    {
                        return;
                    }

                    //追いかける最中にプレイヤーが自分より逆にいた場合の処理
                    if (playerObject.transform.position.x >= transform.position.x && !directionChange && AttackPhase == 0 && stanTimeRemain <= 0)
                    {
                        directionChangeFlag = true;
                        directionTime = _directionRate;
                        directionChange = true;    //右
                        //_direction *= -1;
                        //_directionChange = true;
                        //gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    }
                    else if (playerObject.transform.position.x <= transform.position.x && directionChange && AttackPhase == 0 && stanTimeRemain <= 0)
                    {
                        directionChangeFlag = true;
                        directionTime = _directionRate;
                        directionChange = false;     //左
                        //_direction *= -1;
                        //gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    }

                    if (AttackPhase == 0 && stanTimeRemain <= 0)
                    {
                        // 現在の座標からのxyz を _MoveSpeed ずつ加算して移動
                        myTransform.Translate(_MoveSpeed * _direction, 0.0f, 0.0f, Space.World);
                    }

                    break;

                case (byte)PATROL_TYPE.Attack:
                    if (AttackPhase == 1 && stanTimeRemain <= 0)   //敵を捉えた時 攻撃までの硬直
                    {
                        // 現在の座標からのxyz を1ずつ加算して移動
                        //myTransform.Translate(0.001f * gameObject.transform.localScale.x, 0.0f, 0.0f, Space.World);
                        Count += Time.deltaTime;
                        if (Count >= _AttackWait)
                        {
                            AttackPhase = 2;
                            Count = 0;
                            SoundManagerV2.Instance.PlaySE(25);
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
                            patrolType = (byte)PATROL_TYPE.tracking;
                            AttackObject.enabled = true;
                            //trackingTime = _trackingRate + stanTimeRemain;
                        }
                    }
                    break;
            }
        }
    }

    public void AllColliderEnable()
    {
        bodyCollider.enabled = true;
        playerHitBox.enabled = true;
        WeakPointHitBox.enabled = true;
    }

    public void AllColliderDisable()
    {
        bodyCollider = GetComponent<BoxCollider2D>();
        playerHitBox = transform.GetChild((int)Child.PlayerHitBox).GetComponent<BoxCollider2D>();
        WeakPointHitBox = transform.GetChild((int)Child.Hit_WeakPoint).GetComponent<BoxCollider2D>();
        bodyCollider.enabled = false;
        playerHitBox.enabled = false;
        AttackObject.enabled = false;
        WeakPointHitBox.enabled = false;
    }

    public void HitBoxDisable()
    {
        playerHitBox.enabled = false;
        AttackObject.enabled = false;
        WeakPointHitBox.enabled = false;
    }

    // 死亡時処理
    private void Kill()
    {
        isZeroHP = true;
        animator.SetBool("Walk", false);
        animator.SetBool("Stand", false);
        animator.SetBool("Stun", false);
        animator.SetBool("Death", true);
        ScoreManager.Instance.KillCnt++;
        ScoreManager.Instance.TotalKillCnt++;
        HitBoxDisable();
        Instantiate(_smokeEffect, transform.position, _smokeEffect.transform.rotation);
        Instantiate(_bloodSplashEffect1, transform.position, _bloodSplashEffect1.transform.rotation);
        Instantiate(_bloodSplashEffect2, transform.position, _bloodSplashEffect2.transform.rotation);
        if (GetComponent<Rigidbody2D>().velocity.y == 0) {
            Instantiate(_bloodSplashEffect3, transform.position - new Vector3(0, 0.8F, 0), _bloodSplashEffect3.transform.rotation);
        }
        SoundManagerV2.Instance.PlaySE(26);
        SoundManagerV2.Instance.PlaySE(37);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isZeroHP) return;

        if (collision.CompareTag("AcidFlask"))
        {
            nowHP -= _HitDamage;
            Debug.Log(gameObject.name + "の弱点にヒット");
            enemyHpbar.SetBarValue(_HP, nowHP);
            if (nowHP <= 0)
            {
                Kill();
            }
        }

        if (collision.CompareTag("Player") && patrolType == (byte)PATROL_TYPE.tracking && AttackPhase == 0 && stanTimeRemain <= 0)
        {
            AttackPhase = 1;
            patrolType = (byte)PATROL_TYPE.Attack;
            AttackObject.enabled = false;
        }



        if (collision.CompareTag("Player") && patrolType == (byte)PATROL_TYPE.crawl)   //パトロール中にplayerを見つけた時
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            gameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            patrolType = (byte)PATROL_TYPE.tracking;     //敵を見つけて追いかけるモード
            //trackingTime = _trackingRate;
            transform.GetChild((int)Child.PlayerHitBox).gameObject.SetActive(false);
            if (playerObject.transform.position.x <= gameObject.transform.position.x)
            {
                directionChange = false;
                gameObject.transform.localScale = new Vector2(startScale.x, startScale.y);
                _direction = -1;     //左
            }
            else
            {
                directionChange = true;
                if (_direction == -1)
                    gameObject.transform.localScale = new Vector2(-startScale.x, startScale.y);
                _direction = 1;    //右
            }
            AttackObject.enabled = true;
            playerHitBox.enabled = false;
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


        if (collision.CompareTag("ResidualAcid"))
        {
            if (acidDamageTime <= 0)
            {
                GameObject acidParentBlock = collision.transform.parent.gameObject;
                var sprite = acidParentBlock.GetComponent<SpriteRenderer>();
                var _sprite = sprite.sprite;
                var halfX = _sprite.bounds.extents.x;
                var _vec = new Vector3(-halfX, 0f, 0f); // これは左上
                var _unvec = new Vector3(halfX, 0f, 0f); // これは右上
                var _pos = sprite.transform.TransformPoint(_vec);
                var _unpos = sprite.transform.TransformPoint(_unvec);
                if (transform.position.x >= _pos.x && transform.position.x <= _unpos.x)
                {
                    acidDamageTime += _acidDamageRate;
                    nowHP -= _acidDamage;
                    enemyHpbar.SetBarValue(_HP, nowHP);
                    if (nowHP <= 0)
                    {
                        Kill();
                    }
                    Debug.Log("酸に触れて " + _acidDamage + " ダメージを受けた");
                }
            }
        }
        else if (collision.CompareTag("WallReAcid"))
        {
            if (acidDamageTime <= 0)
            {
                GameObject acidParentBlock = collision.transform.parent.gameObject;
                var sprite = acidParentBlock.GetComponent<SpriteRenderer>();
                var _sprite = sprite.sprite;
                var _halfY = _sprite.bounds.extents.y;
                var _vec = new Vector3(0f, -_halfY, 0f); // これは上
                var _unvec = new Vector3(0f, _halfY, 0f); // これは下
                var _pos = sprite.transform.TransformPoint(_vec);
                var _unpos = sprite.transform.TransformPoint(_unvec);
                if (transform.position.y >= _pos.y && transform.position.y <= _unpos.y)
                {
                    acidDamageTime += _acidDamageRate;
                    nowHP -= _acidDamage;
                    enemyHpbar.SetBarValue(_HP, nowHP);
                    if (nowHP <= 0)
                    {
                        Kill();
                    }
                    Debug.Log("酸に触れて " + _acidDamage + " ダメージを受けた");
                }
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isZeroHP) return;


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
            }
            SoundManagerV2.Instance.PlaySE(2);
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
