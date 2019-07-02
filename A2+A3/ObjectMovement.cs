using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System;
public class ObjectMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(.0001f,1)]
    public int knot_num = 4;
    private float curr_t;
    private float curr_s = 0;
    private float range = 1.5f;
    private float followRadius =2f;
    private float[] sampleTable;
    private int sampleNum = 40;
    private float collision_threshold = 2.5f;
    private float predator_threshold = 3.5f;
    private float predator_capture_range = 2f;
 
    private CatmulRomSpline3D spline;
    public GameObject follower_boid;
    public GameObject predator_boid;
    public List<GameObject> object_list;
   
    void Start()
    {
        spline = new CatmulRomSpline3D();
        Vector3[] cps = new Vector3[8]; //initialize 8 points 
        cps[0] = new Vector3(-3,4,-3);
        cps[1] = new Vector3(-3,4,3);
        cps[2] = new Vector3(3,4,3);
        cps[3] = new Vector3(3,4,-3);
        cps[4] = cps[0];
        cps[5] = cps[1];
        cps[6] = cps[2];
        cps[7] = cps[3];
        spline.control_points = cps;
        sampleTable = spline.getSampleTimes(sampleNum);
        
        curr_t = 0;
        object_list = new List<GameObject>();
  
        for(int i = 0;i<50;++i){
            GameObject boid = Instantiate(follower_boid);
           
            Vector3 relative_pos = new Vector3 (
                (Random.value - 0.5f)* 2 * followRadius,
                (Random.value - 0.5f)* 2 * followRadius,
                (Random.value - 0.5f)* 2 * followRadius
            );
          
            // SetParent(gameObject,boid);
            boid.name = "Object No." + i;
            boid.transform.position = gameObject.transform.position+relative_pos;

            object_list.Add(boid);            
        }   
    }
    // public void SetParent(GameObject parent,GameObject child)
    // {
    //     child.transform.parent = parent.transform;
    // }
    // Update is called once per frame
    Vector3 LeaderBoidForward(){
        float deltaS = 0.3f;
        float fullArcLen = spline.getArcLen(0,4);
        float dis1 = curr_s-sampleTable[sampleTable.Length-2]-(float)0.2;
        float dis2 = sampleTable[1] - curr_s -2;
        if(dis1>=0||dis2>=0){
            deltaS += 2.5f;
        }
        float next_s = curr_s + deltaS;
        float next_t;
        if (next_s > fullArcLen){
            next_s = next_s - fullArcLen;
        } 
        
        next_t = spline.interpolateTime(next_s,0,sampleTable.Length-1,sampleTable);
       // Debug.LogFormat("############ t:{0} {1}--{2}",curr_t,curr_s,sampleTable[40]);
        curr_s = next_s;
        Vector3 curr_pos = EvaluateAt(curr_t);
        Vector3 next_pos = EvaluateAt(next_t);
        curr_t = next_t;
        gameObject.transform.position = curr_pos;

        Vector3 forward = next_pos - curr_pos;
        if (forward.sqrMagnitude > 1e-5)
        {
            forward.Normalize();
            gameObject.transform.forward = forward;
            gameObject.transform.Rotate(0,90,90); 
        }
        return forward;
    }
    void FollowerBoidForward(Vector3 leaderForward){
    //    List<GameObject> obstacles = new List<GameObject>();
        
        float available_thrust = 0.2f;
        float alignment_weight = 0.5f;
        float cohesion_weight = 0.1f;
        float collision_weight = 0.5f;
        foreach(GameObject obj in object_list){
        
            Vector3 self_pos = obj.transform.position;
            // Debug.Log(self_pos);
            List<GameObject> neighbors = getNeighbors(self_pos, range);
            Vector3 velocity = Vector3.zero;
            Vector3 alignment = getAlignment(obj,neighbors) * alignment_weight;
            Vector3 cohesion = getCohesion(obj) * cohesion_weight;
            Debug.Log(cohesion);
            Vector3 initVelo = cohesion + alignment;
        
            
            Vector3 avoidance = getAvoidance(obj,velocity,neighbors,predator_boid) * collision_weight;
            Vector3 total_demand = avoidance + initVelo;
            Debug.LogFormat("alignment :: {0}",alignment);
            Debug.LogFormat("cohesion  :: {0}",cohesion);
            Debug.LogFormat("avoidance :: {0}",avoidance);
            Debug.LogFormat("================");
            velocity += total_demand.normalized * available_thrust;
            obj.transform.position += velocity;
            obj.transform.rotation = Quaternion.LookRotation(velocity.normalized,Vector3.up);
            obj.transform.Rotate(0,90,90);
        }
    
    }
    List<GameObject> getNeighbors(Vector3 self_pos,float range){
        List<GameObject> visible_list = new List<GameObject>();
        foreach(GameObject obj in object_list){
            if ((obj.transform.position - self_pos).magnitude<=range)visible_list.Add(obj);
        }
        return visible_list;

    }

    Vector3 getAvoidance(GameObject self,Vector3 velocity,List<GameObject> neighbors,GameObject obstacles){
        Vector3 avoidance = new Vector3(0,0,0);
        foreach(GameObject nb in neighbors){
            Vector3 p0 = self.transform.position;
            Vector3 p1 = nb.transform.position;
            float distance = (p0-p1).magnitude;
            if(distance<=collision_threshold){
                avoidance += (p0-p1).normalized;
            }
            avoidance = (velocity.normalized+avoidance.normalized)/2;
        }
        float distance2 = (self.transform.position-obstacles.transform.position).magnitude;
        if(distance2<=predator_threshold){
                if(distance2<predator_capture_range){
                    object_list.Remove(self);
                    Destroy(self,1.5f);
                    return -velocity;
                }
                avoidance += 2*(self.transform.position-obstacles.transform.position).normalized;
        }
        return avoidance.normalized;
    }
    Vector3 getAlignment(GameObject self,List<GameObject> neighbors){
        Vector3 alignment = (gameObject.transform.position-self.transform.position);
        return alignment.normalized;
    }
    Vector3 getCohesion(GameObject self){
        Vector3 sum = Vector3.zero;
        foreach(GameObject obj in object_list){
            Vector3 pos = obj.transform.position;
            sum+=pos;
        }
        //Debug.Log((sum/(object_list.Count)-self.transform.position).normalized);
        return (sum/(object_list.Count)-self.transform.position).normalized;
    }
    void Update()
    {
        Vector3 leaderForward = LeaderBoidForward();
        FollowerBoidForward(leaderForward);
        
    }//if use FixedUpdate

    Vector3 EvaluateAt (float t){
        if(t>knot_num) t-=knot_num;
        return spline.Sample(t);
    }
}
