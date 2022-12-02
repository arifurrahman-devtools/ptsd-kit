using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate2 : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(Vector3.up, 180 * Time.deltaTime);
    }
}
