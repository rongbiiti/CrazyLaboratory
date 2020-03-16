using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField, Range(0f, 100f)] private float _moveSpeed = 1f;
    [SerializeField] private float _moveForceMultiplier = 20f;
    [SerializeField] private float _HP = 4f;
    [SerializeField] private float _bulletHitDamage = 1f;
    [SerializeField] private GameObject _pointA;
    [SerializeField] private GameObject _pointB;
    private Rigidbody2D rb;
    private EnemyHpbar enemyHpbar;
    private float damageTime;
    private Vector2 startScale;
    public Vector3 targetPosition;
    private bool isReachTargetPosition;
    private bool istargetPointA;

    void Start () {
        startScale = transform.localScale;
        targetPosition = _pointA.transform.position;
        rb = GetComponent<Rigidbody2D>();
        enemyHpbar = GetComponent<EnemyHpbar>();
        enemyHpbar.SetBarValue(_HP, _HP);
    }

    private void FixedUpdate()
    {
        if (0 < damageTime) {
            if (0 < transform.localScale.x) {
                transform.localScale -= new Vector3(startScale.x / _HP * Time.deltaTime, startScale.y / _HP * Time.deltaTime);
            } else if (transform.localScale.x < 0) {
                transform.localScale -= new Vector3(-startScale.x / _HP * Time.deltaTime, startScale.y / _HP * Time.deltaTime);
            }
            if (Mathf.Abs(transform.localScale.x) <= startScale.x / 95) {
                Destroy(gameObject);
            }
            damageTime -= Time.deltaTime;
        }

        DecideTargetPotision();
        // 巡回ポイントの位置を取得
        Vector2 targetPos = targetPosition;
        // 巡回ポイントのx座標
        float x = targetPos.x;
        // ENEMYは、地面を移動させるので座標は常に0とする
        float y = 0;
        // 移動を計算させるための２次元のベクトルを作る
        Vector2 direction = new Vector2(x - transform.position.x, y).normalized;
        // ENEMYのRigidbody2Dに移動速度を指定する
        rb.velocity = direction * _moveSpeed;

        /* transform.position = Vector3.Slerp(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
        if(transform.position.x == targetPosition.x) {
            isReachTargetPosition = true;
        } */
        
    }

    private void DecideTargetPotision()
    {
        // まだ目的地についてなかったら（移動中なら）目的地を変えない
        if (!isReachTargetPosition) {
            return;
        }

        // 目的地に着いていたら目的地を再設定する
        if (istargetPointA) {
            targetPosition = _pointA.transform.position;
            istargetPointA = false;
            _pointA.SetActive(true);
            _pointB.SetActive(false);
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y);
        } else {
            targetPosition = _pointB.transform.position;
            istargetPointA = true;
            _pointA.SetActive(false);
            _pointB.SetActive(true);
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y);
        }
        
        isReachTargetPosition = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PatrolPoint")) {
            isReachTargetPosition = true;
            DecideTargetPotision();
        }

        // 弱点のみ、IsTriggerをオンにしている。
        if (collision.CompareTag("AcidFlask")) {
            damageTime += _bulletHitDamage;
            Destroy(collision.gameObject);
            Debug.Log(gameObject.name + "の弱点にヒット");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AcidFlask")) {
            Destroy(collision.gameObject);
            Debug.Log(gameObject.name + "の非弱点にヒット");
        }
    }
}
