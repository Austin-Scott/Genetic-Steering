using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenePool : System.IComparable<GenePool>
{
    GeneticCode code;
    float score;
    public GenePool(GeneticCode code, float score)
    {
        this.code = code;
        this.score = score;
    }

    public int CompareTo(GenePool other)
    {
        if(score>other.getScore())
        {
            return 1;
        } else if(score<other.getScore())
        {
            return -1;
        }

        return 0;
    }

    public GeneticCode getCode()
    {
        return code;
    }
    public float getScore()
    {
        return score;
    }
}

public class World : MonoBehaviour {

    public static World world=null;

    public GameObject HumanPrefab;
    public GameObject ZombiePrefab;
    public GameObject FoodPrefab;

    public float worldWidth = 40;
    public float worldHeight = 40;

    public int startingHumans = 20;
    public int startingZombies = 1;
    public int startingFood = 10;
    public int maxFood = 100;

    public float timeBetweenFoodDrops = 1f;
    private float timeUntilNextFoodDrop = 0f;

    private float generationTime = 0;
    private int generationNum = 0;

    private int radiusVertexCount = 10;

    private int humansAlive;
    private int zombiesAlive;
    private int foodPieces;

    private int humanVictories = 0;
    private int zombieVictories = 0;

    private bool roundIsOver = false;

    private bool showDetectionRadii=true;

    private List<GenePool> deadHumans;
    private List<GenePool> deadZombies;

    private List<uint> boidsToRemove;
    private List<Boid> boidsToAdd;

    List<Boid> boids;

    private int boidCount = 0;

    private float simulationSpeed = 1f;

    public World()
    {
        boids = new List<Boid>();
        deadHumans = new List<GenePool>();
        deadZombies = new List<GenePool>();
        boidsToRemove = new List<uint>();
        boidsToAdd = new List<Boid>();
        roundIsOver = false;
    }
    public void setSimulationSpeed(float speed)
    {
        simulationSpeed = speed;
    }
    private bool isBoidGoingToBeRemoved(uint id)
    {
        for(int i=0;i<boidsToRemove.Count;i++)
        {
            if(boidsToRemove[i]==id)
            {
                return true;
            }
        }
        return false;
    }
    public void setShowDetectionRadii(UnityEngine.UI.Toggle value)
    {
        showDetectionRadii = value.isOn;
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
        if(!isBoidGoingToBeRemoved(id))
        {
            boidsToRemove.Add(id);
            foodPieces--;
        }
    }
    public void spawnChild(uint motherID, Vector2 location, GeneticCode code)
    {
        Boid mother = getBoid(motherID);
        float fuel = mother.getFuel();
        mother.setFuel(fuel * 0.75f);
        Boid child = new Human(code);
        child.setPosition(location);
        child.setFuel(fuel * 0.5f);
        boidsToAdd.Add(child);

        humansAlive++;
    }
    public void killZombie(uint id)
    {
        if(!isBoidGoingToBeRemoved(id))
        {
            Boid boid = getBoid(id);
            deadZombies.Add(new GenePool(boid.getGeneticCode(), boid.getScore(generationTime)));
            boidsToRemove.Add(id);

            zombiesAlive--;
        }
    }
    public void killHuman(uint id)
    {
        if(!isBoidGoingToBeRemoved(id))
        {
            Boid boid = getBoid(id);
            deadHumans.Add(new GenePool(boid.getGeneticCode(), boid.getScore(generationTime)));
            boidsToRemove.Add(id);

            humansAlive--;
        }
    }
    private void createAndRemoveBoids()
    {
        for (int i = 0; i < boidsToAdd.Count; i++)
        {
            addBoid(boidsToAdd[i]);
        }
        boidsToAdd.Clear();
        for (int i=0;i<boidsToRemove.Count;i++)
        {
            uint idToRemove = boidsToRemove[i];
            removeBoid(boidsToRemove[i]);
        }
        boidsToRemove.Clear();
    }
    private int getTopID(int count)
    {
        int result = Mathf.FloorToInt((-1f * Mathf.Sqrt(1f - Mathf.Pow(Random.value, 2f))) + 1f);
        if(result>=count)
        {
            result = count - 1;
        } else if(result<0)
        {
            result = 0;
        }
        return result;
    }
    private List<GenePool> getTopGenes(List<GenePool> from, int n)
    {
        if(from.Count==0)
        {
            return null;
        }
        from.Sort();
        List<GenePool> result = new List<GenePool>();
        while(n>0)
        {
            result.Add(from[getTopID(from.Count)]);
            n--;
        }
        return result;
    }
    private void nextGeneration()
    {
        for(int i=0;i<boids.Count;i++)
        {
            string f = boids[i].getFaction();
            if (f=="Child" || f=="Male" || f=="Female" || f=="PregnantFemale")
            {
                killHuman(boids[i].getID());
            } else if(f=="Zombie")
            {
                killZombie(boids[i].getID());
            } else
            {
                removeFood(boids[i].getID());
            }
        }
        createAndRemoveBoids();
        boids.Clear();
        Boid.resetIDs();

        List<GenePool> topHumans = getTopGenes(deadHumans, startingHumans);
        List<GenePool> topZombies = getTopGenes(deadZombies, startingZombies);

        deadHumans.Clear();
        deadZombies.Clear();

        for (int i = 0; i < startingZombies; i++)
        {
            Zombie zom = new Zombie(new GeneticCode(topZombies[i].getCode(), 0.05f));
            zom.setPosition(getRandomPosition());
            addBoid(zom);
        }

        for (int i = 0; i < startingFood; i++)
        {
            spawnFood();
        }

        for (int i = 0; i < startingHumans; i++)
        {
            Human human = new Human(new GeneticCode(topHumans[i].getCode(), 0.05f));
            human.setPosition(getRandomPosition());
            addBoid(human);
        }

        topHumans.Clear();
        topZombies.Clear();

        humansAlive = startingHumans;
        zombiesAlive = startingZombies;
        foodPieces = startingFood;
        generationTime = 0;
        roundIsOver = false;
        generationNum++;
    }
    public void updateHumanState(uint id)
    {
        Boid boid = getBoid(id);
        if(boid==null)
        {
            return;
        }
        string f = boid.getFaction();
        SpriteRenderer sr = boid.getObj().GetComponent<SpriteRenderer>();
        if(f=="Child")
        {
            sr.color = new Color(1, 1, 0);
        } else if(f=="Male")
        {
            sr.color = new Color(1, 0, 0);
        } else if(f=="Female")
        {
            sr.color = new Color(0.95f, 0.05f, 0.56f);
        } else if(f=="PregnantFemale")
        {
            sr.color = new Color(0.5f, 0, 0.5f);
        }
    }
    public void infectHuman(uint humanID, uint zombieID)
    {
        if(!isBoidGoingToBeRemoved(humanID))
        {
            Boid human = getBoid(humanID);
            Boid zombie = getBoid(zombieID);
            float humanFuel = human.getFuel();
            float zombieFuel = zombie.getFuel();
            float fuel = humanFuel;
            Boid newZombie = new Zombie(new GeneticCode(zombie.getGeneticCode(), 0.05f));
            newZombie.setPosition(zombie.getPosition());
            newZombie.setFuel(fuel);
            zombie.setFuel(zombie.getFuel()+fuel);
            killHuman(humanID);
            boidsToAdd.Add(newZombie);

            zombiesAlive++;
        }
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

        boidCount++;
    }
    private void spawnFood()
    {
        if(foodPieces<maxFood)
        {
            Food food = new Food();
            food.setPosition(getRandomPosition());
            addBoid(food);
            foodPieces++;
        }
    }
    private void drawDetectionRadius(Boid boid)
    {
        Vector2 pos = boid.getPosition();
        LineRenderer lr = boid.getObj().GetComponent<LineRenderer>();
        if(lr==null)
        {
            return;
        }
        if(!showDetectionRadii)
        {
            lr.enabled = false;
            return;
        }
        lr.enabled = true;
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
    private void removeBoid(uint ID)
    {
        for(int i=0;i<boids.Count;i++)
        {
            if(boids[i].getID()==ID)
            {
                int countBefore = boids.Count;
                boids[i].destroyGameObject();
                boids.RemoveAt(i);
                boidCount--;
                return;
            }
        }
    }

    public void Start()
    {
        world = this;
        Camera main = Camera.main;
        main.orthographicSize = getUpperBounds();

        for(int i=0;i<startingZombies;i++)
        {
            Zombie zom = new Zombie(new GeneticCode(17 * 3));
            zom.setPosition(getRandomPosition());
            addBoid(zom);
        }

        for(int i=0;i<startingFood;i++)
        {
            spawnFood();
        }

        for(int i=0;i<startingHumans;i++)
        {
            Human human = new Human(new GeneticCode(17 * 3 * 4));
            human.setPosition(getRandomPosition());
            addBoid(human);
        }

        humansAlive = startingHumans;
        zombiesAlive = startingZombies;
        foodPieces = startingFood;
    }
    public void Update()
    {
        float deltaTime = simulationSpeed*Time.deltaTime;
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
                if (Vector2.Distance(boids[i].getPosition(), neighbors[j].getPosition()) < 0.5)
                {
                    touching.Add(neighbors[j]);
                }
            }

            boids[i].move(deltaTime, neighbors);

            string faction = boids[i].getFaction();
            boids[i].update(deltaTime, neighbors, touching);

            Vector2 pos = boids[i].getPosition();
            pos = constrainPosition(pos);
            boids[i].setPosition(pos);

            drawDetectionRadius(boids[i]);
        }

        createAndRemoveBoids();

        generationTime += deltaTime;

        timeUntilNextFoodDrop -= deltaTime;
        if(timeUntilNextFoodDrop<0)
        {
            timeUntilNextFoodDrop = timeBetweenFoodDrops;
            spawnFood();
        }

        if(!roundIsOver && (humansAlive==0 || zombiesAlive==0))
        {
            endGeneration();
        }
    }
    public int getHumanVictories()
    {
        return humanVictories;
    }
    public int getZombieVictories()
    {
        return zombieVictories;
    }
    public int getGeneration()
    {
        return generationNum;
    }
    public void endGeneration()
    {
        if (humansAlive > zombiesAlive)
        {
            humanVictories++;
        }
        else
        {
            zombieVictories++;
        }
        roundIsOver = true;
        Debug.Log("Round " + (generationNum + 1) + " over. Humans: " + humanVictories + " Zombies: " + zombieVictories);
        Invoke("nextGeneration", 2.5f);
    } 
    private Vector2 getRandomPosition()
    {
        float randX = (Random.value - 0.5f) * 2f;
        float randY = (Random.value - 0.5f) * 2f;
        float x = randX * getRightMostBounds();
        float y = randY * getUpperBounds();
        return new Vector2(x, y);
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
