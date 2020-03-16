using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour {

    public Text nameText = null;
    public float risePoint = 1.0f;
    Camera cam;

    [SerializeField, Range(0, 999)] private int _bulletsInItem = 1;
    
    public void SetBulletsInItem( int num ) {
        _bulletsInItem = num;
        nameText.text = "" + _bulletsInItem;
    }

    public int GetBulletsInItem() {
        nameText.text = "" + _bulletsInItem;
        return _bulletsInItem;
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    void Start()
    {
        GameObject obj = GameObject.Find("Main Camera");
        cam = obj.GetComponent<Camera>();
        nameText.text = "" + _bulletsInItem;
    }

    private void FixedUpdate()
    {
        nameText.transform.position = cam.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + risePoint, transform.position.z));
    }

}
