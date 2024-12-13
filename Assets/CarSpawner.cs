using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;   // Array di prefab delle macchine
    public Transform spawnPoint;      // Punto di spawn
    public Transform trafficLight;    // Semaforo associato a questo spawner

    private float spawnTimer;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnCar();
            spawnTimer = Random.Range(2, 5); // Tempo casuale tra gli spawn
        }
    }

    void SpawnCar()
    {
        if (spawnPoint != null && trafficLight != null)
        {
            int randomIndex = Random.Range(0, carPrefabs.Length);
            GameObject car = Instantiate(carPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);

            // Assegna il tag "Car" automaticamente
            car.tag = "Car";

            // Configura il comportamento della macchina
            CarMovement carMovement = car.AddComponent<CarMovement>();
            carMovement.trafficLight = trafficLight; // Assegna il semaforo corretto
        }
        else
        {
            Debug.LogError("Spawn Point or Traffic Light not assigned!");
        }
    }
}
