using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Semaforo : Agent
{
    public GameObject roadA; // Prima strada
    public GameObject roadB; // Seconda strada
    public GameObject greenLightRoadA, yellowLightRoadA, redLightRoadA; // Luci per Road A
    public GameObject greenLightRoadB, yellowLightRoadB, redLightRoadB; // Luci per Road B

    private int carsOnRoadA;
    private int carsOnRoadB;
    private bool isRoadAGreen; // Stato del semaforo per Road A
    private float timer; // Per il controllo del tempo sullo stato attuale

    public float maxGreenTime = 10.0f; // Tempo massimo per luce verde
    public float minSwitchInterval = 2.0f; // Intervallo minimo per cambiare stato

    public override void OnEpisodeBegin()
    {
        // Inizializza lo stato del semaforo
        carsOnRoadA = 0;
        carsOnRoadB = 0;
        timer = 0.0f;
        isRoadAGreen = true;
        UpdateLightStates();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Osservazioni sul traffico
        sensor.AddObservation(carsOnRoadA); // Numero di auto su Road A
        sensor.AddObservation(carsOnRoadB); // Numero di auto su Road B

        // Osservazioni sullo stato del semaforo
        sensor.AddObservation(isRoadAGreen ? 1 : 0); // Stato attuale (1 se Road A è verde, 0 altrimenti)
        sensor.AddObservation(timer / maxGreenTime); // Tempo trascorso come percentuale del tempo massimo
    }

    public bool IsRedOrYellow()
    {
        // Restituisce true se il semaforo è rosso o giallo
        return redLightRoadA.activeSelf || yellowLightRoadA.activeSelf || redLightRoadB.activeSelf || yellowLightRoadB.activeSelf;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Azioni possibili: 0 = Mantieni stato, 1 = Cambia semaforo
        int action = actions.DiscreteActions[0];

        timer += Time.deltaTime;

        if (action == 1 && timer >= minSwitchInterval)
        {
            // Cambia lo stato del semaforo
            isRoadAGreen = !isRoadAGreen;
            timer = 0.0f;
            UpdateLightStates();
        }

        // Ricompense basate sul traffico
        if (isRoadAGreen && carsOnRoadA > carsOnRoadB)
        {
            AddReward(0.1f); // Ricompensa per dare priorità a Road A con più traffico
        }
        else if (!isRoadAGreen && carsOnRoadB > carsOnRoadA)
        {
            AddReward(0.1f); // Ricompensa per dare priorità a Road B con più traffico
        }
        else
        {
            AddReward(-0.01f); // Penalità per scelte meno ottimali
        }

        // Penalità per mantenere il verde troppo a lungo senza necessità
        if (timer >= maxGreenTime)
        {
            AddReward(-0.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Logica manuale: cambia stato se una strada ha molte più auto
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = carsOnRoadA > carsOnRoadB ? 0 : 1;
    }

    private void UpdateLightStates()
    {
        // Aggiorna lo stato dei semafori
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
        // Gestisce le luci del semaforo
        greenLight.SetActive(isGreen);
        yellowLight.SetActive(false);
        redLight.SetActive(!isGreen);
    }

    private void Update()
    {
        // Aggiorna il conteggio delle auto in base alla loro posizione
        carsOnRoadA = CountCarsOnRoad(roadA);
        carsOnRoadB = CountCarsOnRoad(roadB);
    }

    private int CountCarsOnRoad(GameObject road)
    {
        // Conta le auto sulla strada utilizzando un box collider
        Collider[] cars = Physics.OverlapBox(
            road.transform.position,
            road.transform.localScale / 2,
            Quaternion.identity,
            LayerMask.GetMask("Car")
        );
        return cars.Length;
    }
}
