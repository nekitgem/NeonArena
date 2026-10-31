using UnityEngine;

public class EnergyCore : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        // Кристалл плавно крутится вокруг своей оси Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
