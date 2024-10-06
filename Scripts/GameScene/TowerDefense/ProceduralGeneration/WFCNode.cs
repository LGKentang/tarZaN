using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCNode : MonoBehaviour
{
    public WFCType type;
    private GameObject model;

    // Dictionary to store associations between types and GameObjects
    private static Dictionary<WFCType, GameObject> typeToModelDictionary;

    public WFCNode(WFCType type)
    {
        this.type = type;
        // Get the associated GameObject from the dictionary based on the type
        if (typeToModelDictionary.TryGetValue(type, out GameObject associatedModel))
        {
            model = associatedModel;
        }
        else
        {
            Debug.LogError("No GameObject associated with this type: " + type);
        }
    }

    public GameObject Model
    {
        get { return model; }
    }

 
}
