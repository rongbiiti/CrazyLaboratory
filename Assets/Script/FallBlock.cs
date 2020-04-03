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

	void Start () {
        explodable = GetComponent<Explodable>();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BreakBlock")) {
            explodable.explode();
            ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
            ef.doExplosion(transform.position);
            SoundManagerV2.Instance.PlaySE(8);
        }
    }
}
