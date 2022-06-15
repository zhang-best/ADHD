using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringButtonController : MonoBehaviour
{
    public int index;//stringButton所使用的的传感器序号
    public bool isFinish;

    public AudioSource successAudio;
    public AudioSource failAudio;
    public GameObject paddle;
    public Light light;
    public GameObject target;
    public Particle particle;

    ForceSensor force;

    public SaveData saveData;
    ArrayList forceArray = new ArrayList();
    //test
    
    int i = 0;
    //
    public float tmpDurationTime;//累积时间的临时变量
    public float tmpReactTime;//反应时间的临时变量

    public float durationTime = 0.2f;//薄片维持在目标内以完成任务的时间,固定为200ms
    public float reactTime;//要求的反应时间

    public float w;//target的容许误差
    public float a = 1.0f;//target的目标力

    public enum State
    {
        PLAY_BREAK,
        PLAY_CARRYON,
    }

    State state;
    // Start is called before the first frame update
    void Awake()
    {
        //reactTime = 1.7f;//修改这里以修改允许的完成时间
        w = 0.4f;//修改这里以修改力控制容差
    }
    void Start()
    {
        successAudio = gameObject.GetComponent<AudioSource>();
        failAudio = GameObject.Find("Guitar").GetComponent<AudioSource>();
        paddle = transform.Find("Paddle").gameObject;
        light = transform.Find("Light").gameObject.GetComponent<Light>();
        target = transform.Find("target").gameObject;
        force = GameObject.Find("MP4623").GetComponent<ForceSensor>();
        particle = GameObject.Find("Main Camera").GetComponent<Particle>();
        saveData = GameObject.Find("Main Camera").GetComponent<SaveData>();

        isFinish = false;
        light.enabled = false;

        state = State.PLAY_BREAK;

       
    }
    //产生目标物体
    public void CreateTarget()
    {
        isFinish = false;
        //改变target的形态
        target.transform.position = new Vector3(target.transform.position.x, a, target.transform.position.z);
        target.transform.localScale = new Vector3(target.transform.localScale.x, w / 2.0f, target.transform.localScale.z);
        //产生
        target.SetActive(true);
        
        //变为任务态
        state = State.PLAY_CARRYON;
    }
    //查询任务是否完成
    public bool IsFinish()
    {
        return isFinish;
    }
    // Update is called once per frame
    void Update()
    {
        //查询当前按压力
        float currentF = force.fsl.m_FingerForce[index];
        //test
        //test[i] = (currentF); i++;
        //
        //paddle应该处于的位置值
        float y = currentF;
        paddle.transform.position = new Vector3(paddle.transform.position.x, y, paddle.transform.position.z);
        //若检测到按压，则点亮pointLight
        if (currentF >= 0.2)
        {
            light.enabled = true;
        }
        else
        {
            light.enabled = false;
        }

        if (this.state == State.PLAY_CARRYON)
        {
            //存储输出力数据
            forceArray.Add(currentF);

            tmpReactTime += Time.deltaTime;
            //判断任务时间是否超过反应时间要求
            if (tmpReactTime > reactTime)
            {
                          
                //任务完成（失败）
                isFinish = true;
                //target消失
                target.SetActive(false);
                //播放任务失败的动画和音效
                particle.MotivateEffect(false, target.transform.position.x, target.transform.position.y);
                failAudio.Play();
                tmpReactTime = 0;
                //
                state = State.PLAY_BREAK;
                //进行EXCEL数据记录
                saveData.writeExcel(0, 0, forceArray);

                tmpDurationTime = 0;
              
                forceArray.Clear();
            }
            else
            {
                //判断paddle是否处于target内
                if (paddle.transform.position.y > (target.transform.position.y - target.transform.lossyScale.y) && paddle.transform.position.y < (target.transform.position.y + target.transform.lossyScale.y))//误？lossyScale是全局缩放
                {
                    tmpDurationTime += Time.deltaTime;
                    //满足累计时间
                    if (tmpDurationTime > durationTime)
                    {

                        //任务完成（成功）
                        isFinish = true;
                        //target消失
                        target.SetActive(false);
                        //播放任务成功的动画
                        particle.MotivateEffect(true, target.transform.position.x, target.transform.position.y);
                        successAudio.Play();
                        //
                        state = State.PLAY_BREAK;
                        //进行EXCEL数据记录
                        saveData.writeExcel(1, tmpReactTime, forceArray);

                        tmpReactTime = 0;
                        tmpDurationTime = 0;
                        forceArray.Clear();
                    }
                }
                else
                {
                    tmpDurationTime = 0;
                }
            }
            
        }
        
  
    }
}
