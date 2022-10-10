using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Building
{
    TownHall = 0
}

[System.Serializable]
public class BuildingData
{
    public int id;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Building building;
}

public struct CurrencyData
{
    public int food;
    public int wood;
    public int stone;
    public int iron;
    public int gold;
}

[CreateAssetMenu()]
public class GameData : ScriptableObject
{
    public List<BuildingData> data;
    public CurrencyData currencyData;
    public bool canSaveLocal = true;
    public bool canBuild = true;
    [SerializeField]
    private BuildingType townHall;
    [SerializeField]
    private LayerMask buildingLayerMask;

    public BuildingType GetConfig(BuildingData data)
    {
        switch (data.building)
        {
            case Building.TownHall:
                return townHall;

            default:
                return null;
        }
    }

    public LayerMask GetLayerMaskBuilding() => buildingLayerMask;

}
