using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBuildPress : MonoBehaviour
{
    private Button button;
    [SerializeField]
    private GameData gData;
    [SerializeField]
    private Build buildScript;
    private bool canTurnOn = true;

    private void Awake()
    {
        button = GetComponent<Button>();
        int buttonName = System.Convert.ToInt32(button.gameObject.name.Substring(0, 1)); //Assumes the names are correct

        Check();

        button.onClick.AddListener(() => 
        {
            gData.currentBuilding = gData.GetType(buttonName);
            gData.canBuild = true;
            buildScript.enabled = true;
        });
    }

    private void LateUpdate()
    {
        Check();

        if (canTurnOn)
        {
            if (gData.canBuild)
                button.interactable = false;
            else
                button.interactable = true;
        }
    }

    private void Check()
    {
        int buttonName = System.Convert.ToInt32(button.gameObject.name.Substring(0, 1));

        if (gData.GetType(buttonName) == TypeOf.TownHall && gData.capacityConstruction.townHall == 0)
        {
            canTurnOn = false;
            button.interactable = false;
        }
        else if (gData.GetType(buttonName) == TypeOf.WarriorsStore && gData.capacityConstruction.warriorsStore == 0)
        {
            canTurnOn = false;
            button.interactable = false;
        }
    }

}
