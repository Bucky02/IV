using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    public float speed = 10f; // Velocità dell'auto
    private bool canMove = true; // Indica se la macchina può muoversi
    public Transform trafficLight; // Riferimento al semaforo
    public float stopDistance = 15f; // Distanza dalla linea del semaforo per fermarsi
    public float carStopDistance = 22f; // Distanza minima da un'altra macchina per fermarsi

    private void Update()
{
    bool shouldMove = CanMoveBasedOnTrafficLight() && CanMoveBasedOnCars();

    if (shouldMove)
    {
        canMove = true;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    else if (canMove)
    {
        canMove = false;
        StartCoroutine(BrakeSmoothly());
    }
}

private IEnumerator BrakeSmoothly()
{
    float currentSpeed = speed;

    while (currentSpeed > 0)
    {
        currentSpeed -= speed * Time.deltaTime;
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        yield return null;
    }
}


   private bool CanMoveBasedOnTrafficLight()
{
    if (trafficLight == null) return true;

    float distanceToLight = Vector3.Distance(transform.position, trafficLight.position);
    Vector3 toTrafficLight = (trafficLight.position - transform.position).normalized;
    bool isTrafficLightAhead = Vector3.Dot(transform.forward, toTrafficLight) > 0;

    // Debug per il semaforo
    Debug.Log($"Distanza dal semaforo: {distanceToLight}, Semaforo davanti: {isTrafficLightAhead}");

    if (isTrafficLightAhead && distanceToLight <= stopDistance)
    {
        Semaforo semaforoScript = trafficLight.GetComponent<Semaforo>();
        if (semaforoScript != null)
        {
            Debug.Log($"Semaforo: Verde? {semaforoScript.IsGreen()}");
            if (!semaforoScript.IsGreen()) // Cambiato da IsRedOrYellow a IsGreen
            {
                return false;
            }
        }
    }

    return true;
}



    private bool CanMoveBasedOnCars()
{
    RaycastHit hit;
    if (Physics.Raycast(transform.position, transform.forward, out hit, carStopDistance))
    {
        if (hit.collider != null && hit.collider.CompareTag("Car"))
        {
            return false;
        }
    }

    return true;
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
