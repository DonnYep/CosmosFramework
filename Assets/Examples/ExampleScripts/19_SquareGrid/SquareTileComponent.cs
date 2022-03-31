using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class SquareTileComponent : MonoBehaviour
{
    GameObject squardTileNormal;
    GameObject squardTileHighlight;
    public void SquareTileHighlight(bool state)
    {
        squardTileNormal.SetActiveOptimize(!state);
        squardTileHighlight.SetActiveOptimize(state);
    }
    private void Awake()
    {
        squardTileHighlight = transform.Find("SquardTileHighlight").gameObject;
        squardTileNormal = transform.Find("SquardTileNormal").gameObject;
        squardTileHighlight.SetActive(false);
    }
}
