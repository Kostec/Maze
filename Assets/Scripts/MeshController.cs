using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour
{
    public SkinnedMeshRenderer meshRender;
    public BlendShapePoint[] shapePoints;
    private Dictionary<GameObject, BlendShapeValues> shapePointsMap = null;
    private Mesh sharedMesh = null;
    // Start is called before the first frame update
    void Start()
    {
        shapePointsMap = new Dictionary<GameObject, BlendShapeValues>();
        foreach (BlendShapePoint point in shapePoints)
        {
            shapePointsMap.Add(point.point, point.values);
        }
        sharedMesh = new Mesh();
        updateMeshColider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePinHeight(GameObject colider, float value)
    {
        float delta = value - colider.transform.position.y;
        print(delta);
        BlendShapeValues values;
        if (shapePointsMap.TryGetValue(colider, out values))
        {
            int newShapeWeight = (int)(LinIntPol(delta, values.minValue, values.maxValue, values.minShapeWeigth, values.maxShapeWeigth) + 0.5);
            meshRender.SetBlendShapeWeight(values.BlendShapeId, newShapeWeight);
            updateMeshColider();
        }
    }
    float LinIntPol(float x, float x1, float x2, float y1, float y2)
    {
        return (x - x1) * (y2 - y1) / (x2 - x1) + y1;
    }

    void updateMeshColider()
    {
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshRender.BakeMesh(sharedMesh);
        meshCollider.sharedMesh = sharedMesh;
    }
}

[System.Serializable]
public struct BlendShapeValues
{
    public float maxValue;
    public int maxShapeWeigth;
    public float minValue;
    public int minShapeWeigth;
    public int BlendShapeId;
}

[System.Serializable]
public struct BlendShapePoint
{
    public GameObject point;
    public BlendShapeValues values;
}
