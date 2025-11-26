using UnityEngine;

public class ItemMover : MonoBehaviour
{
    [HideInInspector] public Transform centerPoint;
    [HideInInspector] public float moveSpeed;

    void Update()
    {
        // Calculate the direction vector pointing from the item to the center
        Vector3 directionToCenter = centerPoint.position - transform.position;

        // Move the item inward based on speed and time
        transform.position += directionToCenter.normalized * moveSpeed * Time.deltaTime;

        // Cleanup: Destroy the item if it passes the center pivot closely
        if (directionToCenter.magnitude < 0.5f)
        {
            Destroy(gameObject);
        }
    }
}