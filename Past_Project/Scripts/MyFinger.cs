using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFinger : MonoBehaviour
{
    public int test_val = 0;
    public Transform j1;
    public Transform j2;
    public Transform j3;
    public int fingerId;

    private float zAng = 0;
    [HideInInspector]
    public int curVal = 0;
    private bool collisionFlag = false;
    private int offset = 600;
    private int collisionPos = 0;

    private float collisionCheckOffset = 10;

    private float[] initAngleVal;

    public int start = 0;
    public int end = 0x0FFF;

    public bool debug = false;

    private int prevVal = 0;

    //public Transform handAnchor;


    [HideInInspector]
    public CommManager comm;
    // Start is called before the first frame update
    void Start()
    {
        j3.GetComponent<TipCollision>().finger = this;
        initAngleVal = new float[3];
        initAngleVal[0] = j1.eulerAngles.z;
        initAngleVal[1] = j2.eulerAngles.z;
        initAngleVal[2] = j3.eulerAngles.z;
    }

    private void Update()
    {
        if (debug) {
            SetPos(test_val);
        }
        comm.SetServoPos(fingerId, collisionPos, collisionFlag);
    }

    public void SetPos(int val)
    {
        float diff = 0;
        if (val > end || val < start)
        {
            //diff = 0;
            if (val > end)
            {
                zAng = -90;
                //prevVal = end;
            }
            else
            {
                zAng = 0;
                //prevVal = start;
            }
            //Debug.LogWarning("Finger percentage out of range.");
            //return;
        }
        else if (collisionFlag && val > (curVal - collisionCheckOffset / (end - start)))
        {
            //prevVal = val;
            return;
        }
        else
        {
            collisionFlag = false;
            curVal = val;
            if (end > start)
            {
                zAng = -90 * (val - start) / (end - start);
            }
        }
        Debug.Log(zAng * 1);
        // j1.rotation = Quaternion.Euler(0, 0, 0);
        j1.rotation = Quaternion.Euler(j1.eulerAngles.x, j1.eulerAngles.y, zAng * 1);// + initAngleVal[0] + handAnchor.eulerAngles.z);// + (handAnchor.rotation.eulerAngles.x - initAnchorRotation.eulerAngles.x));
        j2.rotation = Quaternion.Euler(j2.eulerAngles.x, j2.eulerAngles.y, zAng * 2);// + initAngleVal[1] + handAnchor.eulerAngles.z);
        j3.rotation = Quaternion.Euler(j3.eulerAngles.x, j3.eulerAngles.y, zAng * 3);// + initAngleVal[2] + handAnchor.eulerAngles.z);
        //j1.Rotate(0, 0, zAng * 1 + initAngleVal[0] - j1.eulerAngles.z, Space.Self);
    }

    public void SetStart(int val)
    {
        start = val;
        //SetPos(start);
    }

    public void SetEnd(int val)
    {
        end = val;
        //SetPos(end);
    }

    public bool GetCollisionFlag()
    {
        return collisionFlag;
    }

    public void SetCollisionFlag(bool flag)
    {
        if ((curVal - start) < ((collisionCheckOffset + 5) / (end - start))) {
            collisionFlag = false;
        }
        else {
            collisionFlag = flag;
        }
    }
    
    public void SetServoPos()
    {
        collisionPos = curVal - offset;// 0x0FFF * (curVal - start) / end + offset;
        if (collisionPos < 0) {
            collisionPos = 0;
        }
    }

}
