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

	void Start () {
        explodable = GetComponent<Explodable>();
        if (transform.childCount == 0) return;
        if (transform.GetChild(0).gameObject.CompareTag("ItemMedkit"))
        {
            medKit = transform.GetChild(0).gameObject;
            medKit.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BreakBlock")) {
            if (medKit != null)
            {
                medKit.SetActive(true);
                medKit.transform.SetParent(null);
                medKit.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,0.03f),ForceMode2D.Impulse);
            }
            explodable.explode();
            ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
            ef.doExplosion(transform.position);
            SoundManagerV2.Instance.PlaySE(8);
        }
    }
}
