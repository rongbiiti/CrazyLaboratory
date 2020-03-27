using UnityEngine;

/// <summary>
/// 床や壁に残る酸のスクリプト
/// </summary>
public class ResidualAcidSc : MonoBehaviour {

    [SerializeField, Range(0f, 60f), CustomLabel("当たり判定消えるまでの時間")] private float _destroyTime = 5f;
    [SerializeField, Range(0f, 4f), CustomLabel("フェードアウトする時間")] private float _fadeTime = 0.5f;
    private float resetTime;
    private float fadeResetTime;
    private SpriteRenderer sprite;
    private Color color;
    private BoxCollider2D col;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        resetTime = _destroyTime;
        fadeResetTime = _fadeTime;
    }

    private void FixedUpdate()
    {
        if (resetTime <= 0) {
            fadeResetTime -= Time.deltaTime;
            sprite.color -= new Color(0, 0, 0, sprite.color.a / fadeResetTime * Time.deltaTime);
            if (fadeResetTime < 0) {
                ResetPosition();
            }
        } else {
            resetTime -= Time.deltaTime;
            if (resetTime <= 0) {
                col.enabled = false;
            }
        }
    }

    public float GetReAcidEnableTime()
    {
        return _destroyTime + _fadeTime;
    }

    public void Init(Vector3 pos, Quaternion rota)
    {
        transform.position = pos;
        transform.rotation = rota;
    }

    private void ResetPosition()
    {
        gameObject.SetActive(false);
        resetTime = _destroyTime;
        fadeResetTime = _fadeTime;
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.SetParent(null);
        sprite.color += new Color(0, 0, 0, 1);
        col.enabled = true;
        tag = "ResidualAcid";
    }
}
