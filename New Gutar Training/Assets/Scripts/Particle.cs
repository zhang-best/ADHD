using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public GameObject[] parGameObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //产生粒子特效
    public void MotivateEffect(bool success, float x, float y)
    {
        GameObject particles;
        if (success)
            particles = (GameObject)Instantiate(parGameObject[1]);
        else
            particles = (GameObject)Instantiate(parGameObject[0]);

        particles.transform.position = new Vector3(x, y, 1.79f);
#if UNITY_3_5
			particles.SetActiveRecursively(true);
#else
        particles.SetActive(true);
        for (int i = 0; i < particles.transform.childCount; i++)
            particles.transform.GetChild(i).gameObject.SetActive(true);
#endif

        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        if (ps != null && ps.loop)
        {
            ps.gameObject.AddComponent<CFX3_AutoStopLoopedEffect>();
            ps.gameObject.AddComponent<CFX_AutoDestructShuriken>();
        }
    }
}
