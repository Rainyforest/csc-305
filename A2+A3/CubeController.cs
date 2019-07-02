/*
UVic CSC 305, 2019 Spring
Helping lab for assignment03
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{

    public GameObject cubePrefab;
    public GameObject goalMarker;
    int boidCount = 10;

    private class CubeStatus
    {
        public Vector3 position;
        public GameObject boidObject;
    }
    private List<CubeStatus> statusList;


    void Start()
    {

        statusList = new List<CubeStatus>();
        for (int i = 0; i < boidCount; ++i)
        {
            for (int j = 0; j < boidCount; ++j)
            {
                GameObject newCube = new GameObject();
                newCube.transform.parent = gameObject.transform;
                newCube.name = "Cube No." + (i * boidCount + j).ToString();

                GameObject instPrefab = Instantiate(cubePrefab);
                instPrefab.transform.parent = newCube.transform;

                Vector3 startingPos = new Vector3((float)j - 4, (float)-3, (float)i - 4);
                CubeStatus status = new CubeStatus();
                status.position = startingPos + goalMarker.transform.position;
                newCube.transform.position = status.position;
                status.boidObject = newCube;
                statusList.Add(status);
            }
        }
        cubePrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < statusList.Count; ++i)
        {
            CubeStatus status = statusList[i];
            status.boidObject.transform.forward = Vector3.Normalize(goalMarker.transform.position - status.boidObject.transform.position);
            statusList[i] = status;
        }
    }
}
