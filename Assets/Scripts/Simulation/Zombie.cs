using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieState { Idle, Hunting, Hungry };

public class Zombie : Boid
{
    private float fuel;
    private int kills;
    private GeneticCode code;
    private ZombieState state;

    public Zombie(GeneticCode code)
    {
        this.code = code;
        setFuel(200);
        addBehaviours();
        updateWeights();
    }
    private void addBehaviours()
    {
        addSteeringBehaviour(new Wander(), 1.0f);
        addSteeringBehaviour(new Stop(), 1.0f);
        addSteeringBehaviour(new Pursue("Food"), 1.0f);
        addSteeringBehaviour(new Evade("Food"), 1.0f);
        addSteeringBehaviour(new Pursue("Child"), 1.0f);
        addSteeringBehaviour(new Evade("Child"), 1.0f);
        addSteeringBehaviour(new Pursue("Male"), 1.0f);
        addSteeringBehaviour(new Evade("Male"), 1.0f);
        addSteeringBehaviour(new Pursue("Female"), 1.0f);
        addSteeringBehaviour(new Evade("Female"), 1.0f);
        addSteeringBehaviour(new Pursue("PregnantFemale"), 1.0f);
        addSteeringBehaviour(new Evade("PregnantFemale"), 1.0f);
        addSteeringBehaviour(new Pursue("Zombie"), 1.0f);
        addSteeringBehaviour(new Evade("Zombie"), 1.0f);
    }
    private void updateWeight(string name, int offset)
    {
        setBehaviourWeight(name, code.getValue((17 * (int)state) + 3 + offset));
    }
    private void updateWeights()
    {
        setDetectionRadius(code.getValue((17 * (int)state)));
        setMaxForce(code.getValue((17 * (int)state + 1)));
        setMaxVelocity(code.getValue((17 * (int)state + 2)));

        updateWeight("Wander", 0);
        updateWeight("Stop", 1);
        updateWeight("PursueFood", 2);
        updateWeight("EvadeFood", 3);
        updateWeight("PursueChild", 4);
        updateWeight("EvadeChild", 5);
        updateWeight("PursueMale", 6);
        updateWeight("EvadeMale", 7);
        updateWeight("PursueFemale", 8);
        updateWeight("EvadeFemale", 9);
        updateWeight("PursuePregnantFemale", 10);
        updateWeight("EvadePregnantFemale", 11);
        updateWeight("PursueZombie", 12);
        updateWeight("EvadeZombie", 13);
    }
    public override string getFaction()
    {
        return "Zombie";
    }

    public override void update(float deltaTime, List<Boid> neighbors, List<Boid> touching)
    {
        ZombieState initialState = state;

        bool seesHuman = false;
        for(int i=0;i<neighbors.Count;i++)
        {
            string f = neighbors[i].getFaction();
            if(f=="Child" || f=="Male" || f=="Female" || f=="PregnantFemale")
            {
                seesHuman = true;
                state = ZombieState.Hunting;
            }
        }
        if(seesHuman==false)
        {
            state = ZombieState.Idle;
        }

        if(getFuel()<0)
        {
            World.world.killZombie(getID());
            return;
        } else if(getFuel()<50)
        {
            state = ZombieState.Hungry;
        }
        
        for(int i=0;i<touching.Count;i++)
        {
            string f = neighbors[i].getFaction();
            if (f == "Child" || f == "Male" || f == "Female" || f == "PregnantFemale")
            {
                kills++;
                World.world.infectHuman(neighbors[i].getID(), getID());
                return;
            } else if(f=="Food")
            {
                addFuel(-neighbors[i].getFuel());
                World.world.removeFood(neighbors[i].getID());
            }
        }

        if(state!=initialState)
        {
            updateWeights();
        }
        return;
    }

    public override void burnFuel(float distanceTraveled, float deltaTime)
    {
        setFuel(getFuel() - ((1.0f * distanceTraveled) + (0.25f * getDetectionRadius() * deltaTime)));
    }

    public override GeneticCode getGeneticCode()
    {
        return code;
    }

    public override float getScore(float simulationTime)
    {
        return (10*kills)+getFuel();
    }
}
