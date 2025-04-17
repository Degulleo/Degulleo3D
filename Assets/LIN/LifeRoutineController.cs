using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class LifeRoutineController : MonoBehaviour
{
    [SerializeField] LayerMask furnitureLayerMask;
    private void OnTriggerEnter(Collider other)
    {
        if (furnitureLayerMask == (furnitureLayerMask | (1 << other.gameObject.layer)))
        {
            other.GetComponent<IDailyRoutine>().EventEnter();
        }
    }
}
