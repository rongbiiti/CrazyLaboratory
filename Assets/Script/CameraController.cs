using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player = null;
    private Camera cam;
    private Vector3 offset = Vector3.zero;
    private bool isFloarChange = false;
    private float YAxisFixTime = 0f;
    private float setYAxisFixTime = 1f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;
        Vector3 viewPos = cam.WorldToViewportPoint(player.transform.position);
        if (viewPos.y > 0.75f) {
            newPosition.y = player.transform.position.y - offset.y;
        } else if (viewPos.y < 0.3f) {
            newPosition.y = player.transform.position.y + offset.y;
        }
        newPosition.x = player.transform.position.x + offset.x;
        newPosition.z = player.transform.position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, 5.0f * Time.deltaTime);
        if (isFloarChange) {
            FloarChange(newPosition, viewPos);
        }
    }

    private void FloarChange(Vector3 newPosition, Vector3 viewPos)
    {
        YAxisFixTime -= Time.deltaTime;
        newPosition.x = player.transform.position.x + offset.x;
        newPosition.y = player.transform.position.y + offset.y;
        newPosition.z = player.transform.position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, 2.5f * Time.deltaTime);
        if(YAxisFixTime <= 0f) {
            isFloarChange = false;
        }
    }

    public void SetIsFloarChange()
    {
        if (!isFloarChange) {
            isFloarChange = true;
            YAxisFixTime = setYAxisFixTime;
        }
    }
}