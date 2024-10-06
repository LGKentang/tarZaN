using System;
using System.Collections.Generic;
using System.Linq;

public class AdjacencyRules
{
    public Dictionary<WFCType, Dictionary<WFCType, int>> weight = new Dictionary<WFCType, Dictionary<WFCType, int>>();
    public Dictionary<WFCType, int[]> cumulativeProbabilities = new Dictionary<WFCType, int[]>();
    public Dictionary<WFCType, List<WFCType>> adjacencyInvalidRule = new Dictionary<WFCType, List<WFCType>>();

    public AdjacencyRules()
    {

        adjacencyInvalidRule[WFCType.none] = new List<WFCType> { WFCType.path, WFCType.fence }; 
        adjacencyInvalidRule[WFCType.fence] = new List<WFCType> { WFCType.path, WFCType.fence }; 
        adjacencyInvalidRule[WFCType.rock] = new List<WFCType> { WFCType.path, WFCType.bush, WFCType.fence }; 
        adjacencyInvalidRule[WFCType.tree] = new List<WFCType> { WFCType.path, WFCType.fence };
        adjacencyInvalidRule[WFCType.path] = new List<WFCType> { WFCType.none, WFCType.rock, WFCType.tree , WFCType.path}; 
        adjacencyInvalidRule[WFCType.bush] = new List<WFCType> { WFCType.path, WFCType.tree, WFCType.fence };

        weight[WFCType.none] = new Dictionary<WFCType, int>
        {
            { WFCType.none, 76 },
            { WFCType.bush, 10 },
            { WFCType.tree, 5 },
            { WFCType.rock, 5 },
            { WFCType.fence, 3 },
            { WFCType.path, 3 },  
        };

        weight[WFCType.bush] = new Dictionary<WFCType, int>
                {
                    { WFCType.none, 20 },
                    { WFCType.bush, 50 },
                    { WFCType.tree, 10 },
                    { WFCType.rock, 10 },
                    { WFCType.fence, 3 },
                    { WFCType.path, 2 }, 
                };

        weight[WFCType.tree] = new Dictionary<WFCType, int>
                {
                    { WFCType.none, 30 },
                    { WFCType.bush, 10 },
                    { WFCType.tree, 40 },
                    { WFCType.rock, 10 },
                    { WFCType.fence, 5 },
                    { WFCType.path, 5 },
                };

        weight[WFCType.rock] = new Dictionary<WFCType, int>
                {
                    { WFCType.none, 15 },
                    { WFCType.bush, 10 },
                    { WFCType.tree, 10 },
                    { WFCType.rock, 50 },
                    { WFCType.fence, 2 },
                    { WFCType.path, 3 }, 
                };

        weight[WFCType.fence] = new Dictionary<WFCType, int>
                {
                    { WFCType.none, 70 },
                    { WFCType.bush, 10 },
                    { WFCType.tree, 5 },
                    { WFCType.rock, 5 },
                    { WFCType.fence, 5 },  
                    { WFCType.path, 5 },    
                };

        weight[WFCType.path] = new Dictionary<WFCType, int>
                {
                    { WFCType.none, 2 },
                    { WFCType.bush, 2 },   
                    { WFCType.tree, 2 },   
                    { WFCType.rock, 2 },   
                    { WFCType.fence, 90 }, 
                    { WFCType.path, 2 },
                };

        foreach (var type in weight.Keys)
        {
            cumulativeProbabilities[type] = ComputeCumulativeProbabilities(weight[type]);
        }
    }

    private int[] ComputeCumulativeProbabilities(Dictionary<WFCType, int> probabilities)
    {
        int[] cumulative = new int[probabilities.Count];
        int i = 0;
        int cumulativeValue = 0;

        foreach (var kvp in probabilities)
        {
            cumulativeValue += kvp.Value;
            cumulative[i] = cumulativeValue;
            i++;
        }

        return cumulative;
    }

    public WFCType GetRandomObjectTypeBasedOnProbabilities(WFCType type)
    {
        Dictionary<WFCType, int> probabilities = weight[type];
        int[] cumulative = cumulativeProbabilities[type];

        int randomValue = UnityEngine.Random.Range(0, cumulative[cumulative.Length - 1]);

        for (int i = 0; i < cumulative.Length; i++)
        {
            if (randomValue < cumulative[i])
            {
                return probabilities.ElementAt(i).Key; 
            }
        }

       
        return WFCType.none; 
    }
}
