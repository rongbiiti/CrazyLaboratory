using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField] private float destroyTime = 4f;
    [SerializeField] private float onFlaskHitDamageTime = 1f;
    private float damageTime;
    private Vector2 startScale;

    void Start () {
        startScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (0 < damageTime) {
            transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            if (transform.localScale.x <= 0) {
                Destroy(gameObject);
            }
            damageTime -= Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Acid")) {
            transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            if (transform.localScale.x <= 0) {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AcidFlask")) {
            damageTime = onFlaskHitDamageTime;
        }
    }
}
