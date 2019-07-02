using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   
using System;
public class DayNightCycle : MonoBehaviour
{
    public float time;
    public TimeSpan currenttime;
    public Transform SunTransform;
    public Light Sun;
    public Text timetext;
    public int days;
    public float intensity;
    public Color fogday = Color.white;
    public Color fognight = Color.black;
    public int speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        ChangeTime();
    }
    public void ChangeTime(){
        time += Time.deltaTime *speed;
        if(time > 86400){
            days +=1;
            time =0;
        }
        currenttime = TimeSpan.FromSeconds(time);
        string[]temptime = currenttime.ToString().Split(":"[0]);
        timetext.text = temptime[0] + ":"+ temptime[1];
        SunTransform.rotation = Quaternion.Euler(new Vector3((time-21600)/86400*360,0,0));
        if(time <43200){
            intensity = 1 - (43200 - time)/43200;

        }else
        {
             intensity = 1-((43200 - time)/43200*(-1));
             
        }
        RenderSettings.fogColor = Color.Lerp(fognight,fogday,intensity*intensity);
        Sun.intensity = intensity+1/2;
    }
}
