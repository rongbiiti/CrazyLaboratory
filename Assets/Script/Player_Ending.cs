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
