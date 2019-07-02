using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmulRomSpline3D //don't need monobehaviour
{
    private Vector3[] cps;
    private Vector3[] tangents;
    private float parametric_range;

    private Vector4 h_func(float t){
        float t2 = t*t;
        float t3 = t2*t;
        float hx = 2*t3 - 3* t2 + 1;
        float hy = t3 - 2*t2 +t;
        float hz = -2*t3 + 3*t2;
        float hw = t3 -t2;
        return new Vector4(hx,hy,hz,hw);
    }
    public float getArcLen(float startTime,float endTime){
        float t = startTime; //startTime in range (0,1)
        float arc_len = 0;
        Vector3 position = Sample(t);
        while (t < endTime){
            t += 0.001f;
            arc_len += (Sample(t) - position).magnitude;
            position = Sample(t);
        }
        return arc_len;
    }
    public Vector3[] control_points{
        set{
            cps = value;
            parametric_range = cps.Length - 4;
            tangents = new Vector3[cps.Length -3];
            for(int i=0;i<tangents.Length;++i){
                tangents[i] = (cps[i+2] - cps[i])/2.0f;
            }
        }
    }
    // // cp[0]--0--1--2--3--cp[1]
    public float[] getSampleTimes(int sampleNum){ 
        float[] sampleTable = new float[sampleNum+1];
        float deltaSample = 4/(float)(sampleNum);
            for(int i = 0;i<sampleNum+1;i++){
                sampleTable[i] = getArcLen(0,i*deltaSample);
                //Debug.LogFormat("i = {0},  value = {1}",i,sampleTable[i]);
            }
        return sampleTable;
    }
    public float interpolateTime(float s,int front,int end,float[] sampleTable){   
        //(index+1)*timeInterval
        //Debug.LogFormat("{0}-----{1}",front,end);
        float deltaSample = 4/(float)(sampleTable.Length - 1);
        int mid = Mathf.CeilToInt((front + end)/2);   
        if(front == end-1)return interpolate(s,sampleTable[front],sampleTable[end],(front)*deltaSample,(end)*deltaSample);
        else if(s <= sampleTable[mid])return interpolateTime(s,front,mid,sampleTable);
        else return interpolateTime(s,mid,end,sampleTable);
    }

    public float interpolate(float s,float s0,float s1,float t0,float t1){
        float alpha = (s-s0)/(s1-s0);
        return t0*(1-alpha)+t1*(alpha);
    }
    public Vector3 Sample(float t){
        if(t>0 && t<parametric_range){
            int knot_prev = Mathf.FloorToInt(t);
            float local_params = t - (float)(knot_prev);
            Vector4 h = h_func(local_params);
            Vector3 p0 = cps[knot_prev+1];
            Vector3 m0 = tangents[knot_prev];
            Vector3 p1 = cps[knot_prev+2];
            Vector3 m1 = tangents[knot_prev+1];
            return h.x*p0 + h.y*m0 +h.z *p1 +h.w*m1;
        }
        else{
            return  Vector3.zero;
        }
    }
}
