using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuidlingBlock", menuName = "buldingSystem/buildingblock",order =1)]
public class BuildingMaterial : ScriptableObject
{
    public string name;
    public Sprite image;
    public Transform block;
    public float hp;
    public int count;
}
