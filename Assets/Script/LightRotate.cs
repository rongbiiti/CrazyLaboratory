using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotate : MonoBehaviour {

    [SerializeField, CustomLabel("回転速度")] private float _speed = 1f;

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, _speed, Space.World);
    }


}
