using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    [SerializeField]
    private GameData gData;

    private void Start()
    {
        if (!gData.canBuild)
            this.enabled = false;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            bool tB = false;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                gData.data.Add(new() { building = Building.TownHall, position = hit.point });
                gData.data[^1].id = gData.data.Count - 1;

                tB = CanBuild(gData.GetConfig(gData.data[^1]), hit.point);

                if (tB)
                {
                    GameObject ob = Instantiate(gData.GetConfig(gData.data[^1]).prefab.gameObject, hit.point, Quaternion.identity);
                    gData.data[^1].rotation = ob.transform.rotation;
                    gData.data[^1].scale = ob.transform.localScale;

                    var json = JsonUtility.ToJson(gData.data[^1]);
                    //Debug.Log(json);
                }
            }
        }

#endif

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Moved)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));

                bool tB = false;
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    gData.data.Add(new() { building = Building.TownHall, position = hit.point });
                    gData.data[^1].id = gData.data.Count - 1;

                    tB = CanBuild(gData.GetConfig(gData.data[^1]), hit.point);

                    if (tB)
                    {
                        GameObject ob = Instantiate(gData.GetConfig(gData.data[^1]).prefab.gameObject, hit.point, Quaternion.identity);
                        gData.data[^1].rotation = ob.transform.rotation;
                        gData.data[^1].scale = ob.transform.localScale;

                        var json = JsonUtility.ToJson(gData.data[^1]);
                        //Debug.Log(json);

                        GlobalUtil.Instance.SaveDataCloudAsync(json, gData.data.Count - 1);
                    }
                }
            }
        }
    }

    

    internal bool CanBuild(BuildingType buildingType, Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, buildingType.prefab.GetComponent<BoxCollider>().size, Quaternion.identity, gData.GetLayerMaskBuilding());
        //Gizmos.DrawCube(position, buildingType.prefab.GetComponent<BoxCollider>().size);

        if (colliders.Length > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
