using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hand : MonoBehaviour
{

    public MyFinger[] fingers;
    public CommManager comm;

    public String test_command = "";
    public bool test_enter = false;
    public bool test_check_val = false;
    public int[] finger_angle = { 0, 0, 0, 0, 0, 0, 0};

    // Start is called before the first frame update
    void Start()
    {
        foreach (MyFinger finger in fingers)
        {
            finger.comm = comm;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (test_enter)
        {
            UpdateHand(test_command);
            test_enter = false;
        }
        if (test_check_val)
        {
            for (int i = 0; i < 5; i ++)
            {
                finger_angle[i] = fingers[i].curVal;
            }
            finger_angle[5] = fingers[1].start;
            finger_angle[6] = fingers[1].end;
        }
    }

    public void UpdateHand(string command)
    {
        int[] fingerPos = DecodeCommand(command);
        if (fingerPos.Length != 5)
        {
            return;
        }
        for (int i = 0; i < 5; i ++)
        {
            //Debug.Log(fingerPos[i]);
            fingers[i].SetPos(fingerPos[i]);
        }
    }

    public void SoftCalibrateStart()
    {
        for (int i = 0; i < 5; i++)
        {
            //Debug.Log(finger_angle[i]);
            fingers[i].SetStart(finger_angle[i]);
        }
    }

    public void SoftCalibrateEnd()
    {
        for (int i = 0; i < 5; i++)
        {
            //Debug.Log(finger_angle[i]);
            fingers[i].SetEnd(finger_angle[i]);
        }
    }

    public void CalibrateStart(string command)
    {
        int[] fingerPos = DecodeCommand(command);
        if (fingerPos.Length != 5)
        {
            return;
        }
        Debug.Log("Start: ");
        //Debug.Log(fingerPos[0]);
        for (int i = 0; i < 5; i++)
        {
            Debug.Log(fingerPos[i]);
            fingers[i].SetStart(fingerPos[i]);
        }
    }

    public void CalibrateEnd(string command)
    {
        int[] fingerPos = DecodeCommand(command);
        if (fingerPos.Length != 5)
        {
            return;
        }
        Debug.Log("End: ");
        for (int i = 0; i < 5; i++)
        {
            Debug.Log(fingerPos[i]);
            fingers[i].SetEnd(fingerPos[i]);
        }
    }

    int[] DecodeCommand(string command)
    {
        int[] ret = new int[5];
        int retId = 0;

        //foreach(byte byte_v in System.Text.Encoding.UTF8.GetBytes(command))
        //{
        //   Debug.Log(byte_v);
        //}
        if (command.Length != 12)
        {
            return new int[0];
        }
        for (int i = 2; i < 12; i += 2)
        {
            ret[retId] = (command[i] << 8) + command[i + 1];
            //Debug.Log("#");
            //Debug.Log(ret[retId]);
            retId++;
        }
        return ret;
    }
}
