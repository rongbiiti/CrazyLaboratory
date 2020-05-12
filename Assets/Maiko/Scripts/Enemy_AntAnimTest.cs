using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;

public class Enemy_AntAnimTest : MonoBehaviour {

    private enum Child
    {
        PointA,
        PointB,
        Hit_Body,
        Hit_WeakPoint,
        Hit_Hindlegs,
        Hit_Head,
        PlayerHitBox,
        count,
    }

    [SerializeField, CustomLabel("ヒット時エフェクト")] private GameObject _hitEffect;
    [SerializeField, CustomLabel("ヒット時エフェクト反転")] private GameObject _hitEffectReverse;
    [SerializeField, CustomLabel("死亡時エフェクト")] private GameObject _deathEffect;
    [SerializeField, CustomLabel("死亡時エフェクト反転")] private GameObject _deathEffectReverse;
    [SerializeField] private float _HP = 10f;
    [SerializeField] private float _HitDamage = 2f;
    [SerializeField] private float _PlayerDamage = 2000f;
    [SerializeField] private float _PlayerDamageRate = 3f;
    [SerializeField] private float _nockBuckPower = 300f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
    private float _PlayerDamageTime;
    [SerializeField] private float _MoveSpeed = 0.1f;
    [SerializeField] private bool _directionChange = false;
    private int _direction;
    [SerializeField] private float _directionRate = 3f;  //追跡中の反転時間
    private float directionTime;                    //追跡中の反転時間の格納用
    private bool directionChangeFlag;              //追跡中の反転するかしないか
    [SerializeField] private byte _AttackWait = 1;
    [SerializeField] private float _AttackTime = 0.45f;
    [SerializeField] private float _stanTime = 3f;
    private float stanTimeRemain = 0;
    private float nowHP;
    private Vector3 PointA_Position;
    private Vector3 PointB_Position;
    private byte AttackPhase;
    private float Count;
    private EnemyHpbar enemyHpbar;
    private bool isZeroHP;
    [SerializeField] private float _destroyTime = 2f;
    private Vector2 startPosition;
    private Vector2 startScale;
    private int patrolType;     //0:パトロール 1:追尾 3:攻撃
    [SerializeField] private float _trackingRate = 10f;       //追跡時間
    private float trackingTime;   //追跡時間の格納用
    [SerializeField] private float _tracking = 30f;     //エネミーの追跡範囲
    private GameObject playerObject;  //playerのオブジェクトを格納

    Animator animator;
    private CubismModel Model;
    private float anitime = 0f;

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        PointA_Position = transform.GetChild(0).gameObject.transform.position;
        PointB_Position = transform.GetChild(1).gameObject.transform.position;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        AttackPhase = 0;
        Count = 0;
        nowHP = _HP;
        directionChangeFlag = false;
        if (_directionChange)
        {
            _direction = 1;
            gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        }
        else
        {
            _direction = -1;
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, gameObject.transform.localScale.y);
        }
        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, nowHP);
        patrolType = 0;
        playerObject = GameObject.FindGameObjectWithTag("Player");

        animator = GetComponent<Animator>();
        Model = this.FindCubismModel();

    }

    private void OnEnable()
    {
        if (isZeroHP)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild((int)Child.Hit_Body).gameObject.SetActive(true);
            gameObject.transform.GetChild((int)Child.Hit_WeakPoint).transform.GetComponent<CapsuleCollider2D>().enabled = true;
            gameObject.transform.GetChild((int)Child.Hit_Hindlegs).transform.GetComponent<CapsuleCollider2D>().enabled = true;
            gameObject.transform.GetChild((int)Child.Hit_Head).transform.GetComponent<Collider2D>().enabled = true;
            animator.SetBool("Walk", false);
            animator.SetBool("Atack", false);
            animator.SetBool("Stun", false);
            animator.SetBool("Death", false);
            AttackPhase = 0;
            Count = 0;
            patrolType = 0;
            transform.localScale = startScale;
            nowHP = _HP;
            enemyHpbar.SetBarValue(_HP, nowHP);
            enemyHpbar.hpbar.gameObject.SetActive(true);
            isZeroHP = false;
            directionChangeFlag = false;
            if (_directionChange)
            {
                _direction = 1;
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            }
            else
            {
                _direction = -1;
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            }
        }

    }



    void FixedUpdate()
    {
        if (isZeroHP)
        {
            if (0 < _destroyTime)
            {
                _destroyTime -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
                enemyHpbar.hpbar.gameObject.SetActive(false);
            }
            //if (0 < transform.localScale.x)
            //{
            //    transform.localScale -= new Vector3(startScale.x / _destroyTime * Time.deltaTime, startScale.y / _destroyTime * Time.deltaTime);
            //}
            //else if (transform.localScale.x < 0)
            //{
            //    transform.localScale -= new Vector3(-startScale.x / _destroyTime * Time.deltaTime, startScale.y / _destroyTime * Time.deltaTime);
            //}
            //if (Mathf.Abs(transform.localScale.x) <= startScale.x / 95)
            //{
            //    gameObject.SetActive(false);
            //    enemyHpbar.hpbar.gameObject.SetActive(false);
            //}

        }
        else
        {
            //弱点の点滅タイマー
            if (++anitime < 11){
                Model.Parts[7].Opacity = 1;
                Model.Parts[8].Opacity = 0;
                Model.Parts[9].Opacity = 0;
                Model.Parts[10].Opacity = 0;
            }else if (anitime < 21){
                Model.Parts[7].Opacity = 0;
                Model.Parts[8].Opacity = 1;
                Model.Parts[9].Opacity = 0;
                Model.Parts[10].Opacity = 0;
            }else if (anitime < 31){
                Model.Parts[7].Opacity = 0;
                Model.Parts[8].Opacity = 0;
                Model.Parts[9].Opacity = 1;
                Model.Parts[10].Opacity = 0;
            }else if (anitime < 41){
                Model.Parts[7].Opacity = 0;
                Model.Parts[8].Opacity = 0;
                Model.Parts[9].Opacity = 0;
                Model.Parts[10].Opacity = 1;
            }
            else if (anitime > 51) { anitime = 0f; }

            if (0 < _PlayerDamageTime)
            {
                _PlayerDamageTime -= Time.deltaTime;
            }

            if (0 < stanTimeRemain)
            {
                stanTimeRemain -= Time.deltaTime;
                if (stanTimeRemain <= 0)
                {
                    animator.SetBool("Walk", true);
                    animator.SetBool("Atack", false);
                    animator.SetBool("Stun", false);
                    animator.SetBool("Death", false);
                }
                else
                {
                    animator.SetBool("Walk", false);
                    animator.SetBool("Atack", false);
                    animator.SetBool("Stun", true);
                    animator.SetBool("Death", false);
                }
            }

            if (patrolType == 1 && 0 < trackingTime)
            {
                trackingTime -= Time.deltaTime;
                if (trackingTime <= 0)
                {
                    patrolType = 0;
                }
            }

            if (directionChangeFlag && 0 < directionTime)
            {
                directionTime -= Time.deltaTime;
                //Debug.Log("方向転換準備");
                if (directionTime <= 0)
                {
                    directionChangeFlag = false;
                    _direction *= -1;
                    gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                }
            }

            // transformを取得
            Transform myTransform = this.transform;

            switch (patrolType)
            {
                case 0:     //パトロールの動き

                    if (PointA_Position.x >= transform.position.x && AttackPhase == 0 && !_directionChange && stanTimeRemain <= 0)
                    {
                        _direction *= -1;
                        _directionChange = true;
                        gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    }
                    else if (PointB_Position.x <= transform.position.x && AttackPhase == 0 && _directionChange && stanTimeRemain <= 0)
                    {
                        _direction *= -1;
                        _directionChange = false;
                        gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    }

                    if (stanTimeRemain <= 0)
                        // 現在の座標からのxyz を _MoveSpeed ずつ加算して移動
                        myTransform.Translate(_MoveSpeed * _direction, 0.0f, 0.0f, Space.World);
                    break;

                case 1: //追尾の動き playerを追いかける

                    if (playerObject.transform.position.x >= transform.position.x && AttackPhase == 0 && !_directionChange && stanTimeRemain <= 0)
                    {
                        directionChangeFlag = true;
                        directionTime = _directionRate;
                        //_direction *= -1;
                        _directionChange = true;
                        //gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                    }
                    else if (playerObject.transform.position.x <= transform.position.x && AttackPhase == 0 && _directionChange && stanTimeRemain <= 0)
                    {
                        directionChangeFlag = true;
                        directionTime = _directionRate;

                        //_direction *= -1;
                        _directionChange = false;
                        //gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
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
                    if (AttackPhase == 1 && stanTimeRemain <= 0)
                    {
                        animator.SetBool("Walk", false);
                        animator.SetBool("Atack", true);
                        animator.SetBool("Stun", false);
                        animator.SetBool("Death", false);

                        // 現在の座標からのxyz を1ずつ加算して移動
                        //myTransform.Translate(0.001f * gameObject.transform.localScale.x, 0.0f, 0.0f, Space.World);
                        Count += Time.deltaTime;
                        if (Count >= _AttackWait)
                        {
                            AttackPhase = 2;
                            Count = 0;
                        }
                    }
                    else if (AttackPhase == 2 && stanTimeRemain <= 0)
                    {
                        animator.SetBool("Walk", true);
                        animator.SetBool("Atack", false);
                        animator.SetBool("Stun", false);
                        animator.SetBool("Death", false);

                        myTransform.Translate(0.2f * _direction, 0.0f, 0.0f, Space.World);
                        Count += Time.deltaTime;
                        if (Count >= _AttackTime)
                        {
                            AttackPhase = 0;
                            Count = 0;
                            stanTimeRemain += 2;
                            patrolType = 1;
                            trackingTime = _trackingRate + stanTimeRemain;
                        }

                    }
                    break;
            }


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
            SoundManagerV2.Instance.PlaySE(36);
            enemyHpbar.SetBarValue(_HP, nowHP);
            if (nowHP <= 0)
            {
                isZeroHP = true;
                animator.SetBool("Walk", false);
                animator.SetBool("Atack", false);
                animator.SetBool("Stun", false);
                animator.SetBool("Death", true);
                ScoreManager.Instance.KillCnt++;
                ScoreManager.Instance.TotalKillCnt++;
                gameObject.transform.GetChild((int)Child.Hit_Body).gameObject.SetActive(false);
                gameObject.transform.GetChild((int)Child.Hit_WeakPoint).transform.GetComponent<CapsuleCollider2D>().enabled = false;
                gameObject.transform.GetChild((int)Child.Hit_Hindlegs).transform.GetComponent<CapsuleCollider2D>().enabled = false;
                gameObject.transform.GetChild((int)Child.Hit_Head).transform.GetComponent<Collider2D>().enabled = false;
                if(0 < transform.localScale.x) Instantiate(_deathEffect, transform.position, Quaternion.identity);
                else Instantiate(_deathEffectReverse, transform.position, Quaternion.identity);
                SoundManagerV2.Instance.PlaySE(28);
                SoundManagerV2.Instance.PlaySE(37);
            } else {
                if (0 < transform.localScale.x) {
                    GameObject obj = Instantiate(_hitEffect, transform.position, Quaternion.identity) as GameObject;
                    obj.transform.parent = transform;
                } else {
                    GameObject obj = Instantiate(_hitEffectReverse, transform.position, Quaternion.identity) as GameObject;
                    obj.transform.parent = transform;
                }
                
            }
        }

        if (collision.CompareTag("Player") && patrolType == 0)   //パトロール中にplayerを見つけた時
        {
            patrolType = 1;     //敵を見つけて追いかけるモード
            trackingTime = _trackingRate;
        }

        if (collision.CompareTag("Player") && patrolType == 1 && AttackPhase == 0 && stanTimeRemain <= 0)
        {
            AttackPhase = 1;
            patrolType = 2;
            SoundManagerV2.Instance.PlaySE(27);
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
                SoundManagerV2.Instance.PlaySE(2);
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isZeroHP) return;

        if (collision.gameObject.CompareTag("AcidFlask"))
        {
            Debug.Log(gameObject.name + "の非弱点にヒット");
            SoundManagerV2.Instance.PlaySE(7);
            if (patrolType == 0)   //パトロール中にplayerを見つけた時
            {
                patrolType = 1;     //敵を見つけて追いかけるモード
                trackingTime = _trackingRate;
            }

        }

        if (collision.gameObject.CompareTag("Gareki"))
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Atack", false);
            animator.SetBool("Stun", true);
            animator.SetBool("Death", false);

            stanTimeRemain += _stanTime;
            ScoreManager.Instance.StunCnt++;
            ScoreManager.Instance.TotalStunCnt++;
            AttackPhase = 0;
            Count = 0;
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
            if (!collision.gameObject.GetComponent<PlayerController>().IsNotNockBack)
            {
                prb.velocity = direction * _nockBuckPower;
            }

            SoundManagerV2.Instance.PlaySE(2);
        }
    }
}
