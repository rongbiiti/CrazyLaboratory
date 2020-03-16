using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FallBlock : MonoBehaviour {

    private Rigidbody2D rb;
    private Explodable explodable;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        explodable = GetComponent<Explodable>();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy")) {
            explodable.explode();
            ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
            ef.doExplosion(transform.position);
            SoundManagerV2.Instance.PlaySE(8);
        }
    }
}
