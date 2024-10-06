using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC : MonoBehaviour
{
    public GameObject rockGO, treeGO, fenceGO;
    WFCNode rock, tree, fence;

   

    private void Awake()
    {
        BindType();
    }


    private void Start()
    {
        
    }


    void BindType()
    {
        //rock = new WFCNode(WFCType.rock,rockGO);
        //tree = new WFCNode(WFCType.tree,treeGO);
        //fence = new WFCNode(WFCType.fence,fenceGO);

    }
}
