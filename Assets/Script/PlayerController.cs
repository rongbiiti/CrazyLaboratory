using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

/// <summary>
/// プレイヤーのスクリプト
/// </summary>
public class PlayerController : MonoBehaviour
{
    private GameObject cam;
    private Rigidbody2D rb;
    [SerializeField, CustomLabel("HPバー")] private Slider _HPbar;
    [SerializeField, CustomLabel("残弾数UI")] private Text _bulletsRemain;
    [SerializeField, CustomLabel("ハンドガン装備UI")] private Button _handgunUI;
    [SerializeField, CustomLabel("ホールメイカー装備UI")] private Button _hmUI;

    private bool flip = true;

    [SerializeField, CustomLabel("地面との当たり判定")] private ContactFilter2D filter2d;
    private bool isGrounded = true;

    [SerializeField, CustomLabel("弾のプレハブ")] private GameObject _acidbulletPrefab;
    [SerializeField, Range(0.001f, 9999f), CustomLabel("最大HP")] private float _maxHP = 9999f;
    [SerializeField, Range(0f, 9999f), CustomLabel("酸に触れたときの被ダメージ")] private float _acidDamage = 500f;
    [SerializeField, Range(0.0167f, 10f), CustomLabel("酸の被ダメージレート")] private float _acidDamageRate = 0.5f;
    [SerializeField, Range(0, 999), CustomLabel("弾の最大所持数")] private int _bulletCapacity = 10;
    [SerializeField, CustomLabel("ホールメイカー弾最大所持数")] private int _hmBulletCapacity = 20;
    [Header("同時発射数は奇数だった場合-1された偶数に自動調整されます。")]
    [SerializeField, CustomLabel("ホールメイカー弾同時発射数")] private int _hmShotBullets = 6;
    [SerializeField, CustomLabel("ホールメイカー拡散範囲")] private float _hmSpreadRange = 45f;
    [SerializeField, CustomLabel("ホールメイカー発射間隔")] private float _hmFireRate = 2.5f;
    [SerializeField, Range(0f, 5f), CustomLabel("弾の発射間隔")] private float _fireRate = 0f;

    [SerializeField, Range(0.001f, 1f), CustomLabel("スティック上向きの閾値")] private float YStickUpDeadZone = 0.4f;
    [SerializeField, Range(-0.001f, -1f), CustomLabel("スティック下向きの閾値")] private float _YStickDownDeadZone = -0.4f;

    [Serializable]
    private class UpShot
    {
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・上")] private float _muzzleVelocity = 11.5f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・上")] private float _gravityScale = 6f;
        [SerializeField, Range(0f, 90f), CustomLabel("発射角度・上")] private float _fireAngle = 63f;

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
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・水平")] private float _muzzleVelocity = 11f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・水平")] private float _gravityScale = 12f;
        [SerializeField, Range(-45f, 45f), CustomLabel("発射角度・水平")] private float _fireAngle = 20f;

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
        [SerializeField, Range(0f, 100f), CustomLabel("砲口初速・下")] private float _muzzleVelocity = 11f;
        [SerializeField, Range(0f, 30f), CustomLabel("弾の落下しやすさ・下")] private float _gravityScale = 5f;
        [SerializeField, Range(0f, -90f), CustomLabel("発射角度・下")] private float _fireAngle = -20f;

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

    private enum Equipment
    {
        None,
        Handgun,
        HoleMaker
    }
    Equipment equipment;

    private List<float> hmSpreadAngle = new List<float>();
    private float muzzleVelocity = 5f;
    private bool isGetGun = false;
    private bool isGetHoleMaker = false;
    private Vector3 mainThrowPoint;
    private float HP;
    private int bullets;
    private int hmBullets;
    public bool IsBulletsFull()
    {
        if (bullets >= _bulletCapacity) {
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
        if (_hmShotBullets % 2 != 0) {
            _hmShotBullets--;
        }
        PreCalcSpreadAngle();

        ShowListContentsInTheDebugLog(hmSpreadAngle);
    }

    public void ShowListContentsInTheDebugLog<T>(List<T> list)
    {
        string log = "";

        foreach (var content in list.Select((val, idx) => new { val, idx })) {
            if (content.idx == list.Count - 1)
                log += content.val.ToString();
            else
                log += content.val.ToString() + ", ";
        }

        Debug.Log(log);
    }


    void Update()
    {

        // 上に発射
        if (YStickUpDeadZone < inputManager.UpMoveKey && isGetGun) {
            mainThrowPoint = transform.GetChild(0).transform.position;
            
            // 下に発射
        } else if (inputManager.UpMoveKey < _YStickDownDeadZone && isGetGun) {
            mainThrowPoint = transform.GetChild(2).transform.position;
            // 正面に発射
        } else {
            mainThrowPoint = transform.GetChild(1).transform.position;
        }

        //if (inputManager.MoveStopKey != 0) {
        //    playerManager.MoveSpeed = 0;
        //} else {
        //    playerManager.MoveSpeed = startMoveSpeed;
        //}

        // 地面と当たり判定をしている。
        isGrounded = rb.IsTouching(filter2d);

        if (isJumpingCheck && inputManager.JumpKey == 1 && isGrounded) {
            jumpTimeCounter = playerManager.JumpTime;
            isJumpingCheck = false;
            isJumping = true;
            _jumpPower = playerManager.JumpPower;
            SoundManagerV2.Instance.PlaySE(9);
        }

        if (inputManager.MoveKey >= 0.3 && !flip && inputManager.MoveStopKey == 0) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = true;
        }
        if (inputManager.MoveKey <= -0.3 && flip && inputManager.MoveStopKey == 0) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = false;
        }
        
        if (inputManager.ShotKey == 1 && fireTime <= 0) {

            if(equipment == Equipment.Handgun && isGetGun) {
                HandgunShot();
            } else if(equipment == Equipment.HoleMaker && isGetHoleMaker && 0 < hmBullets) {
                HoleMakerShot();
            }
        }

        if (inputManager.EquipHandGun && isGetGun) {
            _handgunUI.Select();
            equipment = Equipment.Handgun;
            _bulletsRemain.text = " ∞ ";
        }
        if (inputManager.EquipHoleMaker && isGetHoleMaker && 0 < hmBullets) {
            _hmUI.Select();
            equipment = Equipment.HoleMaker;
            _bulletsRemain.text = hmBullets + " / " + _hmBulletCapacity;
        }
 
    }

    void FixedUpdate()
    {
        if(0 < fireTime) {
            fireTime -= Time.deltaTime;
        }

        if(0 < acidDamageTime) {
            acidDamageTime -= Time.deltaTime;
        }

        // 地面にいるとき
        if (isGrounded) {
            rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.MoveSpeed - rb.velocity.x), rb.velocity.y));

        // 空中にいるとき
        } else {
            // ジャンプキーが話されたらジャンプ中でないことにする
            if (inputManager.JumpKey == 0) {
                isJumping = false;
            }
            // ジャンプしてない
            if (!isJumping) {
                // 落ちる力が一定を超えるとそれ以上落下速度が上がらないようにする
                if(rb.velocity.y <= -playerManager.MaxFallSpeed) {
                    rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), 0));
                } else if(rb.velocity.y <= 0) {
                    // 落ち始めると重力を使って落ちるようにする
                    rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), Physics.gravity.y * playerManager.GravityRate));
                } else {
                    // ジャンプパワーがまだあるときは小ジャンプ実現のためにジャンプパワー軽減率を使う
                    if (0 <= _jumpPower) {
                        _jumpPower -= playerManager.JumpPowerAttenuation * 2;
                        rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), 1 * _jumpPower));
                    // ジャンプパワーがないときは重力を使って落とす
                    } else {
                        rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), Physics.gravity.y * playerManager.GravityRate));
                    }
                }
            }
        }

        // ジャンプ中
        if (isJumping) {
            
            // ジャンプキーを押し続けていられる時間をへらす
            jumpTimeCounter -= Time.deltaTime;

            // ジャンプキーを押し続けている間は通常のジャンプパワー軽減率がはたらく
            if (inputManager.JumpKey == 2) {
                _jumpPower -= playerManager.JumpPowerAttenuation;
                rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.JumpMoveSpeed - rb.velocity.x), 1 * _jumpPower));
            } else if(inputManager.JumpKey == 0) {
                _jumpPower -= playerManager.JumpPowerAttenuation;

            }
            // ジャンプキーを押し続けていられる時間がくると、ジャンプ中を解除する
            if (jumpTimeCounter < 0) {
                isJumping = false;
            }
            // 下に落ちているときはジャンプ中を解除
            if (rb.velocity.y <= -1) {
                isJumping = false;
            }
        }

        if (inputManager.JumpKey == 0) {
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

    // ハンドガン発射
    private void HandgunShot()
    {
        GameObject bullet = Instantiate(_acidbulletPrefab, mainThrowPoint, Quaternion.identity) as GameObject;
        Rigidbody2D bRb = bullet.GetComponent<Rigidbody2D>();

        float rad = 0;

        // 上に発射
        if (YStickUpDeadZone < inputManager.UpMoveKey) {
            rad = upShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            muzzleVelocity = upShot.MuzzleVelocity; //上へ発射時の初速を代入
            bRb.gravityScale = upShot.GravityScale; //上へ発射時の弾の重量を代入
                                                    // 下に発射
        } else if (inputManager.UpMoveKey < _YStickDownDeadZone) {
            rad = downShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            muzzleVelocity = downShot.MuzzleVelocity; //下へ発射時の初速を代入
            bRb.gravityScale = downShot.GravityScale; //下へ発射時の弾の重量を代入
                                                      // 正面に発射
        } else {
            rad = horizontalShot.FireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            muzzleVelocity = horizontalShot.MuzzleVelocity;  //正面へ発射時の初速を代入
            bRb.gravityScale = horizontalShot.GravityScale;  //正面へ発射時の弾の重量を代入
        }

        //rad(ラジアン角)から発射用ベクトルを作成
        double addforceX = Math.Cos(rad * 1);
        double addforceY = Math.Sin(rad * 1);
        Vector3 shotangle = new Vector3((float)addforceX, (float)addforceY, 0);

        // キャラが反対向いてたら反対に発射させる
        if (!flip) {
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
        _bulletsRemain.text = " ∞ ";
        fireTime += _fireRate;
    }

    // ホールメイカー発射
    private void HoleMakerShot()
    {
        for (int i = 0; i < _hmShotBullets; i++) {
            GameObject bullet = Instantiate(_acidbulletPrefab, mainThrowPoint, Quaternion.identity) as GameObject;
            Rigidbody2D bRb = bullet.GetComponent<Rigidbody2D>();

            float rad = 0;

            // 上に発射
            if (YStickUpDeadZone < inputManager.UpMoveKey) {
                rad = (upShot.FireAngle + hmSpreadAngle[i]) * Mathf.Deg2Rad; //角度をラジアン角に変換
                muzzleVelocity = upShot.MuzzleVelocity; //上へ発射時の初速を代入
                bRb.gravityScale = upShot.GravityScale; //上へ発射時の弾の重量を代入
                                                        // 下に発射
            } else if (inputManager.UpMoveKey < _YStickDownDeadZone) {
                rad = (downShot.FireAngle + hmSpreadAngle[i]) * Mathf.Deg2Rad; //角度をラジアン角に変換
                muzzleVelocity = downShot.MuzzleVelocity; //下へ発射時の初速を代入
                bRb.gravityScale = downShot.GravityScale; //下へ発射時の弾の重量を代入
                                                          // 正面に発射
            } else {
                rad = (horizontalShot.FireAngle + hmSpreadAngle[i]) * Mathf.Deg2Rad; //角度をラジアン角に変換
                muzzleVelocity = horizontalShot.MuzzleVelocity;  //正面へ発射時の初速を代入
                bRb.gravityScale = horizontalShot.GravityScale;  //正面へ発射時の弾の重量を代入
            }

            //rad(ラジアン角)から発射用ベクトルを作成
            double addforceX = Math.Cos(rad * 1);
            double addforceY = Math.Sin(rad * 1);
            Vector3 shotangle = new Vector3((float)addforceX, (float)addforceY, 0);

            // キャラが反対向いてたら反対に発射させる
            if (!flip) {
                shotangle = Vector3.Reflect(shotangle, Vector3.right);
            }

            //　発射角度にオブジェクトを回転させる、進んでいる方向と角度を一致させる
            var diff = shotangle - bullet.transform.position;
            var axis = Vector3.Cross(bullet.transform.right, shotangle);
            var angle = Vector3.Angle(bullet.transform.right, shotangle) * (axis.z < 0 ? -1 : 1);
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //　で、でますよ
            bRb.AddForce(shotangle * muzzleVelocity, ForceMode2D.Force);
        }

        StartCoroutine("HoleMakerSound");
        --hmBullets;
        _bulletsRemain.text = hmBullets + " / " + _hmBulletCapacity;
        fireTime += _hmFireRate;
        if (hmBullets <= 0) {
            _handgunUI.Select();
            equipment = Equipment.Handgun;
            _bulletsRemain.text = " ∞ ";
        }
    }

    IEnumerator HoleMakerSound()
    {
        SoundManagerV2.Instance.PlaySE(13);
        yield return new WaitForSeconds(_hmFireRate - 1.5f);
        SoundManagerV2.Instance.PlaySE(14);
        yield return new WaitForSeconds(0.5f);
        SoundManagerV2.Instance.PlaySE(15);
    }

    private void PreCalcSpreadAngle()
    {
        float range = _hmSpreadRange / _hmShotBullets;
        // 一番上から一番下まで順に計算する。
        // 一番上は拡散範囲÷2
        hmSpreadAngle.Add(_hmSpreadRange / 2);
        int i;

        // 2番目から真ん中まではループで求める
        for(i = 1; i < _hmShotBullets / 2; i++) {
            hmSpreadAngle.Add(hmSpreadAngle[i-1] - range);
        }

        // 下半分の最初は拡散範囲÷同時発射数×2した数にする
        hmSpreadAngle.Add(hmSpreadAngle[i-1] - range * 2);
        i++;

        // 残りの一番下まではループで
        for(; i < _hmShotBullets; i++) {
            hmSpreadAngle.Add(hmSpreadAngle[i-1] - range);
        }

        
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
        if (collision.CompareTag("ItemBullet")) {
            Debug.Log("アイテム取得");
            ItemScript itemScript = collision.gameObject.GetComponent<ItemScript>();
            bullets += itemScript.GetBulletsInItem();

            if (_bulletCapacity < bullets) {
                itemScript.SetBulletsInItem(bullets - _bulletCapacity);
                bullets = _bulletCapacity;
            } else {
                itemScript.SetBulletsInItem(0);
                Destroy(collision.gameObject);              
            }

            _bulletsRemain.text = " ∞ ";
        } else if (collision.CompareTag("PutGun")) {
            _handgunUI.gameObject.SetActive(true);
            _handgunUI.Select();
            _bulletsRemain.enabled = true;
            isGetGun = true;
            Destroy(collision.gameObject);
            SoundManagerV2.Instance.PlaySE(12);
            equipment = Equipment.Handgun;
        } else if (collision.CompareTag("PutHoleMaker") && isGetGun) {
            _hmUI.gameObject.SetActive(true);
            _hmUI.Select();
            isGetHoleMaker = true;
            Destroy(collision.gameObject);
            SoundManagerV2.Instance.PlaySE(12);
            equipment = Equipment.HoleMaker;
            hmBullets += 4;
            _bulletsRemain.text = hmBullets + " / " + _hmBulletCapacity;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "ResidualAcid") {
            if (acidDamageTime <= 0) {
                GameObject acidParentBlock = collision.transform.parent.gameObject;
                var sprite = acidParentBlock.GetComponent<SpriteRenderer>();
                var _sprite = sprite.sprite;
                var _halfX = _sprite.bounds.extents.x;
                var _halfY = _sprite.bounds.extents.y;
                var _vec = new Vector3(-_halfX, 0f, 0f); // これは左上
                var _unvec = new Vector3(_halfX, 0f, 0f); // これは右上
                var _pos = sprite.transform.TransformPoint(_vec);
                var _unpos = sprite.transform.TransformPoint(_unvec);
                if (transform.position.x >= _pos.x && transform.position.x <= _unpos.x) {
                    acidDamageTime += _acidDamageRate;
                    Damage(_acidDamage);
                    SoundManagerV2.Instance.PlaySE(4);
                    Debug.Log("酸に触れて " + _acidDamage + " ダメージを受けた");
                    Debug.Log(transform.position.x);
                    Debug.Log(_pos.x);
                    Debug.Log(_unpos.x);
                }
            }
        } else if(collision.CompareTag("WallReAcid")) {
            if (acidDamageTime <= 0) {
                GameObject acidParentBlock = collision.transform.parent.gameObject;
                var sprite = acidParentBlock.GetComponent<SpriteRenderer>();
                var _sprite = sprite.sprite;
                var _halfX = _sprite.bounds.extents.x;
                var _halfY = _sprite.bounds.extents.y;
                var _vec = new Vector3(0f, -_halfY, 0f); // これは上
                var _unvec = new Vector3(0f, _halfY, 0f); // これは下
                var _pos = sprite.transform.TransformPoint(_vec);
                var _unpos = sprite.transform.TransformPoint(_unvec);
                if (transform.position.y >= _pos.y && transform.position.y <= _unpos.y) {
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

}