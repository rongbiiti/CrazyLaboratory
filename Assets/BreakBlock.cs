using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlock : MonoBehaviour {

    private bool isEnterAcid;
    [SerializeField] private float destroyTime = 4f;
    private Vector2 startScale;

	void Start () {
        startScale = transform.localScale;
	}

    private void FixedUpdate()
    {
        if (isEnterAcid) {
            transform.localScale -= new Vector3(startScale.x / destroyTime * Time.deltaTime, startScale.y / destroyTime * Time.deltaTime);
            if(transform.localScale.x <= 0) {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Acid")) {
            isEnterAcid = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AcidFlask")) {
            isEnterAcid = true;
        }
    }

}
