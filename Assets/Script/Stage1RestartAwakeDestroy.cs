using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1RestartAwakeDestroy : MonoBehaviour {

    [SerializeField, CustomLabel("番号")] private int _num;

    private void Start()
    {
        if (_num < ScoreManager.Instance.RestartPosNum) {
            Destroy(gameObject);
        }
    }

}
