using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : SteeringBehaviour
{
    public override Vector2 generateForce(Boid myself, List<Boid> neighbors)
    {
        return -myself.getVelocity();
    }

    public override string getName()
    {
        return "Stop";
    }
}
