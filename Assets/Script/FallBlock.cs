using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 落ちてくるガレキのスクリプト
/// 床か敵に当たると壊れてバラバラになる
/// </summary>
public class FallBlock : MonoBehaviour {

    private Explodable explodable;
    private GameObject medKit = null;
    [SerializeField, CustomLabel("エフェクト")] private GameObject _effect;
    private GameObject effect;
    private bool isThreadBreaked;

    public bool IsThreadBreaked
    {
        set { isThreadBreaked = value; }
    }

    void Start () {
        explodable = GetComponent<Explodable>();
        effect = Instantiate(_effect);
        if (transform.childCount == 0) return;
        if (transform.GetChild(0).gameObject.CompareTag("ItemMedkit"))
        {
            medKit = transform.GetChild(0).gameObject;
            medKit.SetActive(false);
            medKit.transform.SetParent(transform.parent);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BreakBlock")) && isThreadBreaked) {
            if (medKit != null)
            {
                medKit.transform.position = transform.position;
                medKit.SetActive(true);
                medKit.transform.GetChild(0).GetComponent<Rigidbody2D>().AddForce(new Vector2(0,0.03f),ForceMode2D.Impulse);
            }

            effect.transform.position = transform.position;
            effect.SetActive(true);
            explodable.explode();
            ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
            ef.doExplosion(transform.position);
            SoundManagerV2.Instance.PlaySE(8);
        }
    }
}
