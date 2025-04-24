using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class InteractionController : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] LayerMask interactionLayerMask;
    [FormerlySerializedAs("housingCanvasManager")]
    [Header("UI 연동")]
    [SerializeField] HousingCanvasController housingCanvasController;
    
    private SuddenEventController _suddenEventController = new SuddenEventController();

    private void Start()
    {
        playerStats.OnWorked += SuddenAfterWorkEventHappen;
    }

    // 상호작용 가능한 사물 범위에 들어올 때
    private void OnTriggerEnter(Collider other)
    {
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
        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            housingCanvasController.HideInteractionButton();
        }
    }
    
    // ActionType 별로 화면에 상호작용 내용 표시, 상호작용 버튼에 이벤트 작성
    private void PopActionOnScreen(ActionType interactionType)
    {
        HousingConstants.interactions.TryGetValue(interactionType, out var interactionTexts);
        
        housingCanvasController.ShowInteractionButton(interactionTexts.ActionText,interactionTexts.DescriptionText,()=>
        {
            if (playerStats.CanPerformByHealth(interactionType))
            {
                playerStats.PerformAction(interactionType);
            }
            else
            {
                housingCanvasController.SetActionText(interactionTexts.LackOfHealth);
                housingCanvasController.SetDescriptionText();
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
