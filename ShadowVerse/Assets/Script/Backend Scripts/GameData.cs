using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOf
{
    Null = -1,
    TownHall = 0,
    WarriorsStore = 1
}

[System.Serializable]
public class BuildingData
{
    public int id;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public TypeOf building;
}

[System.Serializable]
public class Amount
{
    public int townHall = 1;
    public int warriorsStore = 1;
    public int playersMax = 25; //The maximum amounts of players one can hold

    public void AddValue(int by, TypeOf building)
    {
        switch (building)
        {
            case TypeOf.TownHall:
                if (townHall + by >= 0)
                    townHall += by;
                else
                    townHall = 0;
                break;
            case TypeOf.WarriorsStore:
                if (warriorsStore + by >= 0)
                    warriorsStore += by;
                else
                    warriorsStore = 0;
                break;
        }
    }
}

[System.Serializable]
public struct PlayerData
{
    public string name;
    public int hp;
    public int level;
    public int attack;
    public Texture2D image;
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
    public List<PlayerData> playerInventoryData;
    public Texture2D[] playersIcons;
    public CurrencyData currencyData;
    public bool canSaveLocal = true;
    public bool canBuild = true;
    public TypeOf currentBuilding = TypeOf.Null;
    public Amount capacityConstruction = new();
    [SerializeField]
    private BuildingType townHall;
    [SerializeField]
    private BuildingType warriorHall;
    [SerializeField]
    private LayerMask buildingLayerMask;
    [SerializeField]
    private LayerMask constructionLayerMask;
    [SerializeField]
    private LayerMask groundLayerMask;

    public BuildingType GetConfig(BuildingData data)
    {
        return data.building switch
        {
            TypeOf.TownHall => townHall,
            TypeOf.WarriorsStore => warriorHall,
            _ => null,
        };
    }

    public TypeOf GetType(int type)
    {
        return type switch
        {
            0 => TypeOf.TownHall,
            1 => TypeOf.WarriorsStore,
            _ => TypeOf.Null,
        };
    }

    public LayerMask GetLayerMaskBuilding() => buildingLayerMask;

    public LayerMask GetLayerMaskConstruction() => constructionLayerMask;

    public LayerMask GetLayerMaskGround() => groundLayerMask;

}
