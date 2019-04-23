using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boid {

    private GameObject obj=null;
    public void setObj(GameObject obj)
    {
        this.obj = obj;
    }
    public GameObject getObj()
    {
        return obj;
    }

    private static uint maxID=0;
    private static uint getNewID()
    {
        maxID++;
        return maxID;
    }
    public static void resetIDs()
    {
        maxID = 0;
    }

    private class SteeringWeight
    {
        public float weight;
        public string behaviourName;
    }

    private uint id;

    public uint getID()
    {
        return id;
    }

    private Vector2 position;
    private Vector2 netForces;
    private Vector2 velocity;

    private float detectionRadius;
    private float maxVelocity;
    private float maxForce;

    private float fuel;

    public float getFuel()
    {
        return fuel;
    }
    public void setFuel(float value)
    {
        fuel = value;
    }
    protected void addFuel(float amount)
    {
        fuel += amount;
    }

    List<SteeringBehaviour> behaviours;
    List<SteeringWeight> weights;

    public Boid()
    {
        id = getNewID();
        weights = new List<SteeringWeight>();
        behaviours = new List<SteeringBehaviour>();
    }

    public Vector2 getPosition()
    {
        return position;
    }
    public void setPosition(Vector2 position)
    {
        this.position = position;
    }
    public Vector2 getVelocity()
    {
        return velocity;
    }
    public void setDetectionRadius(float radius)
    {
        detectionRadius = radius;
    }
    public void setMaxVelocity(float velocity)
    {
        maxVelocity = 10*velocity;
    }
    public void setMaxForce(float force)
    {
        maxForce = 10*force;
    }
    public float getDetectionRadius()
    {
        return 10*detectionRadius;
    }

    public void addSteeringBehaviour(SteeringBehaviour behaviour, float weight)
    {
        weights.Add(new SteeringWeight(){ weight=weight, behaviourName=behaviour.getName() });
        behaviours.Add(behaviour);
    }
    public void setBehaviourWeight(string name, float weight)
    {
        for(int i=0;i<weights.Count;i++)
        {
            if(weights[i].behaviourName==name)
            {
                weights[i].weight = weight;
                break;
            }
        }
    }
    private float getBehaviourWeight(string name)
    {
        for (int i = 0; i < weights.Count; i++)
        {
            if (weights[i].behaviourName == name)
            {
                return weights[i].weight;
            }
        }
        return 0.0f;
    }

    public abstract void update(float deltaTime, List<Boid> neighbors, List<Boid> touching);
    public abstract string getFaction();

    public abstract GeneticCode getGeneticCode();

    public virtual void burnFuel(float distanceTraveled, float deltaTime)
    {
        fuel -= 0.1f * distanceTraveled + 1.0f * deltaTime + 0.5f * detectionRadius * deltaTime + 0.25f * maxForce * deltaTime;
    }

    public void move(float deltaTime, List<Boid> neighbors)
    {
        netForces.Set(0, 0);
        for(int i=0;i<behaviours.Count;i++)
        {
            netForces += behaviours[i].generateForce(this, neighbors) * getBehaviourWeight(behaviours[i].getName());
        }
        if(netForces.magnitude > maxForce)
        {
            netForces.Normalize();
            netForces *= maxForce;
        }
        //Since A=F/M and M=1, A=F
        velocity += netForces * deltaTime;
        if(velocity.magnitude>maxVelocity)
        {
            velocity.Normalize();
            velocity *= maxVelocity;
        }
        Vector2 oldPosition = new Vector2(position.x, position.y);
        position += velocity * deltaTime;

        obj.transform.SetPositionAndRotation(position, Quaternion.identity);

        burnFuel(Vector2.Distance(oldPosition, position), deltaTime);
    }
}
