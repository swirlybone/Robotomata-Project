using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NotificationCenter;

public abstract class Spawnable : MonoBehaviour
{
    private float spawnHeight;
    public enum PlacementState { PLACED, FLOATING }
    public PlacementState placed = PlacementState.FLOATING;

    public float SpawnHeight { get => spawnHeight; set => spawnHeight = value; }

    public bool Floating() => placed == PlacementState.FLOATING;

    public void PlaceDown() { placed = PlacementState.PLACED; PostNotification("Placed", gameObject); }

    public Vector3 Normalize(Vector3 node)
    {
        return new Vector3(node.x, SpawnHeight, node.z);
    }
}
