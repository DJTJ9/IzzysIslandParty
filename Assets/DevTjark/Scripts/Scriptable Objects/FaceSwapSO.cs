using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Face Swapper", menuName = "Scriptable Objects/Swaggy Snapshots/Face Swapper", order = 1)]
public class FaceSwapSO : SerializedScriptableObject
{
    public List<Material> AllFaces =  new List<Material>();
    public List<Material> HappyFaces =  new List<Material>();
    public List<Material> SadFaces =  new List<Material>();

    public Material GetRandomFace(Material _material)
    {
        var possibleMaterials = AllFaces.Where(m => m != _material).ToList();
    
        if (possibleMaterials.Count == 0)
            return _material;
        
        int randomIndex = Random.Range(0, possibleMaterials.Count);
        return possibleMaterials[randomIndex];
    }
    
    public bool IsHappyFace(Material _material) => HappyFaces.Any(_m => _m.name == _material.name);
}