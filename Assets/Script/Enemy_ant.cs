using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ant : MonoBehaviour {
    
    [SerializeField] private float _HP;
    [SerializeField] private float _HitDamage;
    [SerializeField] private float _PlayerDamage;
    [SerializeField] private float _PlayerDamageRate;
    [SerializeField] private float _nockBuckPower = 300f;
    [SerializeField] private float _nockBuckUpperPower = 0.38f;
    private float _PlayerDamageTime;
    [SerializeField] private float _MoveSpeed;
    [SerializeField] private bool _directionChange;
    private int _direction;
    [SerializeField] private byte _AttackWait;
    [SerializeField] private float _AttackTime;
    [SerializeField] private float _stanTime = 3f;
    private float stanTimeRemain = 0;
    private float nowHP;
    private Vector3 PointA_Position;
    private Vector3 PointB_Position;
    private byte AttackPhase;
    private float Count;
    private EnemyHpbar enemyHpbar;
    private bool isZeroHP;
    [SerializeField] private float destroyTime = 2f;
    private Vector2 startScale;

    // Use this for initialization
    void Start () {
        startScale = transform.localScale;
        PointA_Position = transform.GetChild(0).gameObject.transform.position;
        PointB_Position = transform.GetChild(1).gameObject.transform.position;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        AttackPhase = 0;
        Count = 0;
        nowHP = _HP;
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
    }

    void FixedUpdate()
    {
        if (isZeroHP) {
            if (0 < transform.localScale.x) {
                transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            } else if (transform.localScale.x < 0) {
                transform.localScale -= new Vector3(-startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            }
            if (Mathf.Abs(transform.localScale.x) <= startScale.x / 95) {
                Destroy(enemyHpbar.hpbar.gameObject);
                Destroy(gameObject);
            }

        } else {
            if (0 < _PlayerDamageTime) {
                _PlayerDamageTime -= Time.deltaTime;
            }

            if (0 < stanTimeRemain) {
                stanTimeRemain -= Time.deltaTime;
            }

            if (PointA_Position.x >= transform.position.x && AttackPhase == 0 && !_directionChange && stanTimeRemain <= 0) {
                _direction *= -1;
                _directionChange = true;
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            } else if (PointB_Position.x <= transform.position.x && AttackPhase == 0 && _directionChange && stanTimeRemain <= 0) {
                _direction *= -1;
                _directionChange = false;
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
            }

            // transformを取得
            Transform myTransform = this.transform;
            if (AttackPhase == 0 && stanTimeRemain <= 0) {
                // 現在の座標からのxyz を _MoveSpeed ずつ加算して移動
                myTransform.Translate(_MoveSpeed * _direction, 0.0f, 0.0f, Space.World);
            } else if (AttackPhase == 1 && stanTimeRemain <= 0) {
                // 現在の座標からのxyz を1ずつ加算して移動
                //myTransform.Translate(0.001f * gameObject.transform.localScale.x, 0.0f, 0.0f, Space.World);
                Count += Time.deltaTime;
                if (Count >= _AttackWait) {
                    AttackPhase = 2;
                    Count = 0;
                }
            } else if (AttackPhase == 2 && stanTimeRemain <= 0) {
                myTransform.Translate(0.2f * gameObject.transform.localScale.x * -1, 0.0f, 0.0f, Space.World);
                //AttackPhase = 0;
                Count += Time.deltaTime;
                if (Count >= _AttackTime) {
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
            SoundManagerV2.Instance.PlaySE(4);
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
            SoundManagerV2.Instance.PlaySE(2);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AcidFlask"))
        {
            Destroy(collision.gameObject);
            Debug.Log(gameObject.name + "の非弱点にヒット");
            SoundManagerV2.Instance.PlaySE(7);
        }

        if(collision.gameObject.CompareTag("Gareki")) {
            stanTimeRemain += _stanTime;
            AttackPhase = 0;
            Count = 0;
            Debug.Log(gameObject.name + "にガレキがヒットしてスタンした");
            SoundManagerV2.Instance.PlaySE(3);
        }
    }
}
