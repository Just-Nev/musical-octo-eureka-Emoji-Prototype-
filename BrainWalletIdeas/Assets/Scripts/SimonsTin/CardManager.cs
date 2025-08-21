using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public List<GameObject> possibleCards; // could overlap with prefabs
    private GameObject currentCard;
    private PrefabSpawner spawner;

    void Start()
    {
        spawner = FindObjectOfType<PrefabSpawner>();
        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        // show the lineup first
        yield return StartCoroutine(spawner.ShowPrefabs());

        // decide if we should show a "known" or "new" card
        bool chooseShown = Random.value > 0.5f;
        GameObject chosen;

        if (chooseShown && spawner.possiblePrefabs.Count > 0)
        {
            // pick from the prefabs list
            chosen = spawner.possiblePrefabs[Random.Range(0, spawner.possiblePrefabs.Count)];
        }
        else
        {
            // pick from card list
            chosen = possibleCards[Random.Range(0, possibleCards.Count)];
        }

        // spawn card in the center of screen
        float zDist = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, zDist));
        center.z = 0f;

        currentCard = Instantiate(chosen, center, Quaternion.identity);
    }

    // call this from UI buttons or swipe logic
    public void PlayerAnswer(bool saidYes)
    {
        bool wasShown = spawner.WasPrefabShown(currentCard);

        if (saidYes == wasShown)
        {
            Debug.Log("Correct!");
        }
        else
        {
            Debug.Log("Wrong!");
        }

        Destroy(currentCard);

        // loop next round
        StartCoroutine(GameFlow());
    }
}







