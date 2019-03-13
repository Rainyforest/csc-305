using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public Texture2D RayTracingResult;
    
    public static Color BackgroundColor = Color.grey;

    public static Vector3 RayOrigin = Vector3.zero; 
    public static Vector3 VPCentre = Vector3.forward;
    

    // 1   3   5
    //
    // 0   2   4
    private float cal33Det(float[] arr){
        // 0 1 2 
        // 3 4 5
        // 6 7 8 
      
        return arr[0]*(arr[4]*arr[8]-arr[5]*arr[7])-arr[1]*(arr[3]*arr[8]-arr[5]*arr[6])+arr[2]*(arr[3]*arr[7]-arr[4]*arr[6]);
    }
    private Color IntersectTriangle(Vector3 origin,Vector3 direction,Vector3 vA, Vector3 vB,Vector3 vC){   

        Vector3 N = Vector3.Cross((vA - vB), (vA - vC));
        float t = Vector3.Dot(N, (vA - origin))/(float)Vector3.Dot(N, direction);
        Vector3 p = origin + direction * t;
        float alpha,beta,gamma;
        float ax= vA.x;
        float ay= vA.y;
        float az= vA.z;

        float bx= vB.x;
        float by= vB.y;
        float bz= vB.z;

        float cx= vC.x;
        float cy= vC.y;
        float cz= vC.z;

        float dx= direction.x;
        float dy= direction.y;
        float dz= direction.z;
        float[] det_beta =  new float[9]  {ax, ax-cx,dx,
                                    ay, ay-cy,dy,
                                    az, az-cz,dz};
        float[] det_gamma = new float[9] {ax - bx, ax,dx,
                                    ay - by, ay,dy,
                                    az - bz, az,dz};
        float[] det_t = new float[9] {ax - bx, ax - cx,ax,
                                ay - by, ay - cy,ay,
                                az - bz, az - cz,az};
        float[] det_A = new float[9] {ax - bx, ax - cx,dx,
                                ay - by, ay - cy,dy,
                                az - bz, az - cz,dz};
        float det_A_val = cal33Det(det_A); 
        beta = cal33Det(det_beta)/det_A_val;
        gamma = cal33Det(det_gamma)/det_A_val;
        t = cal33Det(det_t)/det_A_val;
        alpha = 1 - beta - gamma;
        return ( alpha <= 0 || alpha >=1 || beta <= 0 || beta >=1 || gamma <= 0 || gamma >=1)?BackgroundColor : new Color(alpha,beta,gamma);
    }
    // Start is called before the first frame update
    void Start()
    {   
        Camera this_camera = gameObject.GetComponent<Camera>();
        Debug.Assert(this_camera);
        int pixel_width = this_camera.pixelWidth;
        int pixel_height = this_camera.pixelHeight;

        RayTracingResult = new Texture2D(pixel_width, pixel_height);
        /////
        float ViewportWidth = 3;
        float ViewportHeight = ViewportWidth  / pixel_width * pixel_height;

        float VPWidthHalf = ViewportWidth / 2;
        float VPHeightHalf = ViewportHeight / 2;

        int pixel_width_half = pixel_width / 2;
        int pixel_height_half = pixel_height / 2;

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(-4, -2.8f, 10)); //0
        vertices.Add(new Vector3(-4, 2.8f, 10));  //1
        vertices.Add(new Vector3(0, -2.8f, 9));   //2
        vertices.Add(new Vector3(0, 2.8f, 9));    //3
        vertices.Add(new Vector3(4, -2.8f, 10));  //4
        vertices.Add(new Vector3(4, 2.8f, 10));   //5
//new Vector3(-4, -2.8f, 10),new Vector3(-4, 2.8f, 10),new Vector3(0, -2.8f, 9)
        for(int i = 0;i < pixel_width;++i){
            for(int j = 0; j < pixel_height;++j){
                Vector3 RayDirection = VPCentre;
                RayDirection.x = (i - pixel_width_half) / (float)pixel_width_half * VPWidthHalf;
                RayDirection.y = (j - pixel_height_half) / (float)pixel_height_half * VPHeightHalf;
                Color pixelColor = BackgroundColor;
                Vector3 Direction = RayDirection.normalized;
                Color Tri_1 = IntersectTriangle(RayOrigin,Direction,vertices[0],vertices[1],vertices[2]);
                Color Tri_2 = IntersectTriangle(RayOrigin,Direction,vertices[2],vertices[1],vertices[3]);
                Color Tri_3 = IntersectTriangle(RayOrigin,Direction,vertices[2],vertices[3],vertices[5]);
                Color Tri_4 = IntersectTriangle(RayOrigin,Direction,vertices[2],vertices[5],vertices[4]);
                pixelColor = Tri_1 == BackgroundColor ? pixelColor : Tri_1;
                pixelColor = Tri_2 == BackgroundColor ? pixelColor : Tri_2;
                pixelColor = Tri_3 == BackgroundColor ? pixelColor : Tri_3;
                pixelColor = Tri_4 == BackgroundColor ? pixelColor : Tri_4; 
                RayTracingResult.SetPixel(i,j,pixelColor);
               
            }
        }
        RayTracingResult.Apply();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Show the generated ray tracing image on screen
        Graphics.Blit(RayTracingResult, destination);
    }
}
