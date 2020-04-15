using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocasUnder : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")) {
            GameObject cam = GameObject.Find("Main Camera");
            cam.GetComponent<CameraController>().SetIsFocasUnder(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player")) {
            GameObject cam = GameObject.Find("Main Camera");
            cam.GetComponent<CameraController>().SetIsFocasUnder(false);
        }
    }
}
