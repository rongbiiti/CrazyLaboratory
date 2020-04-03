using UnityEngine;

public class AcidFlask : MonoBehaviour {

    [SerializeField, CustomLabel("床に残る酸のプレハブ")] private GameObject _residualAcid;
    [SerializeField, CustomLabel("発射されてから消えるまでの時間")] private float _destroyTime = 7f;
    private float resetTime;
    private bool isConflictDestroy = true;
    private Rigidbody2D rb;
    public bool SetConlictDestroyFalse
    {
        set{ isConflictDestroy = false; }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        resetTime = _destroyTime;
    }

    private void FixedUpdate()
    {
        resetTime -= Time.deltaTime;
        if (resetTime <= 0) {
            ResetPosition();
        }
    }

    public void Init(Vector3 pos)
    {
        transform.position = pos;
        transform.rotation = Quaternion.identity;
    }

    private void ResetPosition()
    {
        gameObject.SetActive(false);
        resetTime = _destroyTime;
        rb.velocity = Vector2.zero;
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
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
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MoveBlock")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>();
            var _m = sprite.localToWorldMatrix;
            var _sprite = sprite.sprite;
            var _halfY = _sprite.bounds.extents.y;
            var _vec = new Vector3(0f, _halfY / 2f, 0f);
            GameObject residualAcid = RsdAcdPool.Instance.GetObject();
            if (residualAcid != null) {
                residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform, false);
                residualAcid.GetComponent<ResidualAcidSc>().Init(transform.position - _vec, Vector3.zero);
            }
            
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(0);

        } else if (collision.gameObject.CompareTag("Wall")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>();
            var _m = sprite.localToWorldMatrix;
            var _sprite = sprite.sprite;
            var _halfX = _sprite.bounds.extents.x;
            var _vec = new Vector3(_halfX / 3f, 0f, 0f);
            GameObject residualAcid = RsdAcdPool.Instance.GetObject();
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x) {
                if (residualAcid != null) {
                    residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform, false);
                    residualAcid.GetComponent<ResidualAcidSc>().Init(transform.position - _vec, new Vector3(0, 0, 270));
                }
            } else {
                if (residualAcid != null) {
                    residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform, false);
                    residualAcid.GetComponent<ResidualAcidSc>().Init(transform.position + _vec, new Vector3(0, 0, 90));
                }
            }
            
            residualAcid.tag = "WallReAcid";
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(1);


        } else if (collision.gameObject.CompareTag("Ceil")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>();
            var _m = sprite.localToWorldMatrix;
            var _sprite = sprite.sprite;
            var _halfY = _sprite.bounds.extents.y;
            var _vec = new Vector3(0f, _halfY / 2f, 0f);
            GameObject residualAcid = RsdAcdPool.Instance.GetObject();
            
            if (residualAcid != null) {
                residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform ,false);
                residualAcid.GetComponent<ResidualAcidSc>().Init(transform.position + _vec, new Vector3(0,0,180));
            }
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(0);
        }

        if(isConflictDestroy) {
            ResetPosition();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyHitBox")) {
            if (isConflictDestroy) {
                ResetPosition();
            }
        }
    }

}
