using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; 

    [Header("Settings")]
    public float smoothSpeed = 0.125f; 
    public Vector3 offset; 

    void Start()
    {

        // Рассчитываем начальное смещение (чтобы камера не дергалась)
        offset = transform.position - target.position;
    }

   
    void LateUpdate()
    {
      
        Vector3 desiredPosition = target.position + offset;

       
       
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

     
        transform.position = smoothedPosition;
    }
}