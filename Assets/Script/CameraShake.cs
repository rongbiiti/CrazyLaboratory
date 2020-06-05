using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    private Vector3 pos;
    private float elapsed;
    private float _duration;
    private float _magnitude;
    private bool isShaked;

    public void Shake(float duration, float magnitude)
    {
        _duration = duration;
        _magnitude = magnitude;
        isShaked = true;
        //StartCoroutine(DoShake(duration, magnitude));
    }

    private void LateUpdate()
    {
        if (_duration <= 0f) {
            return;
        }

        _duration -= Time.deltaTime;

        if (isShaked) {
            pos = transform.position;
            var x = pos.x + Random.Range(-1f, 1f) * _magnitude;
            var y = pos.y + Random.Range(-1f, 1f) * _magnitude;

            transform.position = new Vector3(x, y, pos.z);
            pos = transform.position;

            isShaked = false;
        } else {
            transform.position = pos;
            isShaked = true;
        }
       
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        pos = transform.position;

        var elapsed = 0f;

        while (elapsed < duration) {
            pos = transform.position;
            var x = pos.x + Random.Range(-1f, 1f) * magnitude;
            var y = pos.y + Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, pos.z);

            pos = transform.position;

            elapsed += Time.deltaTime;

            yield return null;
            transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = pos;
    }
}
