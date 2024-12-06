using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs; // Array di prefab delle macchine
    public Transform spawnPoint;    // Punto di spawn (cubo)

    private float spawnTimer;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnCar();
            spawnTimer = Random.Range(5, 15);
        }
    }

    void SpawnCar()
    {
        if (spawnPoint != null)
        {
            // Scegli un prefab casuale dall'array
            int randomIndex = Random.Range(0, carPrefabs.Length);
            GameObject car = Instantiate(carPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation); // Spawn della macchina
            car.AddComponent<CarMovement>(); 
        }
        else
        {
            Debug.LogError("Spawn Point not assigned!");
        }
    }
}
