using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    SpriteRenderer Renderer;

    public bool flip = true;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask platformLayer;
    private bool isGrounded = true;
    //private GameObject acid;

    [SerializeField] private GameObject _acidFlaskPrefab;
    [SerializeField] private float _thorwPower = 5f;
    private Vector3 throwPoint;

    private bool isJumping = false;
    private bool isJumpingCheck = true;
    private float jumpTimeCounter;
    private float _jumpPower;

    private Vector3 restartPoint;

    InputManager inputManager;
    PlayerManager playerManager;

    void Start()
    {
        playerManager = PlayerManager.Instance;
        inputManager = InputManager.Instance;
        jumpTimeCounter = playerManager.JumpTime;
        Renderer = gameObject.GetComponent<SpriteRenderer>();
        //acid = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        // 地面と当たり判定をしている。
        Vector2 groundedStart = transform.position - transform.up * 2.2f - transform.right * 0.3f;
        Vector2 groundedEnd = transform.position - transform.up * 2.2f + transform.right * 0.3f;

        isGrounded = Physics2D.Linecast(groundedStart, groundedEnd, platformLayer);
        Debug.DrawLine(groundedStart, groundedEnd, Color.red);

        // 子要素にセットしてある酸の攻撃範囲をアクティブにする
        //acid.SetActive(inputManager.ShotKey);

        if (inputManager.MoveKey == 1 && !flip) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = true;
        }
        if (inputManager.MoveKey == -1 && flip) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            flip = false;
        }

        if (inputManager.ShotKey == 1) {
            throwPoint = transform.GetChild(0).transform.position;

            GameObject flask = Instantiate(_acidFlaskPrefab, throwPoint, Quaternion.identity) as GameObject;
            // クリックした座標の取得（スクリーン座標からワールド座標に変換）
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 向きの生成（Z成分の除去と正規化）
            Vector3 shotForward = Vector3.Scale((mouseWorldPos - transform.position), new Vector3(1, 1, 0)).normalized;

            // 弾に速度を与える
            flask.GetComponent<Rigidbody2D>().velocity = shotForward * _thorwPower;
        }
 
    }

    void FixedUpdate()
    {
        // 地面にいるとき
        if (isGrounded) {
            rb.AddForce(new Vector2(playerManager.MoveForceMultiplier * (inputManager.MoveKey * playerManager.MoveSpeed - rb.velocity.x), rb.velocity.y));

            if (isJumpingCheck && inputManager.JumpKey != 0) {
                jumpTimeCounter = playerManager.JumpTime;
                isJumpingCheck = false;
                isJumping = true;
                _jumpPower = playerManager.JumpPower;
            }
        // 空中にいるとき
        } else {
            // ジャンプキーが話されたらジャンプ中でないことにする
            if (inputManager.JumpKey == 0) {
                isJumping = false;
            }
            // ジャンプしてないかつ磁石の側面にくっついていないとき
            if (!isJumping) {
                // 落ちる力が一定を超えるとそれ以上落下速度が上がらないようにする
                if(rb.velocity.y <= -PlayerManager.Instance.MaxFallSpeed) {
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
        
    }

}