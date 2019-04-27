using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Boid
{

    public Food() {
        setFuel(50);
    }
    
    public override string getFaction()
    {
        return "Food";
    }

    public override void update(float deltaTime, List<Boid> neighbors, List<Boid> touching)
    {
        return;
    }

    public override void burnFuel(float distanceTraveled, float deltaTime)
    {
        //DO NOTHING
    }

    public override GeneticCode getGeneticCode()
    {
        return new GeneticCode(0);
    }

    public override float getScore(float simulationTime)
    {
        return 0;
    }

    public override string getState()
    {
        return "Existing";
    }
}
