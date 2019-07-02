using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin2DGenerator  {
	void Start () {
    }

    //f(t) = 6t^5-15t^4+10t^3
    float interpolation_function(float t)
    {
        float t_cubic = t * t * t;
        float t_square = t * t;

        return 6 * t_cubic * t_square - 15 * t_square * t_square + 10 * t_cubic;
    }

    public float [,] Perlin2DNoise(int grid_len_count, float max_value, int frequency)
    {
        Vector2[,] gradients = new Vector2[frequency+1,frequency+1];
        //method 1. initialize random gradients with random numbers
        
        for (int i = 0; i < frequency+1; ++ i){
            for (int j = 0; j < frequency+1; ++ j){
                Vector2 rand_vector = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1);
                gradients[i,j] = rand_vector.normalized;
            }
        }    
        //method 2. random pick from the 4 edge centers(Simplified perlin)
        
        Vector2[] edge_centers = new Vector2[4];
        edge_centers[0] = (new Vector2(Random.value * 2 - 1, Random.value * 2 - 1)).normalized;
        edge_centers[1] = (new Vector2(Random.value * 2 - 1, Random.value * 2 - 1)).normalized;
        edge_centers[2] = (new Vector2(Random.value * 2 - 1, Random.value * 2 - 1)).normalized;
        edge_centers[3] = (new Vector2(Random.value * 2 - 1, Random.value * 2 - 1)).normalized;

        for (int i = 0; i < frequency; ++ i){
            for (int j = 0; j < frequency; ++ j){
                float roll = Random.value;
                if (roll < 0.25f) gradients[i,j] = edge_centers[0];
                else if (roll < 0.5f) gradients[i,j] = edge_centers[1];
                else if (roll < 0.75f) gradients[i,j] = edge_centers[2];
                else gradients[i,j] = edge_centers[3];
            }
        }
        
        //generate two (instead of 4) dot products and blend them together
        float[,] noise = new float[grid_len_count+1,grid_len_count+1];
        float period = 1.0f / frequency;
        float step = 1.0f / grid_len_count;
        for (int i = 0; i < grid_len_count; ++ i){//avoid array out of bound exception
            for (int j = 0; j < grid_len_count; ++ j)
            {
                float location_period_x = step * i / period;
                int cell_x = Mathf.FloorToInt(location_period_x);
                float location_period_y = step * j / period;
                int cell_y = Mathf.FloorToInt(location_period_y);
                float in_cell_location_x = location_period_x - cell_x;
                float in_cell_location_y = location_period_y - cell_y;
                // Debug.Log(cell_x);
                // Debug.Log(cell_y);
                float dot_s = Vector2.Dot(gradients[cell_x,cell_y], new Vector2(in_cell_location_x, in_cell_location_y));
                float dot_t = Vector2.Dot(gradients[cell_x+1,cell_y], new Vector2(in_cell_location_x-1, in_cell_location_y));
                float dot_u = Vector2.Dot(gradients[cell_x,cell_y+1], new Vector2(in_cell_location_x, in_cell_location_y-1));
                float dot_v = Vector2.Dot(gradients[cell_x+1,cell_y+1], new Vector2(in_cell_location_x-1, in_cell_location_y-1));
                float alpha = interpolation_function(in_cell_location_x);
                float st = (1-alpha)*dot_s+alpha*dot_t;
                float uv = (1-alpha)*dot_u+alpha*dot_v;
                alpha = interpolation_function(in_cell_location_y);
                float height = max_value*((1-alpha)*st+alpha*uv);
                noise[i,j] = height;
            }
        }
        return noise;
    }
	
}
   