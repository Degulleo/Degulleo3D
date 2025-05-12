using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class SwitchingPanelController : MonoBehaviour
{
    [SerializeField] private GameObject Loading_UI;
    [SerializeField] TMP_Text Loading_text;
    
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }
    
    IEnumerator LoadScene(string sceneName){
        Loading_UI.SetActive(true);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; //퍼센트 딜레이용

        float past_time = 0;
        float percentage = 0;

        while(!(async.isDone)){
            yield return null;

            past_time += Time.deltaTime;

            if(percentage >= 90){
                percentage = Mathf.Lerp(percentage, 100, past_time);

                if(percentage == 100){
                    async.allowSceneActivation = true; //씬 전환 준비 완료
                }
            }
            else{
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if(percentage >= 90) past_time = 0;
            }
            Loading_text.text = percentage.ToString("0") + "%"; //로딩 퍼센트 표기
        }
    }

    public void FadeAndSceneLoad(string sceneName)
    {
        if (canvasGroup == null) return;
        
        canvasGroup.DOFade(1.0f, HousingConstants.SWITCH_PANEL_AINIM_DURATION).OnComplete(() =>
        {
            StartCoroutine(LoadScene(sceneName));
        });
    }
}
