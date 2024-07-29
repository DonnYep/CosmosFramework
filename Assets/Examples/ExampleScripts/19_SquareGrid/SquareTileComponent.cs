using UnityEngine;
public class SquareTileComponent : MonoBehaviour
{
    GameObject squardTileNormal;
    GameObject squardTileHighlight;
    public void SquareTileHighlight(bool state)
    {
        squardTileNormal.SetActive(!state);
        squardTileHighlight.SetActive(state);
    }
    private void Awake()
    {
        squardTileHighlight = transform.Find("SquardTileHighlight").gameObject;
        squardTileNormal = transform.Find("SquardTileNormal").gameObject;
        squardTileHighlight.SetActive(false);
    }
}
