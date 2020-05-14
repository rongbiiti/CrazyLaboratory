using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class AcidFlask : MonoBehaviour {

    [SerializeField, CustomLabel("床に残る酸のプレハブ")] private GameObject _residualAcid;
    [SerializeField, CustomLabel("酸の飛沫")] private GameObject _acidEffect;
    [SerializeField, CustomLabel("敵にヒット時エフェクト")] private GameObject _enemyHitEffect;
    [SerializeField, CustomLabel("発射されてから消えるまでの時間")] private float _destroyTime = 7f;
    [SerializeField, CustomLabel("発射後透明解除する時間")]
    private float _fireAfterTransparentCancelTime = 0.1f;

    private float transparentCancelTime;
    private float resetTime;
    private bool isConflictDestroy = true;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer sr;
    private Color startColor;
    private TrailRenderer tr;
    public bool SetConflictDestroyFalse
    {
        set{ isConflictDestroy = false; }
    }
    
    private GameObject acidEffect;
    private ParticleSystem acidParticleSystem;
    private GameObject enemyHitEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<TrailRenderer>();
        resetTime = _destroyTime;
        acidEffect = Instantiate(_acidEffect);
        acidParticleSystem = acidEffect.GetComponent<ParticleSystem>();
        enemyHitEffect = Instantiate(_enemyHitEffect);
        startColor = sr.color;
        sr.color = Color.clear;
    }

    private void FixedUpdate()
    {
        resetTime -= Time.deltaTime;
        if (resetTime <= 0) {
            ResetPosition();
        }

        transparentCancelTime -= Time.deltaTime;
        if (transparentCancelTime <= 0) {
            sr.color = startColor;
            tr.enabled = true;
        }
        
    }

    public void Init(Vector3 pos, bool isTransparentStart)
    {
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        if(isTransparentStart) transparentCancelTime = _fireAfterTransparentCancelTime;
    }

    private IEnumerator ResetPosition()
    {
        col.enabled = false;
        sr.enabled = false;
        rb.velocity = Vector2.zero;
        resetTime = _destroyTime;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
        col.enabled = true;
        sr.enabled = true;
        sr.color = Color.clear;
        tr.enabled = false;
    }

    /// <summary>
    /// 当たった床や壁のスプライトの幅や高さを取得し、端にぴったりつくように
    /// 位置を調整している。
    /// さらにその床や壁のスプライトマスクをONにし、スプライトマスクが酸が消えるまでの秒数だけ起きておくための
    /// タイマーをセットしている。
    /// </summary>
    /// <param name="collision">当たった床や壁など</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        acidParticleSystem.Simulate(0.0f, true, true);
        acidEffect.transform.position = transform.position;
        acidEffect.SetActive(true);
        acidParticleSystem.Play();
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MoveBlock")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>().sprite;
            var halfY = sprite.bounds.extents.y;
            var vec = new Vector3(0f, halfY / 2f, 0f);
            var residualAcid = RsdAcdPool.Instance.GetObject();
            if (residualAcid != null) {
                residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform, false);
                residualAcid.GetComponent<ResidualAcidSc>().Init(transform.position - vec, Vector3.zero);
            }
            
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(0);

        } else if (collision.gameObject.CompareTag("Wall")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>().sprite;
            var halfX = sprite.bounds.extents.x;
            var vec = new Vector3(halfX / 3f, 0f, 0f);
            var residualAcid = RsdAcdPool.Instance.GetObject();
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x) {
                if (residualAcid != null) {
                    residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform, false);
                    residualAcid.GetComponent<ResidualAcidSc>().Init(transform.position - vec, new Vector3(0, 0, 270));
                }
            } else {
                if (residualAcid != null) {
                    residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform, false);
                    residualAcid.GetComponent<ResidualAcidSc>().Init(transform.position + vec, new Vector3(0, 0, 90));
                }
            }
            
            residualAcid.tag = "WallReAcid";
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(1);


        } else if (collision.gameObject.CompareTag("Ceil")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>().sprite;
            var halfY = sprite.bounds.extents.y;
            var vec = new Vector3(0f, halfY / 2f, 0f);
            var residualAcid = RsdAcdPool.Instance.GetObject();
            
            if (residualAcid != null) {
                residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform ,false);
                residualAcid.GetComponent<ResidualAcidSc>().Init(transform.position + vec, new Vector3(0,0,180));
            }
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(0);
        } else if (collision.gameObject.CompareTag("Beaker")) {
            Debug.Log("側面に当たった");
        }

        if(isConflictDestroy)
        {
            StartCoroutine("ResetPosition");
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyHitBox")) {
            if (!isConflictDestroy) return;
            acidParticleSystem.Simulate(0.0f, true, true);
            acidEffect.transform.position = transform.position;
            acidEffect.SetActive(true);
            acidParticleSystem.Play();
            enemyHitEffect.transform.position = transform.position;
            enemyHitEffect.SetActive(true);
            if(isConflictDestroy)
            {
                StartCoroutine("ResetPosition");
            }
        } else if (collision.CompareTag("Beaker")) {
            Debug.Log("中に入った");
            if (isConflictDestroy) {
                StartCoroutine("ResetPosition");
            }
        }
    }

}
