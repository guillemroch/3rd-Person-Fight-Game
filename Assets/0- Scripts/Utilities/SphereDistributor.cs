using System;
using UnityEngine;

[ExecuteInEditMode]
public class SphereDistributor : MonoBehaviour
{
    // Public variables you can set in the Unity editor
    public GameObject spherePrefab;  // Reference to the sphere prefab
    public float sphereDiameter = 0.9f; // Scale of spheres relative to grid space
    public bool regenerateSpheres = false;


    private void Update() {
       if (regenerateSpheres)
           SpawnSpheresInBounds();
    }

    void SpawnSpheresInBounds() {
        CleanSpheres();
        regenerateSpheres = false;
         // Get the Renderer component of this GameObject (the cube or any mesh)
        Renderer renderer = GetComponent<Renderer>();

        // Get the bounds of the GameObject
        Bounds bounds = renderer.bounds;

        // Get the size of the bounds
        Vector3 size = bounds.size;

        // Calculate how many spheres fit along each axis, adjusting for the sphere's fixed diameter
        int gridSizeX = Mathf.FloorToInt(size.x / sphereDiameter);
        int gridSizeY = Mathf.FloorToInt(size.y / sphereDiameter);
        int gridSizeZ = Mathf.FloorToInt(size.z / sphereDiameter);

        // Calculate the offset to center the spheres in the bounds
        Vector3 offset = new Vector3(
            (size.x - gridSizeX * sphereDiameter) / 2f,
            (size.y - gridSizeY * sphereDiameter) / 2f,
            (size.z - gridSizeZ * sphereDiameter) / 2f
        );

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    float posX = bounds.min.x + offset.x + x * sphereDiameter + sphereDiameter / 2f;
                    float posY = bounds.min.y + offset.y + y * sphereDiameter + sphereDiameter / 2f;
                    float posZ = bounds.min.z + offset.z + z * sphereDiameter + sphereDiameter / 2f;

                    Vector3 position = new Vector3(posX, posY, posZ);

                    GameObject sphere = Instantiate(spherePrefab, position, Quaternion.identity);

                    sphere.transform.parent = this.transform;
                    sphere.transform.localScale = Vector3.one * sphereDiameter;
                }
            }
        }
    }

    void CleanSpheres() {
        if (transform.childCount == 0) return;
        Transform[] spheres = transform.GetComponentsInChildren<Transform>();
        foreach (var sphere in spheres) {
            if (sphere == transform) 
                continue;
            
            DestroyImmediate(sphere.gameObject);
        }
    }

}