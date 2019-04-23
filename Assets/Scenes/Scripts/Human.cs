using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanType { Child, Male, Female, PregnantFemale };
public enum HumanState { Idle, Hunted, Hungry };

public class Human : Boid
{
    private HumanType type;
    private float secondsAlive;
    private float secondsUntilMaturity;
    private float secondsUntilBirth;
    private GeneticCode code;
    private GeneticCode childCode;
    private HumanState state;

    public Human(GeneticCode code)
    {
        this.code = code;
        state = HumanState.Idle;
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
        setBehaviourWeight(name, code.getValue((17*3 * (int)type) + (17*(int)state) + 3 + offset));
    }
    private void updateWeights()
    {
        //Genetic code layout
        //  Each type consists of 3 blocks of 17 floats long (Idle, Hunted, Hungry)
        //[Child], [Male], [Female], [Pregnant Female]
        setDetectionRadius(code.getValue((17 * 3 * (int)type) + (17 * (int)state)));
        setMaxForce(code.getValue((17 * 3 * (int)type) + (17 * (int)state) + 1));
        setMaxVelocity(code.getValue((17 * 3 * (int)type + (17 * (int)state) + 2)));

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
        return type.ToString();
    }

    public override void update(float deltaTime, List<Boid> neighbors, List<Boid> touching)
    {
        HumanType initialType = type;
        HumanState initialState = state;
        bool seesZombie = false;
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].getFaction() == "Zombie")
            {
                state = HumanState.Hunted;
                seesZombie = true;
            }
        }
        if (seesZombie == false)
        {
            state = HumanState.Idle;
        }
        for (int i=0;i<touching.Count;i++)
        {
            if(touching[i].getFaction()=="Food")
            {
                addFuel(touching[i].getFuel());
                World.world.removeFood(touching[i].getID());
            } else if(type==HumanType.Female && touching[i].getFaction()=="Male")
            {
                //Become pregnant
                type = HumanType.PregnantFemale;
                childCode = new GeneticCode(touching[i].getGeneticCode(), code, 0.05f);
                secondsUntilBirth = 15f;
            }
        }

        if(type==HumanType.PregnantFemale)
        {
            secondsUntilBirth -= deltaTime;
            if(secondsUntilBirth<0)
            {
                //Give birth
                World.world.spawnChild(getID(), getPosition(), childCode);
                type = HumanType.Female;
            }
        } else if(type==HumanType.Child)
        {
            secondsUntilMaturity -= deltaTime;
            if(secondsUntilMaturity<0)
            {
                //Become an adult
                if(Random.value<0.5f)
                {
                    type = HumanType.Male;
                } else
                {
                    type = HumanType.Female;
                }
            }
        }

        secondsAlive += deltaTime;

        if(initialState!=state || initialType!=type)
        {
            updateWeights();
        }
    }

    public override GeneticCode getGeneticCode()
    {
        return code;
    }
}
