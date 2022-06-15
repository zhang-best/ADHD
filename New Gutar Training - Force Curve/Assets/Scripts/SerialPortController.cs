using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System;
using UnityEngine;

public class SerialPortController : MonoBehaviour
{

    //定义串口基本信息
    public string portName = "COM3";
    public int baudRate = 115200;
    public Parity parity = Parity.None;
    public int dataBits = 8;
    public StopBits stopBits = StopBits.One;

    SerialPort sp = null;//串口控制

    // Use this for initialization
    void Start()
    {

        OpenPort();//打开串口
        byte[] test = new byte[5] { 0x01, 0xE1, 0x01, 0x00, 0x1F };
        //WriteData(test);

    }

    public void OpenPort()
    {
        sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        sp.ReadTimeout = 400;
        try
        {
            sp.Open();

        }
        catch (Exception ex)
        {

            Debug.Log(ex.Message);

        }

    }

    public void ClosePort()
    {
        try
        {
            sp.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void WriteData(byte[] bys)
    {

        if (sp.IsOpen)
        {
            sp.Write(bys, 0, bys.Length);


        }

    }


    void OnApplicationQuit()
    {
        ClosePort();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
