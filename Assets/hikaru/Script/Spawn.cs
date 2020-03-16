using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    [SerializeField] private GameObject[] EnemySpawn;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < EnemySpawn.Length; i++)
        {
            EnemySpawn[i].SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < EnemySpawn.Length; i++)
            {
                EnemySpawn[i].SetActive(true);
            }
        }
    }
}
