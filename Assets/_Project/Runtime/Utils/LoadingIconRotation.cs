using UnityEngine;

public class LoadingIconRotation : MonoBehaviour
{
    private const float RotationSpeed = 65f;
    

    private void Update()
    {
        transform.Rotate(Vector3.back, RotationSpeed * Time.unscaledDeltaTime);
    }
}
