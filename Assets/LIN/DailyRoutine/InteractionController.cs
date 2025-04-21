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
    
    // 상호작용 가능한 사물 범위에 들어올 때
    private void OnCollisionEnter(Collision other)
    {
        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            ActionType interactionType = other.gameObject.GetComponent<InteractionProp>().RoutineEnter();
            PopActionOnScreen(interactionType);
        }
    }

    // 사물에서 벗어날 때 UI 정리
    private void OnCollisionExit(Collision other)
    {
        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            housingCanvasController.HideInteractionButton();
        }
    }
    
    // ActionType 별로 화면에 상호작용 내용 표시, 상호작용 버튼에 이벤트 작성
    private void PopActionOnScreen(ActionType interactionType)
    {
        switch (interactionType)
        {
            case ActionType.Sleep:
                housingCanvasController.ShowInteractionButton("침대에서 잘까?","숙면으로 시간 당 체력 1을 회복한다.", () =>
                {
                    if (playerStats.CanPerformByHealth(ActionType.Sleep))
                    {
                        playerStats.PerformAction(ActionType.Sleep);
                        housingCanvasController.HideInteractionButton();
                        //TODO: 화면 전환 효과와 UI 업데이트 작업
                    }
                    else
                    {
                        housingCanvasController.SetActionText("지금 체력으로 잘 수 없다..");
                        housingCanvasController.SetDescriptionText();
                    }
                });
                break;
            case ActionType.Housework:
                housingCanvasController.ShowInteractionButton("밀린 집안일을 처리할까?","체력 1을 사용하고 좋은일이 일어날지도 모른다", () =>
                {
                    if (playerStats.CanPerformByHealth(ActionType.Housework))
                    {
                        playerStats.PerformAction(ActionType.Housework);
                        housingCanvasController.HideInteractionButton();
                        //TODO: 집안일 후 랜덤 강화 효과 적용
                    }
                    else
                    {
                        housingCanvasController.SetActionText("힘들어서 못해..");
                        housingCanvasController.SetDescriptionText();
                    }
                });
                break;
            case ActionType.Dungeon:
                housingCanvasController.ShowInteractionButton("던전에 입장할까?","체력 3을 사용하고 3시간이 흐른다.", () =>
                {
                    if (playerStats.CanPerformByHealth(ActionType.Dungeon))
                    {
                        playerStats.PerformAction(ActionType.Dungeon);
                        housingCanvasController.HideInteractionButton();
                    }
                    else
                    {
                        housingCanvasController.SetActionText("던전에 갈 체력이 되지 않아..");
                        housingCanvasController.SetDescriptionText();
                    }
                });
                break;
            case ActionType.Work:
                housingCanvasController.ShowInteractionButton("출근한다.","체력 3을 사용하고 저녁 6시에나 돌아오겠지..", () =>
                {
                    if (playerStats.CanPerformByHealth(ActionType.Work))
                    {
                        playerStats.PerformAction(ActionType.Work);
                        housingCanvasController.HideInteractionButton();
                    }
                    else
                    {
                        housingCanvasController.SetActionText("도저히 출근할 체력이 안되는걸..?");
                        housingCanvasController.SetDescriptionText();
                    }
                });
                break;
        }
    }
}
