using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabSpawner : MonoBehaviour
{
    public List<GameObject> possiblePrefabs;  // all prefabs you might show
    public float slideSpeed = 2f;
    public int numberToShow = 3;

    private List<string> shownPrefabNames = new List<string>();

    public IEnumerator ShowPrefabs()
    {
        shownPrefabNames.Clear();

        for (int i = 0; i < numberToShow; i++)
        {
            // pick random prefab
            GameObject prefab = possiblePrefabs[Random.Range(0, possiblePrefabs.Count)];

            // compute screen edges
            float zDist = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, zDist));
            Vector3 leftEdge = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, zDist));

            // spawn just off screen right
            Vector3 spawnPos = rightEdge + new Vector3(1f, 0, 0);
            spawnPos.z = 0f;

            // final target is just off screen left
            Vector3 targetPos = leftEdge - new Vector3(1f, 0, 0);
            targetPos.z = 0f;

            // create object
            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

            // remember its name for later check
            shownPrefabNames.Add(prefab.name);

            // slide all the way across until off screen left
            while (Vector3.Distance(obj.transform.position, targetPos) > 0.01f)
            {
                obj.transform.position = Vector3.MoveTowards(
                    obj.transform.position,
                    targetPos,
                    slideSpeed * Time.deltaTime
                );
                yield return null;
            }

            // destroy after it leaves
            Destroy(obj);

            // short pause before spawning the next one
            yield return new WaitForSeconds(0.5f);
        }
    }

    // check if card was one of the shown prefabs
    public bool WasPrefabShown(GameObject card)
    {
        string cleanName = card.name.Replace("(Clone)", "");
        return shownPrefabNames.Contains(cleanName);
    }
}










