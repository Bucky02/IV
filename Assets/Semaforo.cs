using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Semaforo : Agent
{
    public GameObject roadA; // Riferimento alla prima strada
    public GameObject roadB; // Riferimento alla seconda strada
    public GameObject greenLightRoadA; // Semaforo verde per Road A
    public GameObject yellowLightRoadA; // Semaforo giallo per Road A
    public GameObject redLightRoadA; // Semaforo rosso per Road A
    public GameObject greenLightRoadB; // Semaforo verde per Road B
    public GameObject yellowLightRoadB; // Semaforo giallo per Road B
    public GameObject redLightRoadB; // Semaforo rosso per Road B
 
    private int carsOnRoadA;
    private int carsOnRoadB;
    private bool isRoadAGreen; // Semaforo attivo per Road A

    public override void OnEpisodeBegin()
    {
        // Inizializza il conteggio delle auto e lo stato del semaforo
        carsOnRoadA = 0;
        carsOnRoadB = 0;
        isRoadAGreen = true; // Road A parte con il semaforo verde
        UpdateLightStates();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Osserva il numero di auto su entrambe le strade
        sensor.AddObservation(carsOnRoadA);
        sensor.AddObservation(carsOnRoadB);

        // Osserva lo stato del semaforo attuale
        sensor.AddObservation(isRoadAGreen ? 1 : 0);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Azione: 0 = Mantieni lo stato, 1 = Cambia strada
        int action = actions.DiscreteActions[0];

        if (action == 1)
        {
            isRoadAGreen = !isRoadAGreen;
            UpdateLightStates();
        }

        // Penalità per mantenere il semaforo attivo su una strada meno trafficata
        if (isRoadAGreen && carsOnRoadB > carsOnRoadA)
        {
            AddReward(-0.1f);
        }
        else if (!isRoadAGreen && carsOnRoadA > carsOnRoadB)
        {
            AddReward(-0.1f);
        }

        // Ricompensa per dare priorità alla strada più trafficata
        if (isRoadAGreen && carsOnRoadA >= carsOnRoadB)
        {
            AddReward(0.1f);
        }
        else if (!isRoadAGreen && carsOnRoadB >= carsOnRoadA)
        {
            AddReward(0.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Logica manuale per test
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = carsOnRoadA > carsOnRoadB ? 0 : 1;
    }

    private void UpdateLightStates()
    {
        // Aggiorna lo stato dei semafori per entrambe le strade
        if (isRoadAGreen)
        {
            SetLightState(greenLightRoadA, yellowLightRoadA, redLightRoadA, true);
            SetLightState(greenLightRoadB, yellowLightRoadB, redLightRoadB, false);
        }
        else
        {
            SetLightState(greenLightRoadA, yellowLightRoadA, redLightRoadA, false);
            SetLightState(greenLightRoadB, yellowLightRoadB, redLightRoadB, true);
        }
    }

    private void SetLightState(GameObject greenLight, GameObject yellowLight, GameObject redLight, bool isGreen)
    {
        // Spegne tutte le luci prima di attivarne una
        greenLight.SetActive(false);
        yellowLight.SetActive(false);
        redLight.SetActive(false);

        // Accende la luce appropriata
        if (isGreen)
        {
            greenLight.SetActive(true);
        }
        else
        {
            redLight.SetActive(true);
        }
    }

    private void Update()
    {
        // Aggiorna il conteggio delle auto
        carsOnRoadA = CountCarsOnRoad(roadA);
        carsOnRoadB = CountCarsOnRoad(roadB);
    }

    private int CountCarsOnRoad(GameObject road)
    {
        Collider[] cars = Physics.OverlapBox(road.transform.position, road.transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("Car"));
        return cars.Length;
    }
      public bool IsRedOrYellow()
{
    // Restituisce true se il semaforo � rosso
    return redLightRoadA.activeSelf  || redLightRoadB.activeSelf  || yellowLightRoadA.activeSelf || yellowLightRoadB.activeSelf;
}
}
