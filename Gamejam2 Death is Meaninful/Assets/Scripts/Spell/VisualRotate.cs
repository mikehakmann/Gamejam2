using UnityEngine;

public class VisualRotate : MonoBehaviour
{
    public int speed = 1000;
    void Update()
    {
        //rotate the object continuously
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
