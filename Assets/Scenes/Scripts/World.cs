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
        removeBoid(id);
    }
    public void spawnChild(uint motherID, Vector2 location, GeneticCode code)
    {
        Boid mother = getBoid(motherID);
        float fuel = mother.getFuel();
        mother.setFuel(fuel / 2f);
        Boid child = new Human(code);
        child.setFuel(fuel / 2f);
        addBoid(child);
    }
    public void killZombie(uint id)
    {
        removeBoid(id);
    }
    public void killHuman(uint id)
    {
        removeBoid(id);
    }
    public void infectHuman(uint humanID, uint zombieID)
    {
        Boid human = getBoid(humanID);
        Boid zombie = getBoid(zombieID);
        float humanFuel = human.getFuel();
        float zombieFuel = zombie.getFuel();
        float fuel = (humanFuel + zombieFuel) / 2f;
        Boid newZombie = new Zombie(new GeneticCode(zombie.getGeneticCode(), 0.05f));
        newZombie.setFuel(fuel);
        zombie.setFuel(fuel);
        killHuman(humanID);
        addBoid(newZombie);
    }
    private void addBoid(Boid toAdd)
    {
        boids.Add(toAdd);
    }
    private Boid getBoid(uint ID)
    {
        for(int i=0;i<boids.Count;i++)
        {
            if(boids[i].getID()==ID)
            {
                return boids[i];
            }
        }
        return null;
    }
    private Boid removeBoid(uint ID)
    {
        for(int i=0;i<boids.Count;i++)
        {
            if(boids[i].getID()==ID)
            {
                Boid boidToRemove = boids[i];
                boids.RemoveAt(i);
                return boidToRemove;
            }
        }
        return null;
    }

    public void update()
    {
        float deltaTime = Time.deltaTime;
        for(int i=0;i<boids.Count;i++)
        {
            List<Boid> neighbors = new List<Boid>();
            List<Boid> touching = new List<Boid>();
            float detectionRadius = boids[i].getDetectionRadius();
            for(int j=0;j<boids.Count;j++)
            {
                if(boids[j].getID()!=boids[i].getID() && Vector2.Distance(boids[i].getPosition(), boids[j].getPosition())<detectionRadius)
                {
                    neighbors.Add(boids[j]);
                }
            }
            for(int j=0;j<neighbors.Count;j++)
            {
                if(Vector2.Distance(boids[i].getPosition(), neighbors[j].getPosition())<0.1)
                {
                    touching.Add(neighbors[j]);
                }
            }


            boids[i].update(deltaTime, neighbors, touching);
            boids[i].move(deltaTime, neighbors);
        }
    }
}
