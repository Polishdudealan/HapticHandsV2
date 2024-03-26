using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipCollision : MonoBehaviour
{
    [HideInInspector]
    public MyFinger finger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetCollisionState()
    {
        return finger.GetCollisionFlag();
    }

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("Collision");
        finger.SetCollisionFlag(true);
        finger.SetServoPos();
    }

    void OnTriggerExit(Collider collider)
    {
        //finger.SetCollisionFlag(false);
    }
}
