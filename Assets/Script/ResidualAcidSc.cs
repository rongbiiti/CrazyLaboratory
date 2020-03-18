using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 床や壁に残る酸のスクリプト
/// </summary>
public class ResidualAcidSc : MonoBehaviour {

    [SerializeField, Range(0f, 60f), CustomLabel("当たり判定消えるまでの時間")] private float _destroyTime = 5f;
    [SerializeField, Range(0f, 4f), CustomLabel("フェードアウトする時間")] private float _fadeTime = 0.5f;
    private SpriteRenderer sprite;
    private Color color;
    private BoxCollider2D col;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (_destroyTime <= 0) {
            _fadeTime -= Time.deltaTime;
            sprite.color -= new Color(0, 0, 0, sprite.color.a / _fadeTime * Time.deltaTime);
            if (_fadeTime < 0) {
                Destroy(gameObject);
            }
        } else {
            _destroyTime -= Time.deltaTime;
            if (_destroyTime <= 0) {
                col.enabled = false;
            }
        }
    }

    public float GetReAcidEnableTime()
    {
        return _destroyTime + _fadeTime;
    }
}
