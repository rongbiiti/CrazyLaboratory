using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidualAcidSc : MonoBehaviour {

    [SerializeField, Range(0f, 60f)] private float _destroyTime = 5f;
    [SerializeField, Range(0f, 4f)] private float _fadeTime = 0.5f;
    private SpriteRenderer sprite;
    private Color color;
    private BoxCollider2D col;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        var _m = sprite.localToWorldMatrix;
        var _sprite = sprite.sprite;
        var _halfX = _sprite.bounds.extents.x;
        var _halfY = _sprite.bounds.extents.y;
        var _vec = new Vector3(-_halfX, _halfY, 0f);
        var _pos = _m.MultiplyPoint3x4(_vec);
        Debug.Log("1 : " + _pos);
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
