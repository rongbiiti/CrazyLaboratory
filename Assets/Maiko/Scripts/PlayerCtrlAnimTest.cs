using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// プレイヤーのスクリプト
/// </summary>
public class PlayerCtrlAnimTest : MonoBehaviour
{
    private GameObject cam;
    private Rigidbody2D rb;
    [SerializeField, CustomLabel("HPバー")] private Slider _HPbar;
    [SerializeField, CustomLabel("残弾数UI")] private Text _bulletsRemain;

    private bool flip = true;

    //アニメーション関連
    Animator animator;
    private State state;
    private int GunLayer;
    private int Shot1Layer;
    private int Shot2Layer;
    private int Shot3Layer;
    private float weightGun;
    private float weight1;
    private float weight2;
    private float weight3;
    private bool smoothFlag;
    enum State
    {
        None,
        Gun,
        Shot1,
        Shot2,
        Shot3
    }

    [SerializeField, CustomLabel("地面との当たり判定")] private ContactFilter2D filter2d;
    private bool isGrounded = true;

    [SerializeField, CustomLabel("弾のプレハブ")] private GameObject _acidbulletPrefab;
    [SerializeField, Range(0.001f, 9999f), CustomLabel("最大HP")] private float _maxHP = 9999f;
    [SerializeField, Range(0f, 9999f), CustomLabel("酸に触れたときの被ダメージ")] private float _acidDamage = 500f;
    [SerializeField, Range(0.0167f, 10f), CustomLabel("酸の被ダメージレート")] private float _acidDamageRate = 0.5f;
    [SerializeField, Range(0, 999), CustomLabel("弾の最大所持数")] private int _bulletCapacity = 10;
    [SerializeField, Range(0f, 5f), CustomLabel("弾の発射間隔")] private float _fireRate = 0f;

    [SerializeField, Range(0.001f, 1f), CustomLabel("スティック上向きの閾値")] private float YStickUpDeadZone = 0.4f;
    [SerializeField, Range(-0.001f, -1f), CustomLabel("スティック下向きの閾値")] private float _YStickDownDeadZone = -0.4f;

    [Serializable]
    private class UpShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・上")] private float _muzzleVelocity = 15f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・上")] private float _gravityScale = 2f;
        [SerializeField, Range(0f, 90f), CustomLabel("発射角度・上")] private float _fireAngle = 45f;

        public float MuzzleVelocity
        {
            get { return _muzzleVelocity; }
        }
        public float GravityScale
        {
            get { return _gravityScale; }
        }
        public float FireAngle
        {
            get { return _fireAngle; }
        }
    }
    [SerializeField, CustomLabel("上へ発射")] UpShot upShot;

    [Serializable]
    private class HorizontalShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・水平")] private float _muzzleVelocity = 15f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・水平")] private float _gravityScale = 2f;
        [SerializeField, Range(-45f, 45f), CustomLabel("発射角度・水平")] private float _fireAngle = 0f;

        public float MuzzleVelocity
        {
            get { return _muzzleVelocity; }
        }
        public float GravityScale
        {
            get { return _gravityScale; }
        }
        public float FireAngle
        {
            get { return _fireAngle; }
        }
    }
    [SerializeField, CustomLabel("水平に発射")] HorizontalShot horizontalShot;

    [Serializable]
    private class DownShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・下")] private float _muzzleVelocity = 15f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・下")] private float _gravityScale = 2f;
        [SerializeField, Range(0f, -90f), CustomLabel("発射角度・下")] private float _fireAngle = -45f;

        public float MuzzleVelocity
        {
            get { return _muzzleVelocity; }
        }
        public float GravityScale
        {
            get { return _gravityScale; }
        }
        public float FireAngle
        {
            get { return _fireAngle; }
        }
    }
    [SerializeField, CustomLabel("下へ発射")] DownShot downShot;

    private float muzzleVelocity = 5f;
    private bool isGetGun = false;
    private Vector3 mainThrowPoint;
    private float HP;
    private int bullets;
    public bool IsBulletsFull()
    {
        if (bullets >= _bulletCapacity)
        {
            return true;
        }
        return false;
    }
    private float fireTime;
    private float acidDamageTime;

    private float startMoveSpeed;
    private bool isJumping = false;
    private bool isJumpingCheck = true;
    private float jumpTimeCounter;
    private float _jumpPower;

    private Vector3 restartPoint;

    InputManager inputManager;
    PlayerManager playerManager;

    private float groundCount = 0f;

    void Start()
    {
        inputManager = InputManager.Instance;
        playerManager = PlayerManager.Instance;
        jumpTimeCounter = playerManager.JumpTime;
        rb = GetComponent<Rigidbody2D>();
        startMoveSpeed = playerManager.MoveSpeed;
        HP = _maxHP;
        _HPbar.maxValue = _maxHP;
        _HPbar.value = HP;
        _bulletsRemain.text = " ∞ ";
        _bulletsRemain.enabled = false;
        cam = GameObject.Find("Main Camera");
        SoundManagerV2.Instance.PlayBGM(0);

        //アニメーション関連
        animator = GetComponent<Animator>();
        GunLayer = animator.GetLayerIndex("Gun Layer");
        Shot1Layer = animator.GetLayerIndex("Shot1 Layer");
        Shot2Layer = animator.GetLayerIndex("Shot2 Layer");
        Shot3Layer = animator.GetLayerIndex("Shot3 Layer");
        weightGun = 0f;
        weight1 = 0f;
        weight2 = 0f;
        weight3 = 0f;
        SetState(State.None, first: true);
    }

    void Update()
    {

        // 上に発射
        if (YStickUpDeadZone < inputManager.UpMoveKey && isGetGun)
        {
            mainThrowPoint = transform.GetChild(0).transform.position;
            if (state != State.Shot1) {
                SetState(State.Shot1);
            }   
            Debug.Log("shot1");
        }// 下に発射
        else if (inputManager.UpMoveKey < _YStickDownDeadZone && isGetGun)
        {
            mainThrowPoint = transform.GetChild(2).transform.position;
            if (state != State.Shot3){
                SetState(State.Shot3);
            }
            Debug.Log("shot3");
        }// 正面に発射
        else if(isGetGun)
        {
            mainThrowPoint = transform.GetChild(1).transform.position;
            if (state != State.Gun){
                SetState(State.Gun);
            }
        }

        if (smoothFlag) {
            if (state == State.Shot1){
                weight1 = 1f;
                weight2 = 0f;
                weight3 = 0f;
                weightGun = 0f;
            }else if(state == State.Gun){
                weightGun = 1f;
                weight1 = 0f;
                weight2 = 0f;
                weight3 = 0f;
            }else if (state == State.Shot2){
                weight2 = 1f;
                weightGun = 0f;
                weight1 = 0f;
                weight3 = 0f;
            }else if (state == State.Shot3){
                weight3 = 1f;
                weightGun = 0f;
                weight1 = 0f;
                weight2 = 0f;
            }
            animator.SetLayerWeight(GunLayer, weightGun);
            animator.SetLayerWeight(Shot1Layer, weight1);
            animator.SetLayerWeight(Shot2Layer, weight2);
            animator.SetLayerWeight(Shot3Layer, weight3);
        }
        

        if (inputManager.MoveStopKey != 0)
        {
            playerManager.MoveSpeed = 0;
        }
        else
        {
            playerManager.MoveSpeed = startMoveSpeed;
        }

        // 地面と当たり判定をしている。
        isGrounded = rb.IsTouching(filter2d);

        if (isJumpingCheck && inputManager.JumpKey == 1 && isGrounded)
        {
            jumpTimeCounter = playerManager.JumpTime;
            isJumpingCheck = false;
            isJumping = true;
            _jumpPower = playerManager.JumpPower;
            SoundManagerV2.Instance.PlaySE(9);
        }

        if (inputManager.MoveKey >= 0.3 && !flip)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = true;
        }
        if (inputManager.MoveKey <= -0.3 && flip)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = false;
        }

        if (inputManager.ShotKey == 1 && fireTime <= 0 && 0 == bullets && isGetGun)
        {

            GameObject bullet = Instantiate(_acidbulletPrefab, mainThrowPoint, Quaternion.identity) as GameObject;
            Rigidbody2D bRb = bullet.GetComponent<Rigidbody2D>();

            float rad = 0;

            // 上に発射
            if (YStickUpDeadZone < inputManager.UpMoveKey)
            {
                rad = upShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
                muzzleVelocity = upShot.MuzzleVelocity; //上へ発射時の初速を代入
                bRb.gravityScale = upShot.GravityScale; //上へ発射時の弾の重量を代入
            }// 下に発射
            else if (inputManager.UpMoveKey < _YStickDownDeadZone)
            {
                rad = downShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
                muzzleVelocity = downShot.MuzzleVelocity; //下へ発射時の初速を代入
                bRb.gravityScale = downShot.GravityScale; //下へ発射時の弾の重量を代入
            }// 正面に発射
            else
            {
                rad = horizontalShot.FireAngle * Mathf.Deg2Rad;　//角度をラジアン角に変換
                muzzleVelocity = horizontalShot.MuzzleVelocity;  //正面へ発射時の初速を代入
                bRb.gravityScale = horizontalShot.GravityScale;  //正面へ発射時の弾の重量を代入          
            }

            //rad(ラジアン角)から発射用ベクトルを作成
            double addforceX = Math.Cos(rad * 1);
            double addforceY = Math.Sin(rad * 1);
            Vector3 shotangle = new Vector3((float)addforceX, (float)addforceY, 0);

            // キャラが反対向いてたら反対に発射させる
            if (!flip)
            {
                shotangle = Vector3.Reflect(shotangle, Vector3.right);
            }

            //　発射角度にオブジェクトを回転させる、進んでいる方向と角度を一致させる
            var diff = shotangle - bullet.transform.position;
            var axis = Vector3.Cross(bullet.transform.right, shotangle);
            var angle = Vector3.Angle(bullet.transform.right, shotangle) * (axis.z < 0 ? -1 : 1);
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //　で、でますよ
            bRb.AddForce(shotangle * muzzleVelocity, ForceMode2D.Force);
            SoundManagerV2.Instance.PlaySE(6);
            //--bullets;
            _bulletsRemain.text = " ∞ ";
            fireTime += _fireRate;
        }

    }

    void FixedUpdate()
    {
        if (0 < fireTime)
        {
            fireTime -= Time.deltaTime;
        }

        if (0 < acidDamageTime)
        {
            acidDamageTime -= Time.deltaTime;
        }

        // 地面にいるとき
        if (isGrounded)
        {
            
            animator.SetBool("JumpUp", false);
            rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.MoveSpeed - rb.velocity.x), rb.velocity.y));
           if(inputManager.MoveKey != 0)
            {
                //Debug.Log("顔が消えた");
                animator.SetBool("Run", true);
                animator.SetBool("Stand", false);
            }
            else if(inputManager.MoveKey == 0 && rb.velocity.x <= 4f && -4f <= rb.velocity.x)
           {
                animator.SetBool("Stand", true);
                animator.SetBool("Run", false);
            }
            // 空中にいるとき
        }
        else
        {
            animator.SetBool("JumpUp", true);
            animator.SetBool("Run", false);
            animator.SetBool("Stand", false);

            groundCount = 0;
            // ジャンプキーが話されたらジャンプ中でないことにする
            if (inputManager.JumpKey == 0)
            {
                isJumping = false;
            }
            // ジャンプしてない
            if (!isJumping)
            {
                
                // 落ちる力が一定を超えるとそれ以上落下速度が上がらないようにする
                if (rb.velocity.y <= -playerManager.MaxFallSpeed)
                {
                    rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), 0));
                }
                else if (rb.velocity.y <= 0)
                {
                    // 落ち始めると重力を使って落ちるようにする
                    rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), Physics.gravity.y * playerManager.GravityRate));
                }
                else
                {
                    // ジャンプパワーがまだあるときは小ジャンプ実現のためにジャンプパワー軽減率を使う
                    if (0 <= _jumpPower)
                    {
                        _jumpPower -= playerManager.JumpPowerAttenuation * 2;
                        rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), 1 * _jumpPower));
                        // ジャンプパワーがないときは重力を使って落とす
                    }
                    else
                    {
                        rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), Physics.gravity.y * playerManager.GravityRate));
                    }
                }
            }
        }

        // ジャンプ中
        if (isJumping)
        {

            // ジャンプキーを押し続けていられる時間をへらす
            jumpTimeCounter -= Time.deltaTime;
            
            animator.SetBool("Run", false);

            // ジャンプキーを押し続けている間は通常のジャンプパワー軽減率がはたらく
            if (inputManager.JumpKey == 2)
            {
                _jumpPower -= playerManager.JumpPowerAttenuation;
                rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), 1 * _jumpPower));
            }
            else if (inputManager.JumpKey == 0)
            {
                _jumpPower -= playerManager.JumpPowerAttenuation;

            }
            // ジャンプキーを押し続けていられる時間がくると、ジャンプ中を解除する
            if (jumpTimeCounter < 0)
            {
                isJumping = false;
                //animator.SetTrigger("JumpDown0");
            }
            // 下に落ちているときはジャンプ中を解除
            if (rb.velocity.y <= -1)
            {
                isJumping = false;
                //animator.SetTrigger("JumpDown0");
            }
        }

        if (inputManager.JumpKey == 0)
        {
            isJumpingCheck = true;
        }


    }

    public void Damage(float damage)
    {
        HP -= damage;
        _HPbar.value = HP;
        Debug.Log("HP " + HP + " / " + _maxHP);
    }

    public void Heal(float healPercent)
    {

        HP += HP * (healPercent / 100);
        _HPbar.value = HP;
        Debug.Log("HP " + HP + " / " + _maxHP);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnCollisionStay2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemBullet"))
        {
            Debug.Log("アイテム取得");
            ItemScript itemScript = collision.gameObject.GetComponent<ItemScript>();
            bullets += itemScript.GetBulletsInItem();

            if (_bulletCapacity < bullets)
            {
                itemScript.SetBulletsInItem(bullets - _bulletCapacity);
                bullets = _bulletCapacity;
            }
            else
            {
                itemScript.SetBulletsInItem(0);
                Destroy(collision.gameObject);
            }

            _bulletsRemain.text = " ∞ ";
        }
        else if (collision.CompareTag("PutGun"))
        {
            _bulletsRemain.enabled = true;
            isGetGun = true;
            if (state != State.Gun)
            {
                SetState(State.Gun);
            }
            Destroy(collision.gameObject);
            SoundManagerV2.Instance.PlaySE(12);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "ResidualAcid")
        {
            if (acidDamageTime <= 0)
            {
                GameObject acidParentBlock = collision.transform.parent.gameObject;
                var sprite = acidParentBlock.GetComponent<SpriteRenderer>();
                var _sprite = sprite.sprite;
                var _halfX = _sprite.bounds.extents.x;
                var _halfY = _sprite.bounds.extents.y;
                var _vec = new Vector3(-_halfX, 0f, 0f); // これは左上
                var _unvec = new Vector3(_halfX, 0f, 0f); // これは右上
                var _pos = sprite.transform.TransformPoint(_vec);
                var _unpos = sprite.transform.TransformPoint(_unvec);
                if (transform.position.x >= _pos.x && transform.position.x <= _unpos.x)
                {
                    acidDamageTime += _acidDamageRate;
                    Damage(_acidDamage);
                    SoundManagerV2.Instance.PlaySE(4);
                    Debug.Log("酸に触れて " + _acidDamage + " ダメージを受けた");
                    Debug.Log(transform.position.x);
                    Debug.Log(_pos.x);
                    Debug.Log(_unpos.x);
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
                var _halfX = _sprite.bounds.extents.x;
                var _halfY = _sprite.bounds.extents.y;
                var _vec = new Vector3(0f, -_halfY, 0f); // これは上
                var _unvec = new Vector3(0f, _halfY, 0f); // これは下
                var _pos = sprite.transform.TransformPoint(_vec);
                var _unpos = sprite.transform.TransformPoint(_unvec);
                if (transform.position.y >= _pos.y && transform.position.y <= _unpos.y)
                {
                    acidDamageTime += _acidDamageRate;
                    Damage(_acidDamage);
                    SoundManagerV2.Instance.PlaySE(4);
                    Debug.Log("酸に触れて " + _acidDamage + " ダメージを受けた");
                    Debug.Log(transform.position.y);
                    Debug.Log(_pos.y);
                    Debug.Log(_unpos.y);
                }
            }
        }
    }

    void SetState(State state, bool first = false)
    {
        this.state = state;
        //　最初の設定でなければスムーズフラグをオンにする
        if (!first) {
            smoothFlag = true;
        }
    }

}