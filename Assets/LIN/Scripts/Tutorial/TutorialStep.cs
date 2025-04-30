using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Tutorial Step")]
public class TutorialStep : ScriptableObject
{
    [TextArea(2, 5)]
    public string message;               // 플레이어에게 보여줄 텍스트

    public UnityEvent onBegin;           // 단계 시작 시 추가로 실행할 이벤트
    public UnityEvent onComplete;        // 단계 완료 시 실행할 이벤트

    public float timeout = 0f;           // 0 이상이면 이 시간 경과 후 자동 완료
    public KeyCode requiredKey = KeyCode.None;  // 지정 Key 입력 시 완료
    
    [Tooltip("터치해야 할 위치가 있다면, 게임매니저에게 인덱스 전달")]
    public int touchTargetIndex=-1;

    [Tooltip("다음 단계로 넘어갈 TutorialStep, null이면 튜토리얼 종료")]
    public TutorialStep nextStep;
}