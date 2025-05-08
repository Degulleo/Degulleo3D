using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionAnimationPanelController))]
public class PanelTmpController : MonoBehaviour
{
    InteractionAnimationPanelController interactionAnimationPanelController;

    private void Awake()
    {
        interactionAnimationPanelController = GetComponent<InteractionAnimationPanelController>();
        interactionAnimationPanelController.TutorialSleepAnimation();
    }
}
