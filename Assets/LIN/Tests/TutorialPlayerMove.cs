using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialPlayerMove : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(ForceMove());
    }

    IEnumerator ForceMove()
    {
        while (transform.position.z < -3.1f)
        {
            transform.position += new Vector3(0, 0, 0.02f);
            yield return new WaitForFixedUpdate();
        }
    }

    // private void Update()
    // {
    //     if (transform.position.z < -3.0f)
    //     {
    //         this.GameObject().transform.Translate(new Vector3(0,0,1) * (1f * Time.deltaTime));
    //         // this.GameObject().transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z +(1f * Time.deltaTime));
    //     }
    // }
}
