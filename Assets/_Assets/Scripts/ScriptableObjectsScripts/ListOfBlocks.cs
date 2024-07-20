using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuidlingList", menuName = "buldingSystem/buildinglist", order = 2)]
public class ListOfBlocks : ScriptableObject
{
    public List<BuildingMaterial> materials;

}
