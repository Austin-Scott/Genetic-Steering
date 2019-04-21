using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : SteeringBehaviour
{
    string faction;
    public Pursue(string faction)
    {
        this.faction = faction;
    }
    public override Vector2 generateForce(Boid myself, List<Boid> neighbors)
    {
        float minDistance = float.MaxValue;
        Boid closestBoid = null;
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].getFaction() == faction)
            {
                float distance = Vector2.Distance(myself.getPosition(), neighbors[i].getPosition());
                if (distance < minDistance)
                {
                    closestBoid = neighbors[i];
                    minDistance = distance;
                }
            }
        }
        if (closestBoid != null)
        {
            Vector2 desired = myself.getPosition()-(closestBoid.getPosition() + closestBoid.getVelocity());
            return desired - myself.getVelocity();
        }
        else
        {
            return new Vector2(0, 0);
        }
    }

    public override string getName()
    {
        return "Evade" + faction;
    }
}