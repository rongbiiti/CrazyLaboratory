using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrologueScript : MonoBehaviour {

    private bool isSkip;

    [SerializeField, CustomLabel("カットシーン")] private GameObject[] _cut;
    [SerializeField, CustomLabel("カットごとの待機秒数")] private float[] _cutDuration;

    [SerializeField, CustomLabel("スキップゲージ")] private Image _skipGuage;
    [SerializeField, CustomLabel("スキップ文字フェードまでの時間")] private float buttonTextFadeDuration = 2f;
    private Text buttonText;
    private Text buttonText2;
    private Color buttonTextStartColor;
    private Color buttonTextStartColor2;
    private float notSkipButtonDownTime;

    private void Awake()
    {
        
    }

    private void Start()
    {
        buttonText = _skipGuage.transform.GetChild(0).GetComponent<Text>();
        buttonText2 = _skipGuage.transform.GetChild(1).GetComponent<Text>();
        buttonTextStartColor = buttonText.color;
        buttonTextStartColor2 = buttonText2.color;

        StartCoroutine("Prologue");
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!isSkip) {
            SkipGuageControl();
        }
       
    }

    private IEnumerator Prologue()
    {
        SoundManagerV2.Instance.PlayBGM(4);
        SoundManagerV2.Instance.PlaySE(48);
        int i = 0;
        foreach(var cut in _cut) {
            cut.SetActive(true);
            yield return new WaitForSeconds(_cutDuration[i++]);
        }
        StartCoroutine("TransitionToStage1");
    }

    private IEnumerator TransitionToStage1()
    {
        yield return null;
        FadeManager.Instance.LoadSceneNormalTrans("Stage1", 1.5f);
        yield return new WaitForSeconds(1.4f);
        SoundManagerV2.Instance.PlayBGM(1);
    }

    private void SkipGuageControl()
    {
        if (Input.GetButton("Cancel")) {
            buttonText.color = buttonTextStartColor;
            buttonText2.color = buttonTextStartColor2;
            notSkipButtonDownTime = 0;
            _skipGuage.fillAmount += Time.deltaTime / 1.5f;
            if(1f <= _skipGuage.fillAmount) {
                StopCoroutine("Prologue");
                SoundManagerV2.Instance.StopBGM();
                SoundManagerV2.Instance.StopSE();
                StartCoroutine("TransitionToStage1");
                isSkip = true;
            }
        } else {

            // ifめっちゃネストしててごめんなさい
            if (0 <= _skipGuage.fillAmount) {

                // スキップボタン押してない間はスキップまでの秒数を戻す
                _skipGuage.fillAmount -= Time.deltaTime * 2;
                if (_skipGuage.fillAmount <= 0) {

                    // 秒数が0秒以下なら、「ボタン押してない秒数」を数え始める
                    notSkipButtonDownTime += Time.deltaTime;
                    if (buttonTextFadeDuration < notSkipButtonDownTime && 0 <= buttonText.color.a) {

                        // 「ボタン押してない秒数」が規定の秒数を超えるとtextがフェードアウトし始める
                        buttonText.color -= new Color(0, 0, 0, Time.deltaTime * 1.5f);
                        buttonText2.color -= new Color(0, 0, 0, Time.deltaTime * 1.5f);
                    }
                }
            }
        }
    }
}
