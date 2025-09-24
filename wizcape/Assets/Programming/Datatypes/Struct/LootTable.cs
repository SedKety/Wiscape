using JetBrains.Annotations;
using System;
using System.Linq;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DamageInstance", menuName = "ScriptableObjects/LootTables")]
public class LootTable : ScriptableObject
{
    [SerializeField] private LootItem[] items;

    public GameObject GetRandomItem()
    {
        SortTable();
        var randomNum = UnityEngine.Random.Range(0, 101);
        for(int i = 0; i < items.Length; i++)
        {
            if(items[i].weight >= randomNum)  return items[i].itemGO;
        }
        Console.WriteLine("Somehow was unable to retrieve an item");
        return items.RandomItem().itemGO;
    }

    public void SortTable()
    {
        var middleMan = new LootItem();
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i].weight < items[0].weight)
            {
                middleMan = items[0];
                items[0] = items[i];
                items[i] = middleMan;
            }
        }
        foreach(var item in items) { Console.WriteLine(item.weight); }
    }
}

[System.Serializable]
public struct LootItem
{
    [Range(1, 100)] public int weight;
    public GameObject itemGO;
}
