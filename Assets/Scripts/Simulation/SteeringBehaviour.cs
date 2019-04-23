using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour {

    public abstract Vector2 generateForce(Boid myself, List<Boid> neighbors);

    public abstract string getName();
	
}
