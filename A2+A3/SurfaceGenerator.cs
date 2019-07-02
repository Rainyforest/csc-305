/*
UVic CSC 305, 2019 Spring
Helping lab for assignment02
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceGenerator : MonoBehaviour {

    public Material HeightMaterial;
    public GameObject objectToPlace;
	// Use this for initialization
	void Start () {
        MeshFilter mesh_filter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = mesh_filter.mesh;
        Debug.Break();
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer) renderer.material = HeightMaterial;
        GenerateSurface(mesh);
	}
	// Update is called once per frame
	void Update () {
		Vector4 LightDir = new Vector4(Mathf.Cos(Time.time), 1, Mathf.Sin(Time.time), 0);
        HeightMaterial.SetVector("_LightDir", LightDir.normalized);
	}
    void ObjectPlacer(Mesh mesh){
        
    }
    void GenerateSurface(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] indices = mesh.triangles;
        Vector2[] uvs = mesh.uv;
       
        mesh.Clear();

        //subdivision = how many squares per row/col
        int subdivision = 250;
        int stride = subdivision + 1;
        int num_vert = stride * stride;
        int num_tri = subdivision * subdivision * 2;

        indices = new int[num_tri * 3];
        int index_ptr = 0;
        for (int i = 0; i < subdivision; i++)
        {
            for (int j = 0; j < subdivision; j++)
            {
                int quad_corner = i * stride + j;
                indices[index_ptr] = quad_corner;
                indices[index_ptr+1] = quad_corner+stride;
                indices[index_ptr+2] = quad_corner+stride+1;
                indices[index_ptr+3] = quad_corner;
                indices[index_ptr+4] = quad_corner+stride+1;
                indices[index_ptr+5] = quad_corner+1;
                index_ptr += 6;
            }
        }
        Debug.Assert(index_ptr == indices.Length);

        const float xz_start = -5;
        const float xz_end = 5;
        float step = (xz_end - xz_start) / (float)(subdivision);
        vertices = new Vector3[num_vert];
        uvs = new Vector2[num_vert];

        Perlin2DGenerator perlin = new Perlin2DGenerator();
        float[,] height1 = perlin.Perlin2DNoise(stride,(float)5,4);
        float[,] height2 = perlin.Perlin2DNoise(stride,(float)7,4);
        float[,] height3 = perlin.Perlin2DNoise(stride,(float)9,4);   
        for (int i = 0; i < stride; i++)
        {
            for (int j = 0; j < stride; j++)
            {
                // notice the bahavior here
                bool show_backface = false;
                float cur_x;
                float cur_z;
                //i don't know how this happened(showing back faces)
                if (show_backface)
                {
                    cur_x = xz_start + i * step;
                    cur_z = xz_start + j * step;
                }
                else
                {
                    cur_x = xz_start + j * step;
                    cur_z = xz_start + i * step;
                }
                int water_dom = 50;
                float height_sum = (float)((height1[i,j]+height2[i,j]+height3[i,j])*5/8.75);
                float water_ripple = (Mathf.Sin(i/2/Mathf.PI)+Mathf.Sin(j/2/Mathf.PI)-2+Random.value-Random.value)/water_dom;
                float cur_y = (float)(height_sum>-0.001 ? height_sum:(water_ripple>-0.001?-0.001:water_ripple));    
                vertices[i * stride + j] = new Vector3(cur_x, cur_y, cur_z);
                uvs[i * stride + j] = new Vector2(cur_x/10.0f , cur_z/10.0f);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        int num_objs = 10;
        int rand_index = 0;
        for(int i = 0;i<num_objs;i++){
                rand_index = (int)Mathf.Floor(Random.value*(stride*stride-1));
                Vector3 pos = vertices[rand_index];
                if(pos.y>2.2){
                    Vector3 tree_normal = mesh.normals[rand_index];
                    
                    Instantiate(objectToPlace); 
                    objectToPlace.transform.LookAt(tree_normal,Vector3.up);
                    // Debug.Log(objectToPlace.transform.rotation);
                    objectToPlace.transform.position = (pos);    
                }
        }
        
    }
}