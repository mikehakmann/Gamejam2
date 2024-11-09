using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager Instance; // Singleton instance

    private CinemachineImpulseSource impulseSource; // Change Camera to CinemachineVirtualCamera
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shake();
        }
    }
    [ContextMenu("Shake")]
    public void Shake()
    {
        impulseSource.GenerateImpulse();


    }
}
