using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialStep firstStep;
    [SerializeField] private CanvasGroup overlay;   // 입력 차단 & 다크닝용
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private Canvas overlayCanvas;  // RectTransformUtility를 위한 Canvas
    // [SerializeField] private GameObject housingUICanvas;   //튜토리얼 동안 입력 제한

    [Header("튜토리얼 터치 타겟들")]
    [SerializeField] private GameObject[] touchTargets;  
    
    [Header("터치 타겟에 맞춰 감춰놓을 게임 오브젝트")]
    [SerializeField] private GameObject[] lockTargets;  
    
    private Coroutine _runningCoroutine;
    private RectTransform targetRt;

    public void Start()
    {
        StartTutorial();
    }

    public void StartTutorial()
    {
        overlay.alpha = 1f;
        overlay.blocksRaycasts = true;
        RunStep(firstStep);
    }

    private void RunStep(TutorialStep step)
    {
        if (_runningCoroutine != null) StopCoroutine(_runningCoroutine);
        _runningCoroutine = StartCoroutine(RunStepCoroutine(step));
    }

    private IEnumerator RunStepCoroutine(TutorialStep step)
    {
        // 단계 시작 이벤트
        step.onBegin?.Invoke();
        // 메시지 갱신
        tutorialText.text = step.message;

        float elapsed = 0f;
        bool done = false;
        
        //터치해야 할 위치가 있는지 체크
        if (step.touchTargetIndex >= 0)
        {
            targetRt = touchTargets[step.touchTargetIndex].GetComponent<RectTransform>();
            lockTargets[step.touchTargetIndex].SetActive(false);
        }

        while (!done)
        {            
            // 1) 영역 터치 체크
            if (targetRt != null)
            { 
                // 클릭 또는 터치 이벤트
                bool pressed = Input.GetMouseButtonDown(0) || Input.touchCount > 0;
                if (pressed)
                {
                    Vector2 screenPos = Input.touchCount > 0
                        ? Input.GetTouch(0).position
                        : (Vector2)Input.mousePosition;

                    // 터치 위치가 지정 RectTransform 안에 있는지 검사
                    if (RectTransformUtility.RectangleContainsScreenPoint(
                            targetRt, screenPos, overlayCanvas.worldCamera))
                    {
                        yield return new WaitForSeconds(1f);
                        // done = true;
                        // Debug.Log("영역 터치");
                    }
                }
            }
            
            // 타임아웃 체크
            if (step.timeout > 0f && elapsed >= step.timeout)
                done = true;

            // 키 입력 체크
            if (step.requiredKey != KeyCode.None && Input.GetKeyDown(step.requiredKey))
                done = true;

            
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 단계 완료 이벤트
        step.onComplete?.Invoke();

        // 다음 단계로
        if (step.nextStep != null)
            RunStep(step.nextStep);
        else
            EndTutorial();
    }

    private void EndTutorial()
    {
        tutorialText.text = "";
        overlay.alpha = 0f;
        overlay.blocksRaycasts = false;
    }
}