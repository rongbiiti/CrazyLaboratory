using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    SpriteRenderer Renderer;
    GameObject cam;
    [SerializeField] private Sprite _upShotImage;
    [SerializeField] private Sprite _downShotImage;
    [SerializeField] private Slider _HPbar;
    [SerializeField] private Text _bulletsRemain;
    private Sprite forwardShotImage;

    private bool flip = true;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private ContactFilter2D filter2d;
    Vector2 groundedStart;
    Vector2 groundedEnd;
    private bool isGrounded = true;
    //private GameObject acid;

    [SerializeField] private GameObject _acidbulletPrefab;
    [SerializeField, Range(0.001f, 9999f)] private float _maxHP;
    [SerializeField, Range(0f, 9999f)] private float _acidDamage;
    [SerializeField, Range(0.0167f, 10f)] private float _acidDamageRate;
    [SerializeField, Range(0, 999)] private int _bulletCapacity = 10;
    [SerializeField, Range(0f, 5f)] private float _fireRate = 0f;
    [SerializeField] private float _thorwPower = 5f;
    [SerializeField, Range(10f, 80f), Tooltip("真正面が0、真上が90、真下が180です")] private float _upFireAngle = 45f;
    [SerializeField, Range(-10f, -80f), Tooltip("真正面が0、真上が90、真下が180です")] private float _downFireAngle = -45f;
    [SerializeField, Range(0.001f, 1f), Tooltip("スティック上向きの閾値")] private float _YStickUpThreshold = 0.4f;
    [SerializeField, Range(-0.001f, -1f), Tooltip("スティック下向きの閾値")] private float _YStickDownThreshold = -0.4f;
    private bool isGetGun = false;
    private Vector3 mainThrowPoint;
    private float HP;
    private int bullets;
    public bool IsBulletsFull()
    {
        if(bullets >= _bulletCapacity) {
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
        Renderer = gameObject.GetComponent<SpriteRenderer>();
        forwardShotImage = Renderer.sprite;
        rb = GetComponent<Rigidbody2D>();
        startMoveSpeed = playerManager.MoveSpeed;
        HP = _maxHP;
        _HPbar.maxValue = _maxHP;
        _HPbar.value = HP;
        _bulletsRemain.text = " ∞ ";
        _bulletsRemain.enabled = false;
        cam = GameObject.Find("Main Camera");
        SoundManagerV2.Instance.PlayBGM(0);
    }

    void Update()
    {

        // 上に発射
        if (_YStickUpThreshold < inputManager.UpMoveKey && isGetGun) {
            Renderer.sprite = _upShotImage;
            mainThrowPoint = transform.GetChild(0).transform.position;
            
            // 下に発射
        } else if (inputManager.UpMoveKey < _YStickDownThreshold && isGetGun) {
            Renderer.sprite = _downShotImage;
            mainThrowPoint = transform.GetChild(2).transform.position;
            // 正面に発射
        } else {
            Renderer.sprite = forwardShotImage;
            mainThrowPoint = transform.GetChild(1).transform.position;
        }

        if (inputManager.MoveStopKey != 0) {
            playerManager.MoveSpeed = 0;
        } else {
            playerManager.MoveSpeed = startMoveSpeed;
        }

        //groundedStart = transform.position - transform.up * 2.2f - transform.right * -0.65f;
        //groundedEnd = transform.position - transform.up * 2.2f - transform.right * 0.65f;
        //isGrounded = Physics2D.Linecast(groundedStart, groundedEnd, platformLayer);
        //Debug.DrawLine(groundedStart, groundedEnd, Color.red);

        // 地面と当たり判定をしている。
        isGrounded = rb.IsTouching(filter2d);

        if (isJumpingCheck && inputManager.JumpKey == 1 && isGrounded) {
            jumpTimeCounter = playerManager.JumpTime;
            isJumpingCheck = false;
            isJumping = true;
            _jumpPower = playerManager.JumpPower;
            SoundManagerV2.Instance.PlaySE(9);
        }

        // 子要素にセットしてある酸の攻撃範囲をアクティブにする
        //acid.SetActive(inputManager.ShotKey);

        if (inputManager.MoveKey >= 0.3 && !flip) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = true;
        }
        if (inputManager.MoveKey <= -0.3 && flip) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = false;
        }
        
        if (inputManager.ShotKey == 1 && fireTime <= 0 && 0 == bullets && isGetGun) {

            GameObject bullet = Instantiate(_acidbulletPrefab, mainThrowPoint, Quaternion.identity) as GameObject;

            float rad = 0;

            // 上に発射
            if(_YStickUpThreshold < inputManager.UpMoveKey) {
                rad = _upFireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            // 下に発射
            } else if (inputManager.UpMoveKey < _YStickDownThreshold) {
                rad = _downFireAngle * Mathf.Deg2Rad; //角度をラジアン角に変換
            // 正面に発射
            } else {
                rad = 0 * Mathf.Deg2Rad;
            }
            
            //rad(ラジアン角)から発射用ベクトルを作成
            double addforceX = Math.Cos(rad * 1);
            double addforceY = Math.Sin(rad * 1);
            Vector3 shotangle = new Vector3((float)addforceX, (float)addforceY , 0);

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
            bullet.GetComponent<Rigidbody2D>().AddForce(shotangle * _thorwPower, ForceMode2D.Force);
            SoundManagerV2.Instance.PlaySE(6);
            //--bullets;
            _bulletsRemain.text = " ∞ ";
            fireTime += _fireRate;
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
            _bulletsRemain.enabled = true;
            isGetGun = true;
            Destroy(collision.gameObject);
            SoundManagerV2.Instance.PlaySE(12);
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