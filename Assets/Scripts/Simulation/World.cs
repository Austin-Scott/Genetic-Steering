﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public static World world=null;

    public GameObject HumanPrefab;
    public GameObject ZombiePrefab;
    public GameObject FoodPrefab;

    public float worldWidth = 40;
    public float worldHeight = 40;

    private int radiusVertexCount = 10;

    List<Boid> boids;

    public World()
    {
        boids = new List<Boid>();
    }

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
        bool addLR = false;
        if(toAdd.getFaction()=="Child" || toAdd.getFaction() == "Male" || toAdd.getFaction() == "Female" || toAdd.getFaction() == "PregnantFemale")
        {
            toAdd.setObj(Instantiate(HumanPrefab, toAdd.getPosition(), Quaternion.identity));
            addLR = true;
        } else if(toAdd.getFaction()=="Zombie")
        {
            toAdd.setObj(Instantiate(ZombiePrefab, toAdd.getPosition(), Quaternion.identity));
            addLR = true;
        } else
        {
            toAdd.setObj(Instantiate(FoodPrefab, toAdd.getPosition(), Quaternion.identity));
        }

        if (addLR)
        {
            LineRenderer lr = toAdd.getObj().AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Additive"));
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.positionCount = radiusVertexCount;
        }

        boids.Add(toAdd);
    }
    private void drawDetectionRadius(Boid boid)
    {
        Vector2 pos = boid.getPosition();
        LineRenderer lr = boid.getObj().GetComponent<LineRenderer>();
        if(lr==null)
        {
            return;
        }
        float theta = 0f;
        float radius = boid.getDetectionRadius();
        float thetaStep = (2f * Mathf.PI) / (radiusVertexCount-1);
        for(int i=0;i<radiusVertexCount;i++)
        {
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            x += pos.x;
            y += pos.y;
            lr.SetPosition(i, new Vector3(x, y));
            theta += thetaStep;
        }
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

    public void Start()
    {
        world = this;
        Camera main = Camera.main;
        Debug.Log(getUpperBounds());
        main.orthographicSize = getUpperBounds();

        for(int i=0;i<10;i++)
        {
            Zombie zom = new Zombie(new GeneticCode(17 * 3));
            zom.setPosition(new Vector2(0, 0));
            addBoid(zom);
        }
    }
    public void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < boids.Count; i++)
        {
            List<Boid> neighbors = new List<Boid>();
            List<Boid> touching = new List<Boid>();
            float detectionRadius = boids[i].getDetectionRadius();
            for (int j = 0; j < boids.Count; j++)
            {
                if (boids[j].getID() != boids[i].getID() && Vector2.Distance(boids[i].getPosition(), boids[j].getPosition()) < detectionRadius)
                {
                    neighbors.Add(boids[j]);
                }
            }
            for (int j = 0; j < neighbors.Count; j++)
            {
                if (Vector2.Distance(boids[i].getPosition(), neighbors[j].getPosition()) < 0.1)
                {
                    touching.Add(neighbors[j]);
                }
            }


            boids[i].update(deltaTime, neighbors, touching);
            boids[i].move(deltaTime, neighbors);

            Vector2 pos = boids[i].getPosition();
            pos = constrainPosition(pos);
            boids[i].setPosition(pos);

            drawDetectionRadius(boids[i]);
        }
    }
    private Vector2 constrainPosition(Vector2 pos)
    {
        if(pos.y>getUpperBounds())
        {
            pos.y = getLowerBounds();
        } else if(pos.y<getLowerBounds())
        {
            pos.y = getUpperBounds();
        }
        if(pos.x>getRightMostBounds())
        {
            pos.x = getLeftMostBounds();
        } else if(pos.x<getLeftMostBounds())
        {
            pos.x = getRightMostBounds();
        }
        return pos;
    }
    private float getUpperBounds()
    {
        return worldHeight / 2f;
    }
    private float getLowerBounds()
    {
        return -getUpperBounds();
    }
    private float getRightMostBounds()
    {
        return worldWidth / 2f;
    }
    private float getLeftMostBounds()
    {
        return -getRightMostBounds();
    }
}
