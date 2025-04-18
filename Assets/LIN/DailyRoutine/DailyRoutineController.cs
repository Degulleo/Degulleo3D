using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public enum InteractionType {Bed,Sink,Fridge,Work}

[RequireComponent(typeof(Rigidbody))]
public class DailyRoutineController : MonoBehaviour
{
    [SerializeField] HousingCanvasManager housingCanvasManager;
    [SerializeField] LayerMask furnitureLayerMask;
    
    private Canvas _canvas;
    
    private void OnCollisionEnter(Collision other)
    {
        if (furnitureLayerMask == (furnitureLayerMask | (1 << other.gameObject.layer)))
        {
            InteractionType interactionType = other.gameObject.GetComponent<DailyRoutine>().RoutineEnter();

            switch (interactionType)
            {
                case InteractionType.Bed:
                    housingCanvasManager.ShowInteractionButton("침대에서 잘까?","숙면으로 시간 당 체력 1을 회복한다.", () =>
                    {
                        //TODO: 숙면 할 때 할 일 작성
                        Debug.Log("숙면 행동을 시작합니다");
                    });
                    break;
                case InteractionType.Sink:
                    housingCanvasManager.ShowInteractionButton("밀린 집안일을 처리할까?","체력 1을 사용 좋은일이 일어날지도 모른다", () =>
                    {
                        //TODO: 집안일수행, 랜덤 강화 작성
                        Debug.Log("집안일을 시작합니다");
                    });
                    break;
                case InteractionType.Fridge:
                    housingCanvasManager.ShowInteractionButton("던전에 입장할까?","체력 3을 사용하고 3시간이 흐른다.", () =>
                    {
                        //TODO: 던전 입장
                        Debug.Log("던전으로 이동 합니다.. 씬전환");
                    });
                    break;
                case InteractionType.Work:
                    housingCanvasManager.ShowInteractionButton("출근한다.","체력 3을 소모하고 저녁 6시에나 돌아오겠지..", () =>
                    {
                        //TODO: 던전 입장
                        Debug.Log("출근 후 컷씬 연출");
                    });
                    break;
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (furnitureLayerMask == (furnitureLayerMask | (1 << other.gameObject.layer)))
        {
            housingCanvasManager.HideInteractionButton();
        }
    }

    void Awake()
    {
        SetCanvas();
    }
    
    
    void SetCanvas()
    {
        if (_canvas == null)
        {
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }
    }
}
