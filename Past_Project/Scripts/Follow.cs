using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Transform targetToFollow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetToFollow.transform.position;
        transform.rotation = Quaternion.Euler(0f, targetToFollow.transform.eulerAngles.y - 90, 0f);
    }
}
