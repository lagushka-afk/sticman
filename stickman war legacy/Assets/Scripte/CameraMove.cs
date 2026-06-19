using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float Speed = 500f;
    public float MinX = -500f;
    public float MaxX = 500f;

    private Transform Rect;

    void Start()
    {
        Rect = GetComponent<Transform>();
    }

    void Update()
    {
        float Move = 0;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            Move = Speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            Move = -Speed * Time.deltaTime;

        float NewX = Rect.transform.position.x + Move;
        NewX = Mathf.Clamp(NewX, MinX, MaxX);
        Rect.transform.position = new Vector2(NewX, Rect.transform.position.y);
    }
}