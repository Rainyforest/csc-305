using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    float key_speed = 10.0f;
    float scroll_speed = 1000.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
        {
            transform.position += new Vector3(0.0f,
                                       0.0f,Time.deltaTime * key_speed);
        }
        if (Input.GetKey("down"))
        {
            transform.position -= new Vector3(0.0f,
                                       0.0f,Time.deltaTime * key_speed);
        }
        if (Input.GetKey("left"))
        {
            transform.position -= new Vector3(Time.deltaTime * key_speed,
                                       0.0f,0.0f);
        }
        if (Input.GetKey("right"))
        {
            transform.position += new Vector3(Time.deltaTime * key_speed,
                                       0.0f,0.0f);
        }
        if (Input.GetAxis("Mouse ScrollWheel")!=0.0f ) 
        {
            transform.position += new Vector3(0.0f,
                                       Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scroll_speed,0.0f);
        }
        
    }
}
