using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
	protected static readonly string[] findTags =
	{
		"ScoreManager",
	};

	private static ScoreManager instance;

	public static ScoreManager Instance
	{
		get
		{
			if (instance == null)
			{

				Type type = typeof(ScoreManager);

				foreach (var tag in findTags)
				{
					GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

					for (int j = 0; j < objs.Length; j++)
					{
						instance = (ScoreManager)objs[j].GetComponent(type);
						if (instance != null)
							return instance;
					}
				}

				Debug.LogWarning(string.Format("{0} is not found", type.Name));
			}

			return instance;
		}
	}

	void Awake()
	{
		CheckInstance();
	}

	private bool CheckInstance()
	{
		if (instance == null)
		{
			instance = (ScoreManager)this;
			DontDestroyOnLoad(gameObject);
			return true;
		}
		else if (Instance == this)
		{
			return true;
		}

		Destroy(this);
		return false;
	}
	
	// ステージごとのプレイ時間
	private float playTime;

	public float PlayTime
	{
		get { return playTime; }
		set { playTime = value; }
	}
	
	// ステージごとの敵撃破数
	private int killCnt;

	public int KillCnt
	{
		get { return killCnt; }
		set { killCnt = value; }
	}
	
	// ステージごとのリトライ数
	private int retryCnt;

	public int RetryCnt
	{
		get { return retryCnt; }
		set { retryCnt = value; }
	}
	
	// ステージごとの敵スタン回数
	private int stunCnt;

	public int StunCnt
	{
		get { return stunCnt; }
		set { stunCnt = value; }
	}
	
	// ステージごとの回復薬取得数
	private int medGetCnt;

	public int MedGetCnt
	{
		get { return medGetCnt; }
		set { medGetCnt = value; }
	}
	
	// クリアまでにかかった時間
	private float gameClearTime;

	public float GameClearTime
	{
		get { return gameClearTime; }
		set { gameClearTime = value; }
	}
	
	// 敵総撃退数
	private int totalKillCnt;

	public int TotalKillCnt
	{
		get { return totalKillCnt; }
		set { totalKillCnt = value; }
	}
	
	// 総リトライ数
	private int totalRetryCnt;

	public int TotalRetryCnt
	{
		get { return totalRetryCnt; }
		set { totalRetryCnt = value; }
	}
	
	// 総スタン回数
	private int totalStunCnt;

	public int TotalStunCnt
	{
		get { return totalStunCnt; }
		set { totalStunCnt = value; }
	}
	
	// 総回復薬取得数
	private int totalMedGetCnt;

	public int TotalMedGetCnt
	{
		get { return totalMedGetCnt; }
		set { totalMedGetCnt = value; }
	}
	
	// 現在のシーン名
	private String sceneName;

	public string SceneName
	{
		get { return sceneName; }
		set { sceneName = value; }
	}

    // ステージ2のリスタートポイントに到達したか
    private bool isStage2RestartPointReached;

    public bool IsStage2RestartPointReached
    {
        get { return isStage2RestartPointReached; }
        set { isStage2RestartPointReached = value; }
    }

    // Stage2のリスタート用の変数：位置
    private float stage2RestartPosition; 

    public float Stage2RestartPosition
    {
        get { return stage2RestartPosition; }
        set { stage2RestartPosition = value; }
    }

    // Stage2のリスタート用の変数：HP
    private float stage2RestartHP;

    public float Stage2RestartHP
    {
        get { return stage2RestartHP; }
        set { stage2RestartHP = value; }
    }

    private void Start () {
		
	}

	private void FixedUpdate()
	{
		
	}

	public void StageScoreReset()
	{
		playTime = 0f;
		killCnt = 0;
		retryCnt = 0;
		stunCnt = 0;
		medGetCnt = 0;
		SceneName = SceneManager.GetActiveScene().name;
	}

	public void Test()
	{
        gameClearTime = 364f;
        totalKillCnt = 5;
        totalRetryCnt = 1;
        totalStunCnt = 6;
        totalMedGetCnt = 12;
		SceneName = SceneManager.GetActiveScene().name;
	}

	public void OverCheck()
	{
		if (359940f < playTime) playTime = 359940f;

		if (999 < killCnt) killCnt = 999;
		if (999 < retryCnt) retryCnt = 999;
		if (999 < stunCnt) stunCnt = 999;
		if (999 < medGetCnt) medGetCnt = 999;
		
		if (359940f < gameClearTime) gameClearTime = 359940f;
		
		if (999 < totalKillCnt) totalKillCnt = 999;
		if (999 < totalRetryCnt) totalRetryCnt = 999;
		if (999 < totalStunCnt) totalStunCnt = 999;
		if (999 < totalMedGetCnt) totalMedGetCnt = 999;
	}

	public void AllReset()
	{
		StageScoreReset();
		gameClearTime = 0;
		totalKillCnt = 0;
		totalRetryCnt = 0;
		totalStunCnt = 0;
		totalMedGetCnt = 0;
	}
	
}
