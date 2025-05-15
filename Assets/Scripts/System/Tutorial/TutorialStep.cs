// TutorialStep.cs
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Tutorial Step")]
public class TutorialStep : ScriptableObject
{
    [TextArea(2, 5)]
    public string message;               // 플레이어에게 보여줄 텍스트
    public float timeout = 2f;           // 0 이상이면 이 시간 경과 후 자동 완료
    
    [Tooltip("단계별로 시작, 완료시 추가로 실행할 이벤트")]
    public UnityEvent onStepBegin;           // 단계 시작 시 추가로 실행할 이벤트
    public UnityEvent onStepComplete;        // 단계 완료 시 실행할 이벤트
    
    [Tooltip("터치해야 할 위치가 있다면, 튜토리얼 매니저에게 인덱스 전달")]
    public int touchTargetIndex=-1;
    
    [Tooltip("보여줄 이미지가 있다면, 튜토리얼 매니저에게 인덱스 전달")]
    public int imageIndex=-1;

    [Tooltip("다음 단계로 넘어갈 TutorialStep, null이면 튜토리얼 종료")]
    public TutorialStep nextStep;
}