using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 10f; // Velocit� dell'auto
    private bool canMove = true; // Indica se la macchina pu� muoversi
    public Transform trafficLight; // Riferimento al semaforo
    public float stopDistance = 15f; // Distanza dalla linea del semaforo per fermarsi
    public float carStopDistance = 12f; // Distanza minima da un'altra macchina per fermarsi

    private void Update()
    {
        // Controlla se l'auto pu� muoversi
        canMove = CanMoveBasedOnTrafficLight() && CanMoveBasedOnCars();

        // Movimento della macchina
        if (canMove)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private bool CanMoveBasedOnTrafficLight()
    {
        // Se non c'� un semaforo, continua a muoverti
        if (trafficLight == null) return true;

        // Calcola la distanza dal semaforo
        float distanceToLight = Vector3.Distance(transform.position, trafficLight.position);

        // Controlla se il semaforo � davanti
        Vector3 toTrafficLight = (trafficLight.position - transform.position).normalized;
        bool isTrafficLightAhead = Vector3.Dot(transform.forward, toTrafficLight) > 0;

        // Se il semaforo � davanti e abbastanza vicino
        if (isTrafficLightAhead && distanceToLight <= stopDistance)
        {
            Semaforo semaforoScript = trafficLight.GetComponent<Semaforo>();
            if (semaforoScript != null && semaforoScript.IsRedOrYellow())
            {
                return false; // Fermati se il semaforo � rosso o giallo
            }
        }

        return true; // Altrimenti, muoviti
    }

    private bool CanMoveBasedOnCars()
    {
        // Controlla se c'� un'altra macchina davanti usando un Raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, carStopDistance))
        {
            if (hit.collider.CompareTag("Car"))
            {
                Debug.Log("Macchina davanti rilevata! Fermati.");
                return false; // Fermati se c'� un'auto troppo vicina
            }
        }

        return true; // Altrimenti, muoviti
    }

    // Debug per visualizzare il raycast e il controllo del semaforo
    private void OnDrawGizmos()
    {
        // Visualizza il raycast per il controllo delle auto
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * carStopDistance);

        // Visualizza il range del controllo semaforo
        if (trafficLight != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stopDistance);
        }
    }
}
