﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// シーン遷移時のフェードイン・アウトを制御するためのクラス .
/// </summary>
public class FadeManager : MonoBehaviour
{

    #region Singleton

    private static FadeManager instance;

    public static FadeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (FadeManager)FindObjectOfType(typeof(FadeManager));

                if (instance == null)
                {
                    Debug.LogError(typeof(FadeManager) + "is nothing");
                }
            }

            return instance;
        }
    }

    #endregion Singleton

    /// <summary>
    /// デバッグモード .
    /// </summary>
    public bool DebugMode = false;
    /// <summary>フェード中の透明度</summary>
    private float fadeAlpha = 0;
    /// <summary>フェード中かどうか</summary>
    private bool isFading = false;
    /// <summary>フェード色</summary>
    public Color fadeColor = Color.black;
    /// <summary>ロゴのプレハブ</summary>
    public GameObject _logoPrefab;
    /// <summary>ロゴのオフセット</summary>
    public Vector3 _logoOffSet;
    /// <summary>生成したプレハブ</summary>
    private GameObject logo;
    /// <summary>カメラ</summary>
    private Transform cam;

    Animator animator;

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        logo = Instantiate(_logoPrefab, Vector3.zero, Quaternion.identity);
        logo.transform.parent = transform;
        logo.SetActive(false);
        DontDestroyOnLoad(this.gameObject);
        animator = logo.GetComponent<Animator>();

    }

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    public void OnGUI()
    {

        // Fade .
        if (this.isFading)
        {
            //色と透明度を更新して白テクスチャを描画 .
            this.fadeColor.a = this.fadeAlpha;
            GUI.color = this.fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }

        if (this.DebugMode)
        {
            if (!this.isFading)
            {
                //Scene一覧を作成 .
                //(UnityEditor名前空間を使わないと自動取得できなかったので決めうちで作成) .
                List<string> scenes = new List<string>();
                scenes.Add("SampleScene");
                //scenes.Add ("SomeScene1");
                //scenes.Add ("SomeScene2");


                //Sceneが一つもない .
                if (scenes.Count == 0)
                {
                    GUI.Box(new Rect(10, 10, 200, 50), "Fade Manager(Debug Mode)");
                    GUI.Label(new Rect(20, 35, 180, 20), "Scene not found.");
                    return;
                }


                GUI.Box(new Rect(10, 10, 300, 50 + scenes.Count * 25), "Fade Manager(Debug Mode)");
                GUI.Label(new Rect(20, 30, 280, 20), "Current Scene : " + SceneManager.GetActiveScene().name);

                int i = 0;
                foreach (string sceneName in scenes)
                {
                    if (GUI.Button(new Rect(20, 55 + i * 25, 100, 20), "Load Level"))
                    {
                        LoadScene(sceneName, 1.0f);
                    }
                    GUI.Label(new Rect(125, 55 + i * 25, 1000, 20), sceneName);
                    i++;
                }
            }
        }



    }

    /// <summary>
    /// 画面遷移 .
    /// </summary>
    /// <param name='scene'>シーン名</param>
    /// <param name='interval'>暗転にかかる時間(秒)</param>
    public void LoadScene(string scene, float interval)
    {
        StartCoroutine(TransScene(scene, interval));
    }

    public void LoadSceneNormalTrans(string scene, float interval)
    {
        StartCoroutine(TransSceneNormal(scene, interval));
    }

    public void FadeScreen(float interval)
    {
        StartCoroutine(FadeDisplay(interval));
    }

    public void FadeScreenNormal(float interval)
    {
        StartCoroutine(FadeDisplayNomal(interval));
    }

    /// <summary>
    /// シーン遷移用コルーチン .
    /// </summary>
    /// <param name='scene'>シーン名</param>
    /// <param name='interval'>暗転にかかる時間(秒)</param>
    private IEnumerator TransScene(string scene, float interval)
    {
        yield return 0;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        yield return new WaitForSeconds(1.5f);
        interval = 2.00f;
        logo.transform.position = cam.position - _logoOffSet;
        logo.SetActive(true);
        //だんだん暗く .
        this.isFading = true;
        float time = 0;

        while (time <= interval)
        {
            animator.SetBool("Dark", true); 
            //this.fadeAlpha = Mathf.Lerp (0f, 1f, time / interval);
            time += Time.deltaTime;
            logo.transform.position = cam.position - _logoOffSet;
            yield return 0;
        }

        animator.SetBool("Dark", false);

        //シーン切替 .
        SceneManager.LoadScene(scene);
        
        yield return 0;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        //logo.SetActive(false);

        //だんだん明るく .
        time = 0;
          
        while (time <= interval)
        {
            animator.SetBool("Light", true);  
            //this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            logo.transform.position = cam.position - _logoOffSet;
            yield return 0;
        }

        this.isFading = false;
        animator.SetBool("Light", false);
        logo.SetActive(false);
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private IEnumerator TransSceneNormal(string scene, float interval)
    {
        yield return 0;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        //だんだん暗く .
        this.isFading = true;
        float time = 0;

        while (time <= interval) {
            this.fadeAlpha = Mathf.Lerp (0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        //シーン切替 .
        SceneManager.LoadScene(scene);
        yield return 0;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        //だんだん明るく .
        time = 0;

        while (time <= interval) {
            this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        this.isFading = false;
    }

    private IEnumerator FadeDisplay(float interval)
    {
        yield return 0;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        yield return new WaitForSeconds(1.5f);
        interval = 2.00f;
        logo.transform.position = cam.position - _logoOffSet;
        logo.SetActive(true);
        //だんだん暗く .
        this.isFading = true;
        float time = 0;
        while (time <= interval)
        {
            //animator.SetBool("Dark", true);
            this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }
       
        yield return 0;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        //animator.SetBool("Dark", false);
        //logo.SetActive(false);
        //だんだん明るく .
        time = 0;
        while (time <= interval)
        {
            //animator.SetBool("Light", true);
            this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        //animator.SetBool("Light", false);
        logo.SetActive(false);
        this.isFading = false;
    }

    private IEnumerator FadeDisplayNomal(float interval)
    {
        yield return 0;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        //だんだん暗く .
        this.isFading = true;
        float time = 0;
        while (time <= interval) {
            this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        //だんだん明るく .
        time = 0;
        while (time <= interval) {
            this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        this.isFading = false;
    }
}
