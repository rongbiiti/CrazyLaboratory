using System;
using UnityEngine;

/// <summary>
/// 床や壁に残る酸のスクリプト
/// </summary>
public class ResidualAcidSc : MonoBehaviour {

    [SerializeField, Range(0f, 60f), CustomLabel("当たり判定消えるまでの時間")] private float _destroyTime = 5f;

    [SerializeField, CustomLabel("大きくなる時間")]
    private float _increaseTime = 0.5f;
        
    [SerializeField, Range(0f, 4f), CustomLabel("フェードアウトする時間")] private float _fadeTime = 0.5f;
    [SerializeField, CustomLabel("煙のエフェクト")] private GameObject _effect;
    private float resetTime;
    private float fadeResetTime;
    private Vector3 startScale;
    private Vector3 initScale;
    private SpriteRenderer sprite;
    private Color color;
    private BoxCollider2D col;

    private void Awake()
    {
        startScale = transform.localScale;
    }

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
            if (transform.localScale.x <= initScale.x)
            {
                transform.localScale += new Vector3(initScale.x / _increaseTime * Time.deltaTime, initScale.y / _increaseTime * Time.deltaTime);
            }
            
            if (resetTime <= 0) {
                col.enabled = false;
            }
        }
    }

    public float GetReAcidEnableTime()
    {
        return _destroyTime + _fadeTime;
    }

    public void Init(Vector3 pos, Vector3 rota)
    {
        transform.position = pos;
        transform.Rotate(rota);
        Vector3 vec = startScale;
        Vector3 loVec = transform.localScale;
        Vector3 paVec = transform.lossyScale;
        vec.x = loVec.x / paVec.x * vec.x;
        vec.y = loVec.y / paVec.y * vec.y;
        vec.z = loVec.z / paVec.z * vec.z;
        initScale = vec;
        transform.localScale = Vector3.zero;
        Instantiate(_effect, transform.position, transform.rotation);
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
        transform.localScale = startScale;
        
    }
}
