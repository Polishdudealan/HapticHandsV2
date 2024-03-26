using System.Collections;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;
using System.Text;



public class CommManager : MonoBehaviour
{
    public string port = "COM20";

    public bool test_calibrate_start = false;
    public bool test_calibrate_end = false;

    SerialPort serial;
    public Hand hand;

    public bool isRightHand = true;

    private Queue outputQueue;
    private Queue inputQueue;

    private int[] curAngleSet = { 0, 0, 0, 0, 0};
    private bool[] curCollisionSet = { false, false, false, false, false };

    private char SoP = '$';

    private float prevTime = 0F;
    private float sendPeriod = 0.05F;

    private bool valChecked = false;

    private bool primaryPressedFlag = false;
    private bool secondaryPressedFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        serial = new SerialPort(port, 115200);
        serial.Encoding = System.Text.Encoding.UTF8;
        Debug.Log("Start");
        serial.Open();
        Debug.Log(serial.IsOpen);
        serial.ReadTimeout = 50;

        outputQueue = Queue.Synchronized(new Queue());
        inputQueue = Queue.Synchronized(new Queue());
        Thread thread = new Thread(ThreadLoop);
        thread.Start();
    }

    private void OnApplicationQuit()
    {
        serial.Close();
    }

    public void Send(Byte[] message)
    {
        outputQueue.Enqueue(message);
    }

    public string Read()
    {
        if (inputQueue.Count == 0)
            return null;
        return (string)inputQueue.Dequeue();
    }

    public void ClearInput()
    {
        inputQueue.Clear();
    }

    public void Write(byte[] message)
    {
        serial.Write(message, 0, 17);
        serial.BaseStream.Flush();
    }

    public string Retrieve(int timeout = 50)
    {
        string ret = "";
        char data = ' ';
        try
        {
            data = (char)serial.ReadByte();
            //Debug.Log(data);
            if (data != '$')
            {
                return null;
            }
            ret += data;
            for (int i = 0; i < 11; i ++)
            {
                data = (char)serial.ReadByte();
                //Debug.Log(System.Text.Encoding.UTF8.GetBytes("" + data)[0]);
                ret += data;
                //Debug.Log(i);
            }
            //Debug.Log("exit");
            return ret;
        }
        catch (TimeoutException)
        {
            return null;
        }
    }

    public void ThreadLoop()
    {
        while (true)
        {
            // Send
            if (outputQueue.Count != 0)
            {
                Byte[] command = new Byte[17];
                command = (Byte[])outputQueue.Dequeue();
                //Debug.Log(command.Length);
                Write(command);
            }
            // Read
            string result = Retrieve();
            if (result != null)
                inputQueue.Enqueue(result);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyUp("space")) {
        //     primaryPressedFlag = true;
        //     hand.SoftCalibrateEnd();
        // }
        // if (Input.GetKeyUp("enter")) {
        //     secondaryPressedFlag = true;
        //     hand.SoftCalibrateStart();
        // }
        //Debug.Log(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch));
        if (isRightHand) {
            if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
                primaryPressedFlag = true;
            }
            else if (primaryPressedFlag && !OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
                primaryPressedFlag = false;
                hand.SoftCalibrateEnd();
            }

            if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
                secondaryPressedFlag = true;
            }
            else if (secondaryPressedFlag && !OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
                secondaryPressedFlag = false;
                hand.SoftCalibrateStart();
            }
        }
        else {
            if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch)) {
                primaryPressedFlag = true;
            }
            else if (primaryPressedFlag && !OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch)) {
                primaryPressedFlag = false;
                hand.SoftCalibrateEnd();
            }

            if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
                secondaryPressedFlag = true;
            }
            else if (secondaryPressedFlag && !OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
                secondaryPressedFlag = false;
                hand.SoftCalibrateStart();
            }
        }
        

        if (Time.time > prevTime + sendPeriod)
        {
            valChecked = false;
            prevTime = Time.time;
            SendServoPos();
        }
        string message = Read();
        if (message != null)
        {
            DecodeMessage(message);
            //Debug.Log(message);
            //ClearInput();
        }
        
        if (test_calibrate_start)
        {
            hand.SoftCalibrateStart();
            test_calibrate_start = false;
        }
        if (test_calibrate_end)
        {
            hand.SoftCalibrateEnd();
            test_calibrate_end = false;
        }
    }

    void DecodeMessage(string msg)
    {
        // checksum check here
        switch (msg[1]) {
            case '1':
                if (!valChecked)
                {
                    hand.UpdateHand(msg);
                    valChecked = true;
                }
                break;
            case 'Z':
                //Debug.Log("start enter");
                hand.CalibrateStart(msg);
                break;
            case 'M':
                hand.CalibrateEnd(msg);
                break;
            default:
                break;
        }
    }

    void SendServoPos()
    {
        Byte[] msg = new Byte[17];
        msg[0] = (byte)Encoding.ASCII.GetBytes("$")[0];
        msg[1] = (byte)Encoding.ASCII.GetBytes("1")[0];

        for (int i = 0; i < 5; i++)
        {   
            msg[3*i + 2] = (byte)((curAngleSet[i] & 0xFF00) >> 8);
            msg[3*i + 3] = (byte)(curAngleSet[i] & 0x00FF);
            msg[3*i + 4] = (byte)Encoding.ASCII.GetBytes((curCollisionSet[i]) ? "1" : "0")[0];

        }
        //msg += '^';
        string print = "";
        for (int i = 0; i < 5; i ++)
        {
            print += curCollisionSet[i];
        }
        // Debug.Log(print);

        Send(msg);
    }

    public void SetServoPos(int id, int pos, bool collision = false)
    {
        curAngleSet[id] = pos;
        curCollisionSet[id] = collision;
        if (collision)
        {
            //Debug.Log("Collision scheduled");
        }
    }
}