using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixCameraYAxisScript : MonoBehaviour {

    [SerializeField] private GameObject _combiFixTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) {
            GameObject cam = GameObject.Find("Main Camera");
            cam.GetComponent<CameraController>().SetIsFloarChange();
            _combiFixTrigger.SetActive(true);
            gameObject.SetActive(false);
        }
    }

}