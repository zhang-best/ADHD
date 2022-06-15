using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class ForceSensor : MonoBehaviour
{
    public ForceSensorLogic fsl;
    
    void Awake()
    {
        fsl = new ForceSensorLogic(4);//只采集Mp4623的前4路

    }
    // Start is called before the first frame update
    void Start()
    {
        fsl.InitialADCard();//初始化AD采集卡                         
        InvokeRepeating("CallData2", 2f, 0.05f);//固定时间调用 数据更新函数  2s后执行  每秒调用20次
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CallData2()
    {
        fsl.CallData();
    }
}
[System.Serializable]
public class ForceSensorLogic
{
    private Int32[] addata;//PC里定义的数据缓冲区
    public int ADNum;//采集ADnum路信号（取值1-15）

    public float[] m_FingerForce;//当前采集的电压值（0-5000mv）

    private float[] m_BiasedForce;//用于去除初始偏移
    private long biasedFrameNum = 0;

    private IntPtr hDevice;//采集卡的操作句柄   

    [DllImport("MP4623", EntryPoint = "MP4623_OpenDevice")]
    public static extern IntPtr MP4623_OpenDevice(Int32 dev_num);
    [DllImport("MP4623")]
    public static extern Int32 MP4623_CAL(IntPtr hDevice);
    [DllImport("MP4623")]
    public static extern Int32 MP4623_AD(IntPtr hDevice, Int32 stch, Int32 endch, Int32 gain, Int32 sidi, Int32 trsl, Int32 trpol, Int32 clksl, Int32 clkpol, Int32 tdata);
    [DllImport("MP4623")]
    public static extern Int32 MP4623_ADRead(IntPtr hDevice, Int32 rdlen, Int32[] addata);
    [DllImport("MP4623.DLL")]
    public static extern Int32 MP4623_ADStop(IntPtr hDevice);
    [DllImport("MP4623.DLL")]
    public static extern Int32 MP4623_ADPoll(IntPtr hDevice);
    const int FACTOR = 55;
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="ADNum"></param>
    public ForceSensorLogic(int ADNum)
    {
        this.ADNum = ADNum;//4表示只采集前4路AD
        m_FingerForce = new float[ADNum];
        m_BiasedForce = new float[ADNum];
        addata = new Int32[600000];
    }

    /// <summary>
    /// 初始化采集卡，并开始数据采集
    /// </summary>
    public void InitialADCard()
    {

        hDevice = MP4623_OpenDevice(0);//打开设备，返回采集卡的操作句柄

        //if (hDevice == INVALID_HANDLE_VALUE) Debug.LogError("AD采集卡不存在");

        if (MP4623_CAL(hDevice) != 0) Debug.LogError("AD采集卡校准失败");//进行校准 

        if (MP4623_AD(hDevice, 0, ADNum - 1, 1, 0, 0, 0, 0, 0, 100) != 0) Debug.LogError("AD采集开启失败");//设置采样参数并启动 AD 采样，0、14表示设置0~14为采样通道 

    }

    /// <summary>
    /// 固定时间调用此函数，更新手指数据
    /// </summary>
    public void CallData()
    {

        //Thread.Sleep(10);

        int _x = MP4623_ADPoll(hDevice);//获得当前FIFO中的数据个数
        if (_x < 0)
        {
            Debug.Log("ad fifo over!!!");
        }
        else
        {
            if (MP4623_ADRead(hDevice, _x, addata) == -1)//读取数据（FIFO中的数据全部读取到addata数组）
                Debug.Log("ad fifo over!!!");
            else
            {
                for (int i = 0; i < m_FingerForce.Length; i++)
                {
                    if (biasedFrameNum < 10)
                    {
                        m_BiasedForce[i] += -((addata[i] >> 4) - 2048) * 5000 / 2048;//mp4623 is 12bit ad
                    }
                    else if (biasedFrameNum == 10)
                    {
                        m_BiasedForce[i] = m_BiasedForce[i] / 10f;
                    }
                    else
                    {
                        m_FingerForce[i] = (-((addata[i] >> 4) - 2048) * 5000 / 2048 - m_BiasedForce[i]) / FACTOR;
                    }
                }
                //Debug.Log(string.Format("Biased Data: {0}, {1}, {2}, {3}, {4}", m_BiasedForce[0],m_BiasedForce[1],m_BiasedForce[2],m_BiasedForce[3],m_BiasedForce[4]));
                biasedFrameNum++;
            }
        }
    }

}
