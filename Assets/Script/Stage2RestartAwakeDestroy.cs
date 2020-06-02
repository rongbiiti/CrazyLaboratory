using UnityEngine;

public class Stage2RestartAwakeDestroy : MonoBehaviour {

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (ScoreManager.Instance.IsStage2RestartPointReached) {
            Destroy(gameObject);
        }
    }
}
