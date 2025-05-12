using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class InteractionController : MonoBehaviour
{
    [SerializeField] LayerMask interactionLayerMask;
    [FormerlySerializedAs("housingCanvasManager")]
    [Header("UI 연동")]
    [SerializeField] HousingCanvasController housingCanvasController;
    [SerializeField] private InteractionAnimationPanelController interactionAnimationPanelController;
    
    private SuddenEventController _suddenEventController = new SuddenEventController();

    private void Start()
    {
        PlayerStats.Instance.OnWorked += SuddenAfterWorkEventHappen;
    }

    // 상호작용 가능한 사물 범위에 들어올 때
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats.Instance.HideBubble();
        if(other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            housingCanvasController.ShowNpcInteractionButton(() =>
            {
                GameManager.Instance.StartNPCDialogue(GamePhase.Gameplay);
            });
        }
        
        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            ActionType interactionType = other.gameObject.GetComponent<InteractionProp>().RoutineEnter();
            if( interactionType != null )
            {
                PopActionOnScreen(interactionType);
            }
        }
    }
    // 사물에서 벗어날 때 UI 정리
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("NPC")) housingCanvasController.HideInteractionButton();
        
        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            PlayerStats.Instance.ShowBubble();
            housingCanvasController.HideInteractionButton();
            housingCanvasController.interactionTextsController.InitInteractionTexts();
        }
    }
    
    // ActionType 별로 화면에 상호작용 내용 표시, 상호작용 버튼에 이벤트 작성
    private void PopActionOnScreen(ActionType interactionType)
    {
        HousingConstants.interactions.TryGetValue(interactionType, out var interactionTexts);
        
        housingCanvasController.ShowInteractionButton(interactionTexts.ActionText,interactionTexts.DescriptionText,()=>
        {
            if (PlayerStats.Instance.CanPerformByHealth(interactionType))
            {
                if (interactionType == ActionType.Work)
                {
                    if (!PlayerStats.Instance.CanWork()) // 출근 가능한 시간이 아닐 경우
                    {
                        PlayerStats.Instance.ShowAndHideBubble("출근 시간이 아냐");
                        return;
                    }
                }
                
                if (interactionType == ActionType.Dungeon)
                {
                    GameManager.Instance.ChangeToGameScene();
                }
                else
                {
                    GameManager.Instance.PlayInteractionSound(interactionType);
                    interactionAnimationPanelController.ShowAnimationPanel(interactionType,interactionTexts.AnimationText);
                }
                
                PlayerStats.Instance.PerformAction(interactionType);
            }
            else
            {
                PlayerStats.Instance.ShowAndHideBubble("체력이 없어...");
                housingCanvasController.interactionTextsController.ActiveTexts(interactionTexts.LackOfHealth);
            }
        });
    }

    public Action SuddenEventHappen()
    {
        return null;
    }
    
    public void SuddenAfterWorkEventHappen()
    {
        AfterWorkEvent afterWorkEvent = _suddenEventController.SuddenEventCalculator();
        if (afterWorkEvent == AfterWorkEvent.None)
            return;
        switch (afterWorkEvent)
        {
            case AfterWorkEvent.OvertimeWork:
                housingCanvasController.ShowSuddenEventPanel("부장님이 퇴근을 안하셔.. 야근할까?", () =>
                {
                    //Todo: 컷씬과 스테이터스 변경
                    // 체력상 가능한지 확인 이후 행동 수행
                    
                    housingCanvasController.HideSuddenEventPanel();
                });
                break;
            case AfterWorkEvent.TeamGathering:
                housingCanvasController.ShowSuddenEventPanel("갑자기 팀 회식이 잡혔다. 참석 하러 가자", () =>
                {
                    housingCanvasController.HideSuddenEventPanel();
                });
                break;
        }
    }
}
