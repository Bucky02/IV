using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 10f;          // Velocit� dell'auto
    private bool canMove = true;      // Indica se la macchina pu� muoversi
    public Transform trafficLight;    // Riferimento al semaforo
    public float stopDistance = 15f;   // Distanza dalla linea del semaforo per fermarsi
    public float carStopDistance = 12f; // Distanza minima da un'altra macchina per fermarsi

    private void Update()
    {
        canMove = true; // Di default, consenti il movimento

        // Controllo semaforo
        if (trafficLight != null)
        {
            float distanceToLight = Vector3.Distance(transform.position, trafficLight.position);

            // Verifica se il semaforo � davanti
            Vector3 toTrafficLight = (trafficLight.position - transform.position).normalized;
            bool isTrafficLightAhead = Vector3.Dot(transform.forward, toTrafficLight) > 0;

            // Se il semaforo � davanti e vicino
            if (isTrafficLightAhead && distanceToLight >= 13 && distanceToLight <= stopDistance)
            {
                Semaforo semaforoScript = trafficLight.GetComponent<Semaforo>();
                if (semaforoScript != null && semaforoScript.IsRedOrYellow())
                {
                    canMove = false; // Fermati se il semaforo � rosso o giallo
                }
            }
        }

        // Controllo delle altre macchine davanti
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, carStopDistance))
{
    if (hit.collider.CompareTag("Car"))
    {
        float distanceToCar = Vector3.Distance(transform.position, hit.collider.transform.position);
        if (distanceToCar <= carStopDistance) // Aggiungi un controllo sulla distanza
        {
            Debug.Log("Macchina davanti rilevata! Fermati.");
            canMove = false;
        }
    }
}


        // Movimento della macchina
        if (canMove)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    // Debug per visualizzare il raycast
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * carStopDistance);
    }
}
