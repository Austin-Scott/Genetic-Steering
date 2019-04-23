using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticCode {

    private List<float> data;

	public GeneticCode(int length)
    {
        data = new List<float>();
        for(int i=0;i<length;i++)
        {
            data.Add(Random.value);
        }
    }
    public int getLength()
    {
        return data.Count;
    }
    public GeneticCode(GeneticCode a, GeneticCode b, float mutationRate)
    {
        data = new List<float>();
        if(a.getLength()==b.getLength())
        {
            for(int i=0;i<a.getLength();i++)
            {
                if(Random.value<mutationRate)
                {
                    data.Add(Random.value);
                } else
                {
                    data.Add((a.getValue(i) + b.getValue(i)) / 2.0f);
                }
            }
        }
    }
    public GeneticCode(GeneticCode a, float mutationRate)
    {
        data = new List<float>();
        for (int i = 0; i < a.getLength(); i++)
        {
            if (Random.value < mutationRate)
            {
                data.Add(Random.value);
            }
            else
            {
                data.Add(a.getValue(i));
            }
        }
    }
    public float getValue(int index)
    {
        return data[index];
    }
}
