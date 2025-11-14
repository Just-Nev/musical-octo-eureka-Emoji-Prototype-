using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPathMover : MonoBehaviour
{
    [Header("Objects to spawn")]
    public List<GameObject> objectsToSpawn; // Drag all prefabs here

    public float moveSpeed = 3f;
    public float spawnDelay = 2f;

    [Header("Offsets from screen edges")]
    public float leftOffset = 0.5f;
    public float rightOffset = 0.5f;
    public float topOffset = 0.5f;
    public float bottomOffset = 0.5f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnNewObject();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void SpawnNewObject()
    {
        if (objectsToSpawn.Count == 0) return;

        // Pick a random prefab
        GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];

        StartCoroutine(SpawnAndMove(prefab));
    }

    IEnumerator SpawnAndMove(GameObject prefab)
    {
        // Screen corners
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        topLeft.z = bottomLeft.z = bottomRight.z = topRight.z = 0;

        // Apply offsets
        topLeft.x += leftOffset; topLeft.y -= topOffset;
        bottomLeft.x += leftOffset; bottomLeft.y += bottomOffset;
        bottomRight.x -= rightOffset; bottomRight.y += bottomOffset;
        topRight.x -= rightOffset; topRight.y -= topOffset;

        // Spawn object
        GameObject obj = Instantiate(prefab, topLeft, Quaternion.identity);

        // Move along path
        yield return StartCoroutine(MoveTo(obj, bottomLeft));
        yield return StartCoroutine(MoveTo(obj, bottomRight));
        yield return StartCoroutine(MoveTo(obj, topRight));

        // Offscreen exit
        Vector3 offScreen = topRight + new Vector3(2f, 0, 0);
        yield return StartCoroutine(MoveTo(obj, offScreen));

        Destroy(obj);
    }

    IEnumerator MoveTo(GameObject obj, Vector3 target)
    {
        Draggable drag = obj.GetComponent<Draggable>();

        while (obj != null && Vector3.Distance(obj.transform.position, target) > 0.05f)
        {
            if (drag == null || !drag.IsDragging)
            {
                obj.transform.position = Vector3.MoveTowards(
                    obj.transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
            }
            yield return null;
        }
    }
}

