using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidFlask : MonoBehaviour {

    private float destroyTime = 7f;

    private void FixedUpdate()
    {
        //if (!GetComponent<Renderer>().isVisible) {
        //    Destroy(gameObject);
        //}
        destroyTime -= Time.deltaTime;
        if(destroyTime <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
