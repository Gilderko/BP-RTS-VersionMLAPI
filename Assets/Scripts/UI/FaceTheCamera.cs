using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTheCamera : MonoBehaviour
{
#if UNITY_SERVER == false
    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
#endif
}
