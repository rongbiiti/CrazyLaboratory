using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Rendering;
using UnityEngine.SceneManagement;

public class Player_Ending : MonoBehaviour {

    [SerializeField, CustomLabel("移動速度")] private float _speed;

    //アニメーション関連
    Animator animator;
    private State state;
    private int Shot0Layer;
    private int Shot1Layer;
    private int Shot2Layer;
    private int Shot3Layer;
    private int Shot4Layer;
    private float weight0;
    private float weight1;
    private float weight2;
    private float weight3;
    private float weight4;
    private bool smoothFlag;
    private CubismModel Model;
    private float anicount;
    enum State
    {
        None,
        Shot0,
        Shot1,
        Shot2,
        Shot3,
        Shot4
    }

    private void Awake()
    {
        //アニメーション関連
        Model = this.FindCubismModel();
        animator = GetComponent<Animator>();
        Shot0Layer = animator.GetLayerIndex("Shot0 Layer");
        Shot1Layer = animator.GetLayerIndex("Shot1 Layer");
        Shot2Layer = animator.GetLayerIndex("Shot2 Layer");
        Shot3Layer = animator.GetLayerIndex("Shot3 Layer");
        Shot4Layer = animator.GetLayerIndex("Shot4 Layer");
        weight0 = 0f;
        weight1 = 0f;
        weight2 = 0f;
        weight3 = 0f;
        weight4 = 0f;
        SetState(State.None, first: true);

        animator.SetBool("Run", true);
        animator.SetBool("Stand", false);
        animator.SetBool("JumpStart", false);
        animator.SetBool("JumpUp", false);
        animator.SetBool("JumpStart-run", false);
        animator.SetBool("JumpUp-run", false);
        animator.SetBool("JumpDown", false);
        animator.SetBool("JumpEnd", false);
        animator.SetBool("Wait", false);
        animator.SetBool("Death1", false);
        animator.SetBool("Death2", false);

        Model.Parts[6].Opacity = 0;
        Model.Parts[7].Opacity = 1;
        Model.Parts[11].Opacity = 1;
        Model.Parts[12].Opacity = 0;
    }

    private void Start()
    {
        StartCoroutine("Ending");
        SoundManagerV2.Instance.PlayBGM(5);
    }

    // 以下アニメーションに関する
    void SetState(State state, bool first = false)
    {  //アニメーションレイヤーのステート
        this.state = state;
        //　最初の設定でなければスムーズフラグをオンにする
        if (!first) {
            smoothFlag = true;
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(new Vector3(_speed, 0, 0));
    }

    private IEnumerator Ending()
    {
        yield return new WaitForSeconds(7f);

        FadeManager.Instance.LoadSceneNormalTrans("TotalResultScene", 2f);
    }
}
