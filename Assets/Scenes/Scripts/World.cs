using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

    public static World world;
    public static void createNewWorld()
    {
        world = new World();
    }

    List<Boid> boids;

    public List<Boid> getBoidsInCircle(Vector2 center, float radius)
    {
        List<Boid> result = new List<Boid>();
        for(int i=0;i<boids.Count;i++)
        {
            if(Vector2.Distance(boids[i].getPosition(), center)<radius)
            {
                result.Add(boids[i]);
            }
        }
        return result;
    }
    public void removeFood(uint id)
    {

    }
    public void killZombie(uint id)
    {

    }
    public void killHuman(uint id)
    {

    }
    public void infectHuman(uint humanID, uint zombieID)
    {

    }
}
