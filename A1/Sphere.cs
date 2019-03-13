using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Sphere : MonoBehaviour {
    public Texture2D RayTracingResult;

    public static Color BackgroundColor = Color.grey;
    public static Color AmbientColor = Color.yellow; //R G B defined by myself
    public static Color LightColor = Color.white;
                              
    public static Vector3 LightDirection = new Vector3(1,2,3);

    public static Vector3 RayOrigin = Vector3.zero; 
    public static Vector3 VPCentre = Vector3.forward;
    public static Vector3 SphereCentre = new Vector3(5,6,10); //Define the position of spherecentre
    public static float SphereRadius = 1;

    float ifIntersect(Vector3 RayDirection){
        //Determine the ray intersecting with the sphere or not
       
        Vector3 Direction = RayDirection.normalized;
        // Vector3 intersect_normal;      
        Vector3 OE = SphereCentre - RayOrigin;
        float v = Vector3.Dot(OE,Direction);
        float RadiusSquare = SphereRadius*SphereRadius;
        float OE_Square = Vector3.Dot(OE,OE);
        return RadiusSquare - (OE_Square - v*v);    //return the discriminant of intersection formula
    }
       
    Color renderPixel(Color pixelColor, Vector3 RayDirection,float discriminant){
        //Determine the ray intersecting with the sphere or not
        Vector3 Direction = RayDirection.normalized;
        // Vector3 intersect_normal;
        float t;    
        Vector3 OE = SphereCentre - RayOrigin;
        float v = Vector3.Dot(OE,Direction);
        float diffuseStrength = 0.3f;
        float specularStrength = 0.5f;
        float specularPower = 4;
        float d = Mathf.Sqrt(discriminant);
        t = v - d;
        Vector3 Intersection = RayOrigin + Direction * t;   
        Vector3 normal = RayOrigin + Direction * v - SphereCentre;
        //calculation of the shaders
        pixelColor = AmbientColor;
        float diffuse = Vector3.Dot(normal,LightDirection) *diffuseStrength;
        pixelColor += LightColor * diffuse;
        Vector3 view = RayDirection*(-1);
        Vector3 half = view + RayDirection;
        // specular
        float blinn = Vector3.Dot(half,normal);
        float specular = Mathf.Pow(blinn, specularPower)*specularStrength;
        pixelColor += LightColor * specular;
        return pixelColor;
    }
    void Start () {
        /////
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

        for(int i = 0;i < pixel_width;++i){
            for(int j = 0; j < pixel_height;++j){
                Vector3 RayDirection = VPCentre;
                RayDirection.x = (i - pixel_width_half) / (float)pixel_width_half * VPWidthHalf;
                RayDirection.y = (j - pixel_height_half) / (float)pixel_height_half * VPHeightHalf;
                Color pixelColor = Color.yellow;

                float discriminant = ifIntersect(RayDirection);
                RayTracingResult.SetPixel(i,j,discriminant>0?renderPixel(pixelColor,RayDirection,discriminant):BackgroundColor); 
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