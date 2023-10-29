using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGuiForBuildings : MonoBehaviour
{
    [SerializeField]
    private GameObject menuInventory;
    [SerializeField]
    private GameData gameData;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, gameData.GetLayerMaskBuilding()))
            {
                //Find the building that we want to open the inventory
                //Note: add a button in the ui after the building exists
                if (hit.transform.name.Contains("1"))
                {
                    menuInventory.SetActive(true);
                }
            }
        }
    }

}
