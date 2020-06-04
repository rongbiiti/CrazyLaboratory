using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeVent : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private float speed = 1f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") ||
           collision.gameObject.CompareTag("BreakBlock")) {
            StartCoroutine("FadeOut");
        }
    }

    private IEnumerator FadeOut()
    {
        while( 0 < spriteRenderer.color.a) {
            spriteRenderer.color -= new Color(0, 0, 0, 1 / speed * Time.deltaTime);
            yield return 0;
        }
        Destroy(gameObject);
    }
}
