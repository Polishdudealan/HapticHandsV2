using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipGrabbable : MonoBehaviour
{

    int grabbing_item_count = 0;
    List<GameObject> objectsInTouch;
    public GameObject targetToFollow;

    private int array_count = 0;

    // Start is called before the first frame update
    void Start()
    {
        objectsInTouch = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(objectsInTouch.Count);
        List<int> remove_list = new List<int>();
        for (int i = 0; i < objectsInTouch.Count; i++)
        {
            GameObject obj = objectsInTouch[i];
            if (!obj.GetComponent<TipCollision>().GetCollisionState())
            {
                remove_list.Add(i);
            }
        }
        for (int i = remove_list.Count - 1; i >= 0; i --)
        {
            objectsInTouch.RemoveAt(remove_list[i]);
        }
        if (objectsInTouch.Count >= 2)
        {
            //Debug.Log("Grabbed.");
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = targetToFollow.transform;//objectsInTouch[0].transform;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
            transform.parent = null;
        }
        array_count = objectsInTouch.Count;
    }

    bool objInObjectsInTouch(GameObject obj) {
        for (int i = 0; i < objectsInTouch.Count; i ++) {
            if (objectsInTouch[i] == obj) {
                return true;
            }
        }
        return false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Tips" && !objInObjectsInTouch(collider.gameObject))
        {
            objectsInTouch.Add(collider.gameObject);
        }
        //Debug.Log("Collision");
    }

    void OnTriggerExit(Collider collider)
    {
        //if (collider.tag == "tips")
        //{
        //    grabbing_item_count--;
        //}
    }
}
