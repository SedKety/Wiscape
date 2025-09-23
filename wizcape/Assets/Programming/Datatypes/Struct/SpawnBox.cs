using System;
using UnityEngine;

[Serializable]
public struct SpawnBox
{
    public Vector3 BoxPos;
    public Vector3 BoxSize;

    public void SpawnItem(GameObject item, Vector3 origin, LayerMask spawnLayer)
    {
        if (item == null) { Console.WriteLine("No item found to spawn"); return; };

        var randomPos = new Vector3(
            UnityEngine.Random.Range(BoxPos.x - BoxSize.x / 2, BoxPos.x + BoxSize.x / 2),
            UnityEngine.Random.Range(BoxPos.y - BoxSize.y / 2, BoxPos.y + BoxSize.y / 2),
            UnityEngine.Random.Range(BoxPos.z - BoxSize.z / 2, BoxPos.z + BoxSize.z / 2)
        );

        var rayPos = randomPos + origin + Vector3.up * 5;
        if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, rayPos.y + 10, spawnLayer))
        {
            UnityEngine.Object.Instantiate(item, hit.point, Quaternion.identity);
        }
        else
        {
            Console.WriteLine("No surface with the layer hit");
        }
    }
}
