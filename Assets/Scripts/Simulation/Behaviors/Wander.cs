using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : SteeringBehaviour
{
    private float theta=0.0f;
    private float circleDistance = 2.0f;
    private float circleRadius = 1f;
    private float maxThetaChange = 0.5f;

    public override Vector2 generateForce(Boid myself, List<Boid> neighbors)
    {
        Vector2 directionFacing = new Vector2(myself.getVelocity().x, myself.getVelocity().y);
        directionFacing.Normalize();
        float scale = (Random.value - 0.5f)*2f;
        theta += maxThetaChange * scale;
        Vector2 circle = (new Vector2(Mathf.Cos(theta), Mathf.Sin(theta))) * circleRadius;
        Vector2 desired = (directionFacing * circleDistance) + circle;
        Vector2 result = desired - myself.getVelocity();
        return result;
    }

    public override string getName()
    {
        return "Wander";
    }
}
