using System.Collections;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    // Riferimenti alle luci
    public GameObject greenLight;
    public GameObject yellowLight;
    public GameObject redLight;

    // Durata di ciascuna luce in secondi
    public float greenDuration = 5f;
    public float yellowDuration = 2f;
    public float redDuration = 5f;

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());
    }

    private IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // Accendi la luce verde
            SetLight(greenLight, true);
            SetLight(yellowLight, false);
            SetLight(redLight, false);
            yield return new WaitForSeconds(greenDuration);

            // Accendi la luce arancione
            SetLight(greenLight, false);
            SetLight(yellowLight, true);
            SetLight(redLight, false);
            yield return new WaitForSeconds(yellowDuration);

            // Accendi la luce rossa
            SetLight(greenLight, false);
            SetLight(yellowLight, false);
            SetLight(redLight, true);
            yield return new WaitForSeconds(redDuration);
        }
    }

    // Metodo per accendere/spegnere una luce
    private void SetLight(GameObject light, bool state)
    {
        if (light != null)
        {
            light.SetActive(state);
        }
    }
}

