using System;
using UnityEngine;

[Serializable]
[Tooltip("A box area in which items can be spawned")]
public struct SpawnBox
{
    [Tooltip("The center position of the box relative to the origin")]
    public Vector3 BoxPos;

    [Tooltip("The size of the box in each dimension")]
    public Vector3 BoxSize;

    [Tooltip("Spawns an GO at a random position within the box on a surface with the specified layer")]
    public Transform SpawnItem(GameObject item, Vector3 origin, LayerMask spawnLayer)
    {
        if (item == null) { Console.WriteLine("No item found to spawn"); return null; };

        //Position within the confines of the box
        var randomPos = new Vector3(
            UnityEngine.Random.Range(BoxPos.x - BoxSize.x / 2, BoxPos.x + BoxSize.x / 2),
            UnityEngine.Random.Range(BoxPos.y - BoxSize.y / 2, BoxPos.y + BoxSize.y / 2),
            UnityEngine.Random.Range(BoxPos.z - BoxSize.z / 2, BoxPos.z + BoxSize.z / 2)
        );

        //Position where the ray will be shot from
        var rayPos = randomPos + origin + Vector3.up * 5;

        //Shoots a ray to find any surface that matches the spawnLayer's type
        if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, rayPos.y + 10, spawnLayer))
        {
            //cant call due to it being a struct, but this. sure, yeah why the fuck not..... stupid ass game engine
            GameObject enemy = UnityEngine.Object.Instantiate(item, hit.point, Quaternion.identity);
            return enemy.transform;
            //Anyways, we just spawn an item at the random position 
        }
        else
        {
            Console.WriteLine("No surface with the layer hit");
        }

        return null;
    }
}
