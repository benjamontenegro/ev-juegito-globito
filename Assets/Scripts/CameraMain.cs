using UnityEngine;

public class CameraMain : MonoBehaviour
{
    public Transform target;

    [Range(0.001f, 0.5f)] public float cam_spd = 0.01f;
    Vector3 offset;

    void Awake()
    {
        offset = new Vector3(0, 0, -10);
        //target = GetComponent<Transform>();
    }

    void LateUpdate ()
    {
        Vector3 desired_pos = target.position +offset;
        Vector3 smoothed_pos = Vector3.Lerp(transform.position, desired_pos, cam_spd);
        transform.position = new Vector3(smoothed_pos.x, smoothed_pos.y, -10);

        //transform.LookAt(target);
    }
}
