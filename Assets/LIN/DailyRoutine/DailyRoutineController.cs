using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class DailyRoutineController : MonoBehaviour
{
    [SerializeField] LayerMask furnitureLayerMask;
    private void OnCollisionEnter(Collision other)
    {
        if (furnitureLayerMask == (furnitureLayerMask | (1 << other.gameObject.layer)))
        {
            other.gameObject.GetComponent<DailyRoutine>().RoutineEnter();
        }
    }
}
