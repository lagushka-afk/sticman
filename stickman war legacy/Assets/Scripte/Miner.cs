using UnityEngine;

public class SpawnOnButton : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private Transform spawnPoint; 

    public void OnSpawnButtonClick()
    {
        Vector3 spawnPosition;

       
        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;
        }
        else
        {
            spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * spawnDistance;
        }

        GameObject newObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        
    }
}
