using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeakerBlinkEffect : MonoBehaviour {

    [SerializeField, CustomLabel("スピード")] private float _speed = 0.5f;
    [SerializeField, CustomLabel("移動範囲")] private float _range = 3.87f;

    Vector3 newPosition;

    private void FixedUpdate()
    {
        newPosition.y = Mathf.Sin(2 * Mathf.PI * _speed * Time.time) * _range;
        transform.localPosition = newPosition;
    }
}
