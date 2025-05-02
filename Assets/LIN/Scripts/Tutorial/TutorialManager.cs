using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialStep firstStep;   // 인스펙터에서 첫 단계 드래그
    
    [Header("튜토리얼 패널 생성")]
    [SerializeField] private GameObject[] tutorialPanelPrefabs;
    
    private GameObject _tutorialPanelObject;
    private TutorialPanelController _tutorialPanelController;

    private Coroutine _runningCoroutine;
    private CanvasGroup overlay;  // 화면 암전 및 입력 차단
    private RectTransform targetRt;
    private Canvas overlayCanvas;  // RectTransformUtility를 위한 Canvas
    private Action onTutorialComplete;
    
    public void Start( )
    {
            StartTutorial(null);
    }
    public void StartTutorial(Action onTutorialEnd, int panelIndex = 0)
    {        
        var parentObject = FindObjectOfType(typeof(Canvas));
        if (parentObject != null)
        {
            overlayCanvas = parentObject as Canvas;
            
            _tutorialPanelObject = Instantiate(tutorialPanelPrefabs[panelIndex], parentObject.GameObject().transform);
            overlay = _tutorialPanelObject.GetComponent<CanvasGroup>();
            _tutorialPanelController = _tutorialPanelObject.GetComponent<TutorialPanelController>();
        }

        if (_tutorialPanelController != null)
        {
            onTutorialComplete = onTutorialEnd;
        overlay.alpha = 1f;
        overlay.blocksRaycasts = true;
        RunStep(firstStep);
        }
        else Debug.Log("패널 생성 실패, 튜토리얼 진행이 불가능합니다.");
    }

    private void RunStep(TutorialStep step)
    {
        if (_runningCoroutine != null) StopCoroutine(_runningCoroutine);
        _runningCoroutine = StartCoroutine(RunStepCoroutine(step));
    }

    private IEnumerator RunStepCoroutine(TutorialStep step)
    {
        // 단계 시작 이벤트
        step.onStepBegin?.Invoke();
        // 메시지 갱신
        _tutorialPanelController.setTutorialText(step.message);

        float elapsed = 0f;
        bool done = false;        
        
        //터치해야 할 위치가 있는지 체크
        if (step.touchTargetIndex >= 0)
        {
            targetRt = _tutorialPanelController.touchTargets[step.touchTargetIndex].GetComponent<RectTransform>();
            _tutorialPanelController.touchTargets[step.touchTargetIndex].SetActive(true);
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
                        Debug.Log("타겟 터치");
                        targetRt = null;
                        _tutorialPanelController.touchTargets[step.touchTargetIndex].SetActive(false);
                        done = true;
                    }
                    else
                    {
                        Debug.Log("타겟이 아닌 곳 터치");
                    }
                }
            }

            // 타임아웃 체크
            if (step.timeout > 0f && elapsed >= step.timeout)
                done = true;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 단계 완료 이벤트
        step.onStepComplete?.Invoke();
        
        // 다음 단계로
        if (step.nextStep != null)
            RunStep(step.nextStep);
        else
            EndTutorial();
    }

    private void EndTutorial()
    {
        _tutorialPanelController.setTutorialText("");
        overlay.alpha = 0f;
        overlay.blocksRaycasts = false;
        
        if(onTutorialComplete!=null)
        {
            onTutorialComplete?.Invoke();
            onTutorialComplete = null;
        }

        if (_tutorialPanelObject == null)
            return;
        Destroy(_tutorialPanelObject);
        _tutorialPanelController = null;
    }
}