using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    int index;//当前访问的目标物体序号
    enum State
    {
        START,
        PLAY_BREAK,
        PLAY_CARRYON,
    }
    public float sessionTime;// 整个游戏流程的总试次数目
    public float sessionTimeC;// 整个游戏流程的当前累计试次数目

    public float timeBreak;// 试次之间间隔的休息时间
    private float timeBreakC;// 试次之间间隔的当前已休息时间
    public SerialPortController sp;/// <summary>
    State state;

    public StringButtonController[] stringButton;

    void Start()
    {
        Application.targetFrameRate = 60;
        sessionTime = 100;//
        timeBreak = 2;//

        this.state = State.START;

        this.timeBreakC = 0;

        this.sessionTimeC = 0;

        sp = GameObject.Find("SerialPort").GetComponent<SerialPortController>();

    }

    void Update()
    {
       
        if (this.state == State.START)
        {
            if (Input.GetKeyDown(KeyCode.Space))//空格键触发游戏
            {

                this.state = State.PLAY_BREAK;

            }
        }
        else if (this.state == State.PLAY_BREAK)//PLAY_BREAK为按压游戏结束后的休息时间
        {
            this.timeBreakC += Time.deltaTime;
            if (this.timeBreakC >= this.timeBreak)
            {
                this.timeBreakC = 0;

                this.state = State.PLAY_CARRYON;

                //目标出现并随机选择位置
                index = UnityEngine.Random.Range(0, 2);
                stringButton[index].CreateTarget();
                //发送marker-0
                byte[] marker = new byte[5] { 0x01, 0xE1, 0x01, 0x00, 0x00 };//////////////
                sp.WriteData(marker);//////////////// 
            }
        }

        else if (this.state == State.PLAY_CARRYON)//PLAY_CARRYON为按压任务的trail过程
        {

                //判断本次trail是否完成
            if (stringButton[index].IsFinish())
            {
                //发送marker-1
                byte[] marker = new byte[5] { 0x01, 0xE1, 0x01, 0x00, 0x01 };//////////////
                sp.WriteData(marker);//////////////// 
                this.sessionTimeC += 1;
                if (this.sessionTimeC >= this.sessionTime)//当前session已完成
                {
                    this.sessionTimeC = 0;
                    this.state = State.START;


                    Debug.Log("本次训练已结束...");
                }
                else//没有完成，继续下一个trail
                {

                    this.state = State.PLAY_BREAK;
                }

            }           
        }
    }


}
