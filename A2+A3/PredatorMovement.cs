using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorMovement : MonoBehaviour
{
    private Vector3 velocity;
    private float deltaVelo =  0.15f;
    float key_speed = 5.0f;
    public  GameObject boid;
    //float scroll_speed = 1000.0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0,4,0);
  
        velocity = new Vector3(Random.value,0,Random.value).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 boid_pos = boid.transform.position;
        Vector3 self_pos = transform.position;
        velocity += (boid_pos-self_pos + 3*new Vector3(Random.value,0,Random.value)).normalized;

        if (Input.GetKey("up")){
            velocity += new Vector3(0.0f, 0.0f,Time.deltaTime * key_speed);
        }
        if (Input.GetKey("down")){
            velocity -= new Vector3(0.0f, 0.0f,Time.deltaTime * key_speed);
        }
        if (Input.GetKey("left")){
            velocity -= new Vector3(Time.deltaTime * key_speed, 0.0f,0.0f);
        }
        if (Input.GetKey("right")){
            velocity += new Vector3(Time.deltaTime * key_speed, 0.0f,0.0f);
        }
        // if (Input.GetAxis("Mouse ScrollWheel")!=0.0f ){
        //     velocity += new Vector3(0.0f,Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scroll_speed,0.0f);
        // }
        velocity = velocity.normalized;
        transform.position += deltaVelo*velocity; 
        transform.rotation = Quaternion.LookRotation(velocity.normalized,Vector3.up);
        transform.Rotate(0,90,90); 
    }
}
