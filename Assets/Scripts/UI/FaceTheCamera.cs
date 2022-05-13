using UnityEngine;

public class FaceTheCamera : MonoBehaviour
{

#if !UNITY_SERVER

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }

#endif

}
