using UnityEngine;

public class CenterRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public bool clockwise = true;

    void Update()
    {
        float angle = rotationSpeed * Time.deltaTime;

        if (!clockwise)
        {
            angle *= -1;
        }

        transform.Rotate(0, 0, angle);
    }
}