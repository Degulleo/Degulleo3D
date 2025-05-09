using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class InteractionController : MonoBehaviour
{
    [SerializeField] LayerMask interactionLayerMask;

    [FormerlySerializedAs("housingCanvasManager")] [Header("UI 연동")] [SerializeField]
    HousingCanvasController housingCanvasController;

    [SerializeField] private InteractionAnimationPanelController interactionAnimationPanelController;

    private void Start()
    {
        PlayerStats.Instance.OnWorked += SuddenAfterWorkEventHappen;
    }

    // 상호작용 가능한 사물 범위에 들어올 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            housingCanvasController.ShowNpcInteractionButton(() =>
            {
                GameManager.Instance.StartNPCDialogue(GamePhase.Gameplay);
            });
        }

        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            ActionType interactionType = other.gameObject.GetComponent<InteractionProp>().RoutineEnter();
            if (interactionType != null)
            {
                PopActionOnScreen(interactionType);
            }
        }
    }

    // 사물에서 벗어날 때 UI 정리
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC")) housingCanvasController.HideInteractionButton();

        if (interactionLayerMask == (interactionLayerMask | (1 << other.gameObject.layer)))
        {
            housingCanvasController.HideInteractionButton();
            housingCanvasController.interactionTextsController.InitInteractionTexts();
        }
    }

    // ActionType 별로 화면에 상호작용 내용 표시, 상호작용 버튼에 이벤트 작성
    private void PopActionOnScreen(ActionType interactionType)
    {
        HousingConstants.interactions.TryGetValue(interactionType, out var interactionTexts);

        housingCanvasController.ShowInteractionButton(interactionTexts.ActionText, interactionTexts.DescriptionText,
            () =>
            {
                if (PlayerStats.Instance.CanPerformByHealth(interactionType))
                {
                    PlayerStats.Instance.PerformAction(interactionType);

                    if (interactionType == ActionType.Dungeon)
                    {
                        GameManager.Instance.ChangeToGameScene();
                    }
                    else
                    {
                        GameManager.Instance.PlayInteractionSound(interactionType);
                        interactionAnimationPanelController.ShowAnimationPanel(interactionType,
                            interactionTexts.AnimationText);
                    }
                }
                else
                {
                    housingCanvasController.interactionTextsController.ActiveTexts(interactionTexts.LackOfHealth);
                }
            });
    }

    public Action SuddenEventHappen()
    {
        return null;
    }

    #region 퇴근 후 돌발 이벤트
    
    //이벤트 발생 확률 계산기
    private AfterWorkEventType SuddenEventCalculator()
    {
        var index = Random.Range(0, HousingConstants.AFTER_WORK_DENOMINATOR);
        return HousingConstants.AfterWorkEvents.GetValueOrDefault(index, AfterWorkEventType.None);
    }

    // Interaction Controller와 같은 방식으로 작동됩니다.
    private void SuddenAfterWorkEventHappen()
    {
        AfterWorkEventType afterWorkEventType = SuddenEventCalculator();
        if (afterWorkEventType == AfterWorkEventType.None) return;

        HousingConstants.SuddenEventTexts.TryGetValue(afterWorkEventType, out string suddenEventText);
        
        housingCanvasController.ShowSuddenEventPanel(suddenEventText, () =>
        {
            housingCanvasController.ShowSuddenEventImage(afterWorkEventType);
            GameManager.Instance.PlaySuddenEventSound(afterWorkEventType);
        });
    }

    #endregion

}
