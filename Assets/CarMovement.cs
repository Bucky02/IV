using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 10f;  // Velocità dell'auto (puoi modificarla nel pannello dell'Inspector)

    private void Update()
    {
        // Muovi l'auto in avanti
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
