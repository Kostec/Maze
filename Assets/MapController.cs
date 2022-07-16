using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    Dictionary<GameObject, List<GameObject>> pinsMap;
    // Start is called before the first frame update
    void Start()
    {
        pinsMap = new Dictionary<GameObject, List<GameObject>>();
        CreateMap();
    }

    void CreateMap()
    { 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPinEnterTrigger(GameObject pinObject, GameObject newObject)
    {
        List<GameObject> points = null;
        if (newObject != null)
        {
            if (pinsMap.ContainsKey(pinObject))
            {
                if (pinsMap.TryGetValue(pinObject, out points))
                {
                    if (!points.Contains(newObject))
                    {
                        points.Add(newObject);
                    }
                }
            }
            else
            {
                points = new List<GameObject>();
                points.Add(newObject);
                pinsMap.Add(pinObject, points);
            }
            if (points != null)
            {
                CalcMeshPoint(points);
            }
        } 
    }

    public void OnPinExitTrigger(GameObject pinObject, GameObject exitObject)
    {
        List<GameObject> points = null;
        if (exitObject != null)
        {
            if (pinsMap.ContainsKey(pinObject))
            {
                if (pinsMap.TryGetValue(pinObject, out points))
                {
                    if (!points.Contains(exitObject))
                    {
                        points.Remove(exitObject);
                    }
                }
            }
        }
        if (points != null)
        {
            CalcMeshPoint(points);
        }
    }

    void CalcMeshPoint(List<GameObject> points)
    {
        if (points != null)
        {
            if (points.Count > 0)
            {
                float calcHeght = 0;
                foreach (GameObject point in points)
                {
                    calcHeght += point.transform.position.y;
                }
                calcHeght/=points.Count;
                foreach (GameObject point in points)
                {
                    MeshController meshController = point.GetComponentInParent<MeshController>();
                    if (meshController != null)
                    {
                        meshController.UpdatePinHeight(point, calcHeght);
                    }
                }
            }
        }
    }
}
