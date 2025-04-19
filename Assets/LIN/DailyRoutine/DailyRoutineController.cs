using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class DailyRoutineController : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] LayerMask interactionLayerMask;
    [Header("UI 연동")]
    [SerializeField] HousingCanvasManager housingCanvasManager;
    
    private void OnCollisionEnter(Collision other)
    {
        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            ActionType interactionType = other.gameObject.GetComponent<DailyRoutine>().RoutineEnter();
            PopActionOnScreen(interactionType);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            housingCanvasManager.HideInteractionButton();
        }
    }
    private void PopActionOnScreen(ActionType interactionType)
    {
        switch (interactionType)
        {
            case ActionType.Sleep:
                housingCanvasManager.ShowInteractionButton("침대에서 잘까?","숙면으로 시간 당 체력 1을 회복한다.", () =>
                {
                    if (playerStats.CanPerformByHealth(ActionType.Sleep))
                    {
                        playerStats.PerformAction(ActionType.Sleep);
                    }
                    else
                    {
                        housingCanvasManager.SetActionText("지금 체력으로 잘 수 없다..");
                        housingCanvasManager.SetDescriptionText();
                    }
                });
                break;
            case ActionType.Housework:
                housingCanvasManager.ShowInteractionButton("밀린 집안일을 처리할까?","체력 1을 사용하고 좋은일이 일어날지도 모른다", () =>
                {
                    if (playerStats.CanPerformByHealth(ActionType.Housework))
                    {
                        playerStats.PerformAction(ActionType.Housework);
                        //TODO: 집안일 후 랜덤 강화 효과 적용
                    }
                    else
                    {
                        housingCanvasManager.SetActionText("집안일 할 체력이 남아있지 않다..");
                        housingCanvasManager.SetDescriptionText();
                    }
                });
                break;
            case ActionType.Dungeon:
                housingCanvasManager.ShowInteractionButton("던전에 입장할까?","체력 3을 사용하고 3시간이 흐른다.", () =>
                {
                    if (playerStats.CanPerformByHealth(ActionType.Dungeon))
                    {
                        playerStats.PerformAction(ActionType.Dungeon);
                    }
                    else
                    {
                        housingCanvasManager.SetActionText("던전에 갈 체력이 되지 않아..");
                        housingCanvasManager.SetDescriptionText();
                    }
                });
                break;
            case ActionType.Work:
                housingCanvasManager.ShowInteractionButton("출근한다.","체력 3을 사용하고 저녁 6시에나 돌아오겠지..", () =>
                {
                    if (playerStats.CanPerformByHealth(ActionType.Work))
                    {
                        playerStats.PerformAction(ActionType.Work);
                        Debug.Log("출근");
                    }
                    else
                    {
                        housingCanvasManager.SetActionText("도저히 출근할 체력이 안되는걸..?");
                        housingCanvasManager.SetDescriptionText();
                    }
                });
                break;
        }
    }
}
