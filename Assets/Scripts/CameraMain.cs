using UnityEngine;
using UnityEngine.InputSystem;

enum CameraMode { STANDART, FOLLOW, TEST }

public class CameraMain : MonoBehaviour
{
    public GameObject cameraTarget;
    
    public float camFollowSpd = 0.01f;
    private CameraMode camMode = CameraMode.STANDART;
    [Range(0.5f, 10f)] public float camTestSpd = 4f;
    
    private bool camDebug = false;
    private Vector3 cameraOffset;
    private Vector3 camPosDesired;

    private float DebugAccSpd = 0;

    void Start()
    {
        cameraOffset = new Vector3(0, 0, transform.position.z);

        // GET PLAYER
        if (cameraTarget == null)
            cameraTarget = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        SetCameraMode();
        
        switch(camMode)
        {
            default: break;
            case CameraMode.FOLLOW:
                CameraOnFollow();
            break;
            case CameraMode.TEST:
                CameraOnTest(InputHandler.Movement);
            break;
            case CameraMode.STANDART:
                CameraOnStand();
            break;
        }
    }
    
    private void SetCameraMode()
    {        
        if (Keyboard.current.gKey.wasPressedThisFrame)
            camDebug = !camDebug;

        if (camDebug)
            camMode = CameraMode.TEST;
        else if (cameraTarget != null && cameraTarget.activeInHierarchy)
            camMode = CameraMode.FOLLOW;
        else
            camMode = CameraMode.STANDART;
    }

    private void CameraOnFollow()
    {
        Transform camFollowTarget = cameraTarget.transform;

        camPosDesired = camFollowTarget.position +cameraOffset;
        Vector3 posSmooth = Vector3.Lerp(transform.position, camPosDesired, camFollowSpd);
        transform.position = new Vector3(posSmooth.x, posSmooth.y, posSmooth.z);
    }

    private void CameraOnStand()
    {
        Vector3 posSmooth = Vector3.Lerp(transform.position, camPosDesired, camFollowSpd);
        transform.position = new Vector3(posSmooth.x, posSmooth.y, posSmooth.z);
    }

    private void CameraOnTest(Vector2 move)
    {
        if (move == Vector2.zero)
            DebugAccSpd = 0f;
        else
            DebugAccSpd = Mathf.MoveTowards(DebugAccSpd, camTestSpd, camTestSpd /3);
        
        float spd = DebugAccSpd;

        transform.position = Vector3.Lerp(transform.position,
            transform.position + new Vector3(move.x, move.y, 0), spd);

    }
}