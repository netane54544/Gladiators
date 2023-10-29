using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Build : MonoBehaviour
{
    [SerializeField]
    private GameData gData;
    private GameObject building;
    [SerializeField]
    private Transform canvasDynamic;
    [SerializeField]
    private GameObject buildPrefab;
    [SerializeField]
    private GameObject cancelPrefab;
    [SerializeField]
    private GameObject arrow1;
    [SerializeField]
    private GameObject arrow2;
    [SerializeField,Range(0f, 1f)]
    private float smoothTime = 0.1f;
    private Vector3 oldPos = Vector3.zero;
    [SerializeField]
    private Color defualtColor;
    public Vector3 offset;

    private void Update()
    {
        if(building == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BuildingData data = new() { building = gData.currentBuilding, position = hit.point };

                building = GameObject.Instantiate(gData.GetConfig(data).constractionPrefab.gameObject, hit.point, Quaternion.Euler(0, 180, 0));
                var bP = GameObject.Instantiate(buildPrefab, canvasDynamic, false);
                bP.GetComponent<FollowConstructionUI>().lookAt = building.transform;
                var cP = GameObject.Instantiate(cancelPrefab, canvasDynamic, false);
                cP.GetComponent<FollowConstructionUI>().lookAt = building.transform;
                var a1 = GameObject.Instantiate(arrow1, canvasDynamic, false);
                a1.GetComponent<FollowConstructionUI>().offset = new Vector3(-building.GetComponent<BoxCollider>().size.z + 2, 0, 0);
                a1.GetComponent<FollowConstructionUI>().lookAt = building.transform;
                var a2 = GameObject.Instantiate(arrow2, canvasDynamic, false);
                a2.GetComponent<FollowConstructionUI>().offset = new Vector3(Mathf.Floor(building.GetComponent<BoxCollider>().size.z), 0, 0);
                a2.GetComponent<FollowConstructionUI>().lookAt = building.transform;

                var checkRenderer = building.GetComponent<Renderer>();
                if (checkRenderer == null)
                {
                    if (!CanBuild(gData.GetConfig(data), building.transform.position, building.transform.localScale))
                    {
                        building.GetComponentsInChildren<Renderer>().ToList().ForEach(x => x.material.SetColor("_LightColor", Color.red));
                    }
                    else
                    {
                        building.GetComponentsInChildren<Renderer>().ToList().ForEach(x => x.material.SetColor("_LightColor", defualtColor));
                    }
                }
                else
                {
                    if (!CanBuild(gData.GetConfig(data), building.transform.position, building.transform.localScale))
                    {

                        building.GetComponent<Renderer>().material.SetColor("_LightColor", Color.red);
                    }
                    else
                    {
                        building.GetComponent<Renderer>().material.SetColor("_LightColor", defualtColor);
                    }
                }

                bP.GetComponent<Button>().onClick.AddListener(() =>
                {
                    //Get the position and destroy
                    Vector3 pos = building.transform.position;

                    if(BuildConstruction(data.building, pos, out GameObject storeBuild))
                    {
                        Destroy(cP);
                        Destroy(bP);
                        Destroy(a1);
                        Destroy(a2);
                        Destroy(building);
                        building = null;

                        gData.capacityConstruction.AddValue(-1, data.building);
                        var json = JsonUtility.ToJson(gData.capacityConstruction);
                        Debug.Log(json);
                        GlobalUtil.Instance.SaveDataCloudAsync(json, -1);

                        storeBuild.SetActive(true);
                        gData.canBuild = false;
                        this.enabled = false;
                    }

                });

                cP.GetComponent<Button>().onClick.AddListener(() =>
                {
                    //destroy
                    Destroy(cP);
                    Destroy(bP);
                    Destroy(a1);
                    Destroy(a2);
                    Destroy(building);
                    building = null;

                    gData.canBuild = false;
                    this.enabled = false;
                });

                a1.GetComponent<Button>().onClick.AddListener(() => 
                {
                    building.transform.Rotate(new Vector3(0, 90, 0));
                });

                a2.GetComponent<Button>().onClick.AddListener(() =>
                {
                    building.transform.Rotate(new Vector3(0, -90, 0));
                });
            }
        }
        else
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, gData.GetLayerMaskGround()))
                {
                    if (!EventSystem.current.IsPointerOverGameObject()) //Check if our touch is over an ui element
                    {
                        building.transform.position = Vector3.Lerp(building.transform.position, new Vector3(hit.point.x, 0, hit.point.z), smoothTime);

                        if (building.transform.position != oldPos)
                        {
                            BuildingData data = new() { building = gData.currentBuilding, position = building.transform.position };
                            var checkRenderer = building.GetComponent<Renderer>();

                            if (checkRenderer == null)
                            {
                                if (!CanBuild(gData.GetConfig(data), building.transform.position, building.transform.localScale))
                                {

                                    building.GetComponentsInChildren<Renderer>().ToList().ForEach(x => x.material.SetColor("_LightColor", Color.red));
                                }
                                else
                                {
                                    building.GetComponentsInChildren<Renderer>().ToList().ForEach(x => x.material.SetColor("_LightColor", defualtColor));
                                }
                            }
                            else
                            {
                                if (!CanBuild(gData.GetConfig(data), building.transform.position, building.transform.localScale))
                                {

                                    building.GetComponent<Renderer>().material.SetColor("_LightColor", Color.red);
                                }
                                else
                                {
                                    building.GetComponent<Renderer>().material.SetColor("_LightColor", defualtColor);
                                }
                            }

                            oldPos = building.transform.position;
                        }
                    }
                }
            }
        }
    }

    private bool BuildConstruction(TypeOf _building, Vector3 position, out GameObject createdObject)
    {
        gData.data.Add(new() { building = _building, position = position});
        gData.data[^1].id = gData.data.Count - 1;

        bool tB = CanBuild(gData.GetConfig(gData.data[^1]), position, gData.GetConfig(gData.data[^1]).prefab.transform.localScale);

        if (tB)
        {
            GameObject ob = Instantiate(gData.GetConfig(gData.data[^1]).prefab.gameObject, position, building.transform.rotation);
            ob.name = ((int)_building).ToString(); //easier to parse back as a number if we need to store
            ob.SetActive(false);
            createdObject = ob;
            gData.data[^1].rotation = ob.transform.rotation;
            gData.data[^1].scale = ob.transform.localScale;

            var jsonData = JsonUtility.ToJson(gData.data[^1]);

            GlobalUtil.Instance.SaveDataCloudAsync(jsonData, gData.data.Count - 1);

            return true;
        }
        else
        {
            createdObject = null;
            return false;
        }
    }

    private void OnDestroy()
    {
        //Makes sure it exits the building
        gData.canBuild = false;
        this.enabled = false;
    }

    internal bool CanBuild(BuildingType buildingType, Vector3 position, Vector3 localScale)
    {
        Collider[] colliders = Physics.OverlapBox(position - new Vector3(0, -1.59f, 0), 
            new Vector3(buildingType.prefab.GetComponent<BoxCollider>().size.x * localScale.x, buildingType.prefab.GetComponent<BoxCollider>().size.y * localScale.y, buildingType.prefab.GetComponent<BoxCollider>().size.z * localScale.z), 
            Quaternion.identity, gData.GetLayerMaskBuilding());

        if (colliders.Length > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    /*
    private void OnDrawGizmos()
    {
        if(building != null)
        {
            Gizmos.DrawWireCube(building.transform.position - new Vector3(0, -1.59f, 0), new Vector3(building.GetComponent<BoxCollider>().size.x * building.transform.localScale.x, building.GetComponent<BoxCollider>().size.y * building.transform.localScale.y, building.GetComponent<BoxCollider>().size.z * building.transform.localScale.z));
        }
    }
    */
}
