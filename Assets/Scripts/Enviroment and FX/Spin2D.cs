using UnityEngine;

public class Spin2D : MonoBehaviour
{
    public float rotateSpeed;

    void Update () => transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.World);
}
