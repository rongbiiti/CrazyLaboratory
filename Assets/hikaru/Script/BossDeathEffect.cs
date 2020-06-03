using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathEffect : MonoBehaviour {

    private bool Countflg;
    private float AnimeCount;
    [SerializeField] private GameObject Effect;


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            Effect.SetActive(true);
            SoundManagerV2.Instance.PlaySE(45);


        }
        return;
    }
}
