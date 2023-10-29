using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateImagesInventory : MonoBehaviour
{
    [SerializeField]
    private GameObject panelObject;
    [SerializeField]
    private Transform container;
    [SerializeField]
    GameData gameData;
    [SerializeField]
    private TextMeshProUGUI count;
    private LinkedList<GameObject> storeObjects;

    private void Awake()
    {
        if (storeObjects == null)
            storeObjects = new();
    }

    private void OnEnable()
    {
        Debug.Log(gameData.playerInventoryData.Count);
        CreateImages(gameData.playerInventoryData.Count);
        count.text = gameData.playerInventoryData.Count + "/" + gameData.capacityConstruction.playersMax; //Update the count text
    }

    private void OnDisable()
    {
        int size = storeObjects.Count;

        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                Destroy(storeObjects.First.Value);
                storeObjects.RemoveFirst();
            }
        }
    }

    private void CreateImages(int amount)
    {
        int counter = 0;
        int width = (int)container.GetComponent<GridLayoutGroup>().cellSize.x;
        int height = (int)container.GetComponent<GridLayoutGroup>().cellSize.y;

        while (amount > 0)
        {
            storeObjects.AddFirst(GameObject.Instantiate(panelObject, container));
            var resizedTexture = gameData.playerInventoryData[counter].image;
            //resizedTexture.Reinitialize(width, height);

            storeObjects.First.Value.GetComponent<Image>().sprite = Sprite.Create(resizedTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

            amount--;
            counter++;
        }
    }

}
