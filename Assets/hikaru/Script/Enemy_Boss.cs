using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{

    private enum e_AttackType   //attackタイプの列挙体
    {
        move,
        BeforAttack,
        Attack,
        AfterAttack,
    }

    private enum e_ActivityType
    {
        Wait,
        Jump,
        Attack,
        BodyPress,
    }


    [SerializeField] private float _HP = 12f;
    [SerializeField] private float _HitDamage = 1f;
    [SerializeField] private float _AttackHitDamage = 3000f;        //攻撃のダメージ
    [SerializeField] private float _BodyPressHitDamage = 5000f;     //ボディプレスのダメージ
    private float PlayerDamage;    //ダメージ格納用
    [SerializeField] private float _PlayerDamageRate = 3f;
    private float _PlayerDamageTime;
    [SerializeField] private float _nockBuckPower = 300f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
     private float moveSpeed;   //スピード格納用
    /*******************Attack*********************/
    private byte AttackType;    //攻撃のタイプ 0:Wait 1:攻撃前befor 2:攻撃attack 3:攻撃後after
    [SerializeField] private int _rangeMin = 2;     //ランダムの最小値
    [SerializeField] private int _rangeMax = 4;     //ランダムの最大値
    [SerializeField] private float _beforeAttackRate = 0.7f;    //攻撃前の硬直時間
    //private float BeforeAttackTime;     //攻撃前の時間の格納用
    [SerializeField] private float _AttackRate = 1f; //攻撃の時間
    private float AttackTime;           //攻撃の時間の格納用  
    [SerializeField] private float _afterAttackRate = 1f;    //攻撃後の硬直時間
    //private float AfterAttackTime;      //攻撃後の時間の格納用
    [SerializeField] private float _playerY;    //playerのｙにプラス　降下時点の微調整用
    [SerializeField] private float _playerX;    //playerのｘにプラスかマイナスする　効果時点の微調整用
    [SerializeField] private float _fallSpeed = 30f;  //降下スピード
    [SerializeField] private float _ascentSpeed = 30f;    //上昇スピード
    /*******************BodyPress*********************/
    [SerializeField] private float _bodyPressRate = 3f;
    private float BodyPressTime;
    [SerializeField] private float _bodyPressFallSpeed = 50f;
    /*******************Stan***********************/
    [SerializeField] private float _stanRate = 4f;  //ひるみの時間
    private float StanTime;   //ひるみの時間格納用
    /*******************Jump***********************/
    [SerializeField] private float _jumpRate = 3f;  //ジャンプまでの時間
    private float JumpTime;     //ジャンプまでの時間の格納用
    [SerializeField] private float _jumpSpeed = 30f;  //ジャンプスピード
    /*******************ActivityTime***********************/
    private byte ActivityType;     //行動のタイプ
    [SerializeField] e_ActivityType[] _activityTypeCount;
    private byte ActivityCount;     //カウントによって行動を登録
    [SerializeField] private float _activityRate;   //次に行動する時間
    private float ActivityTime;     //次に行動する時間の格納用
    /*******************GameObject**********************/
    [SerializeField] private GameObject _switchObject;   //上昇して行動を切り替えさせるためのオブジェクト
    [SerializeField] private GameObject _attackObject;   //攻撃のオブジェクト
    [SerializeField] private GameObject _bodyPressEndObject;    //ボディプレスを止めるためのオブジェクト
    /**********************************************/
    [SerializeField] private float _destroyTime = 2f;
    private bool SwitchFlag;    //スイッチオブジェクトに当たった時の処理をするかしないか
    private float nowHP;
    private EnemyHpbar enemyHpbar;
    private bool isZeroHP;
    private Vector2 startScale;
    private Vector2 startposition;
    private Vector3 startPlayerPosition;
    private GameObject playerObject;  //playerのオブジェクトを格納
    Rigidbody2D rb;
    Animator animetor;

    // Use this for initialization
    void Start()
    {
        /*transform*/
        startScale = transform.localScale;
        startposition = transform.position;
        /*HP*/
        nowHP = _HP;
        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, nowHP);
        
        /*GameObject*/
        playerObject = GameObject.FindGameObjectWithTag("Player");
        _switchObject.transform.parent = null;
        _bodyPressEndObject.transform.parent = null;
        _bodyPressEndObject.SetActive(false);
        _attackObject.SetActive(false);
        /**/
        var p = playerObject.transform.position;
        SwitchFlag = false;
        startPlayerPosition = p;
        ActivityCount = 0;
        InitActivityType((byte)_activityTypeCount[ActivityCount]);
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (isZeroHP)
        {
            transform.position = startposition;
            transform.localScale = startScale;
            nowHP = _HP;
            enemyHpbar.SetBarValue(_HP, nowHP);
            enemyHpbar.hpbar.gameObject.SetActive(true);
            isZeroHP = false;
            ActivityCount = 0;
            InitActivityType((byte)_activityTypeCount[ActivityCount]);
            JumpTime += _jumpRate;
            _switchObject.SetActive(true);
        }

    }

    // Update is called once per frame
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
            if (0 < _PlayerDamageTime)  //接触ダメージの時間間隔
            {
                _PlayerDamageTime -= Time.deltaTime;
            }
            if (0 < AttackTime)     //攻撃時間
            {
                AttackTime -= Time.deltaTime;

                if (AttackTime <= 0)
                {
                    switch (AttackType)
                    {
                        case (byte)e_AttackType.move:
                            var BossP = transform.position;
                            var PlayerP = playerObject.transform.position;
                            if(0.0f > playerObject.transform.localScale.x)
                            {
                                BossP.x = PlayerP.x + _playerX;
                            }
                            else
                            {
                                BossP.x = PlayerP.x - _playerX;
                                var ls = gameObject.transform.localScale;   //localscaleの格納
                                gameObject.transform.localScale = new Vector3(ls.x, -ls.y, ls.z);
                            }
                            
                            transform.position = BossP;
                            break;
                        case (byte)e_AttackType.BeforAttack:
                            AttackType = (byte)e_AttackType.Attack;
                            AttackTime = _AttackRate;
                            _attackObject.SetActive(true);
                            break;
                        case (byte)e_AttackType.Attack:
                            AttackType = (byte)e_AttackType.AfterAttack;
                            AttackTime = _afterAttackRate;
                            _attackObject.SetActive(false);
                            break;
                        case (byte)e_AttackType.AfterAttack:
                            moveSpeed = _ascentSpeed;   //上昇するスピードを格納
                            _switchObject.SetActive(true);
                            SwitchFlag = false;
                            break;
                    }
                }
            }

            if(0 < BodyPressTime)
            {
                BodyPressTime -= Time.deltaTime;
                if(BodyPressTime <= 0)
                {
                    var BossP = transform.position;
                    var PlayerP = playerObject.transform.position;

                    if(PlayerP.x < BossP.x)
                    {
                        var ls = gameObject.transform.localScale;   //localscaleの格納
                        gameObject.transform.localScale = new Vector3(ls.x, -ls.y, ls.z);
                    }

                    BossP.x = PlayerP.x;
                    transform.position = BossP;
                }
            }


            if (0 < StanTime)       //スタンの時間
            {
                StanTime -= Time.deltaTime;
            }
            if (0 < JumpTime)       //ジャンプをするまでの時間
            {
                JumpTime -= Time.deltaTime;
                if(JumpTime <= 0)
                {
                    _switchObject.SetActive(true);
                    SwitchFlag = false;
                }
            }
            if (0 < ActivityTime)   //行動までの時間
            {
                ActivityTime -= Time.deltaTime;
            }

            switch (ActivityType)   //行動のタイプによって動きを変える
            {
                case (byte)e_ActivityType.Jump:
                    if (JumpTime <= 0)
                    {
                        Vector2 speed = new Vector2(0.0f, _jumpSpeed);
                        rb.velocity = speed;
                    }
                    break;

                case (byte)e_ActivityType.Attack:
                    if(AttackType == (byte)e_AttackType.move && AttackTime <= 0)
                    {
                        Vector2 speed = new Vector2(0.0f, moveSpeed);
                        rb.velocity = speed;
                        if (startPlayerPosition.y + _playerY >= transform.position.y)
                        {
                            AttackType = (byte)e_AttackType.BeforAttack;
                            AttackTime += _beforeAttackRate;
                            rb.velocity = new Vector2(0.0f, 0.0f);
                        }
                    }
                    
                    if(AttackType == (byte)e_AttackType.AfterAttack && AttackTime <= 0)
                    {
                        Vector2 speed = new Vector2(0.0f, moveSpeed);
                        rb.velocity = speed;
                    }
                    break;

                case (byte)e_ActivityType.BodyPress:

                    if(BodyPressTime <= 0)
                    {
                        Vector2 speed = new Vector2(0.0f, moveSpeed);
                        rb.velocity = speed;
                    }

                    break;

            }

        }
    }
    
    void InitActivityType(byte Type)
    {
        Debug.Log(SwitchFlag);
        switch (Type)
        {
            case (byte)e_ActivityType.Jump:
                ActivityType = (byte)e_ActivityType.Jump;
                AttackType = 0;
                JumpTime = _jumpRate;
                PlayerDamage = _AttackHitDamage;
                break;
            case (byte)e_ActivityType.Attack:
                ActivityType = (byte)e_ActivityType.Attack;
                AttackType = (byte)e_AttackType.move;
                int range = Random.Range(_rangeMin, _rangeMax);
                AttackTime += range;
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 90f);
                moveSpeed = -_fallSpeed;
                PlayerDamage = _AttackHitDamage;
                break;
            case (byte)e_ActivityType.BodyPress:
                ActivityType = (byte)e_ActivityType.BodyPress;
                BodyPressTime = _bodyPressRate;
                moveSpeed = -_bodyPressFallSpeed;
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                _bodyPressEndObject.SetActive(true);
                PlayerDamage = _BodyPressHitDamage;
                break;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isZeroHP) return;

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

        if (collision.CompareTag("PatrolPoint") && !SwitchFlag)     //SwitchObjectとの当たり判定
        {
            SwitchFlag = true;
            _switchObject.gameObject.SetActive(false);
            gameObject.transform.localScale = startScale;
            if (_activityTypeCount.Length <= ++ActivityCount)
            {
                ActivityCount = 0;
            }
            Debug.Log((byte)_activityTypeCount[ActivityCount]);
            
            rb.velocity = new Vector2(0.0f, 0.0f);
            InitActivityType((byte)_activityTypeCount[ActivityCount]);
            
        }

        if (collision.CompareTag("WaitingPoint"))
        {
            if (_activityTypeCount.Length <= ++ActivityCount)
            {
                ActivityCount = 0;
            }

            rb.velocity = new Vector2(0.0f, 0.0f);
            InitActivityType((byte)_activityTypeCount[ActivityCount]);
            _bodyPressEndObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isZeroHP) return;

        if (collision.CompareTag("Player") && _PlayerDamageTime <= 0 )
        {
            _PlayerDamageTime += _PlayerDamageRate;
            collision.gameObject.GetComponent<PlayerController>().Damage(PlayerDamage);
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isZeroHP) return;

        if (collision.gameObject.CompareTag("AcidFlask"))
        {
            Debug.Log(gameObject.name + "の非弱点にヒット");
            SoundManagerV2.Instance.PlaySE(7);
        }

        if (collision.gameObject.CompareTag("Gareki"))
        {
            //animator.SetBool("Stand", false);
            //animator.SetBool("Stun", true);
            //animator.SetBool("BeforeAtack", false);
            //animator.SetBool("Atack", false);
            //animator.SetBool("Jump", false);

            StanTime += _stanRate;
            Debug.Log(gameObject.name + "にガレキがヒットしてスタンした");
            SoundManagerV2.Instance.PlaySE(3);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Damage(PlayerDamage);
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
    }
}
