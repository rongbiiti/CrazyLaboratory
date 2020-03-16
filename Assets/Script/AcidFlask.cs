using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidFlask : MonoBehaviour {

    [SerializeField] private GameObject _residualAcid;
    [SerializeField] private float _destroyTime = 7f;

    private void FixedUpdate()
    {
        _destroyTime -= Time.deltaTime;
        if (_destroyTime <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MoveBlock")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>();
            var _m = sprite.localToWorldMatrix;
            var _sprite = sprite.sprite;
            var _halfX = _sprite.bounds.extents.x;
            var _halfY = _sprite.bounds.extents.y;
            var _vec = new Vector3(0f, _halfY / 2f, 0f);
            var _pos = _m.MultiplyPoint3x4(_vec);
            GameObject residualAcid = Instantiate(_residualAcid, transform.position - _vec, Quaternion.identity) as GameObject;
            residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform);
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(0);

        } else if (collision.gameObject.CompareTag("Wall")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>();
            var _m = sprite.localToWorldMatrix;
            var _sprite = sprite.sprite;
            var _halfX = _sprite.bounds.extents.x;
            var _halfY = _sprite.bounds.extents.y;
            var _vec = new Vector3(_halfX / 3f, 0f, 0f);
            var _pos = _m.MultiplyPoint3x4(_vec);
            var x = transform.position.x;
            GameObject residualAcid;
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x) {
                residualAcid = Instantiate(_residualAcid, transform.position - _vec, Quaternion.Euler(0, 0, 270)) as GameObject;
            } else {
                residualAcid = Instantiate(_residualAcid, transform.position + _vec, Quaternion.Euler(0, 0, 90)) as GameObject;
            }
            residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform);
            residualAcid.tag = "WallReAcid";
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(1);


        } else if (collision.gameObject.CompareTag("Ceil")) {
            var sprite = collision.transform.parent.GetComponent<SpriteRenderer>();
            var _m = sprite.localToWorldMatrix;
            var _sprite = sprite.sprite;
            var _halfX = _sprite.bounds.extents.x;
            var _halfY = _sprite.bounds.extents.y;
            var _vec = new Vector3(0f, _halfY / 2f, 0f);
            var _pos = _m.MultiplyPoint3x4(_vec);
            GameObject residualAcid = Instantiate(_residualAcid, transform.position + _vec, Quaternion.Euler(0, 0, 180)) as GameObject;
            residualAcid.transform.SetParent(collision.gameObject.transform.parent.transform);
            collision.transform.parent.GetComponent<SprMaskCtrl>().EnableSpriteMask(residualAcid.GetComponent<ResidualAcidSc>().GetReAcidEnableTime());
            SoundManagerV2.Instance.PlaySE(0);
        }

        Destroy(gameObject);
    }

}
