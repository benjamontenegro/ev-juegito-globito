// using UnityEditor;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;

enum CameraMode { STANDART, FOLLOW, TEST }

public class CameraMain : MonoBehaviour
{
    public GameObject cameraTarget;
    
    public float camFollowSpd = 0.01f;
    private CameraMode camMode = CameraMode.STANDART;
    
    [SerializeField] private bool camDebugToggle = false;
    private bool camDebug = false;
    private Vector3 cameraOffset;
    private Vector3 camPosDesired;
    
    // [SerializeField] private float FindTargetTime = 3f;
    // private float FindTargetDelay;

    private float camTestSpd = 0;

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

    #region COSAS EXTRA
    //#########################################
    //#########################################
    //#########################################

    private void SetCameraMode()
    {        
        if (camDebugToggle && Keyboard.current.gKey.wasPressedThisFrame)
        {
            camDebug = !camDebug;
            // Selection.activeObject = camDebug? gameObject : null;
        }

        if (camDebug)
            camMode = CameraMode.TEST;
        else if (cameraTarget != null)
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
        camTestSpd = (move == Vector2.zero) ? 0f : Mathf.MoveTowards(camTestSpd, 2f, 0.1f);
        float spd = camTestSpd *60 *Time.deltaTime;
        camPosDesired += new Vector3(move.x *spd, move.y *spd, 0);

        Vector3 posSmooth = Vector3.Lerp(transform.position, camPosDesired, camFollowSpd);
        transform.position = new Vector3(posSmooth.x, posSmooth.y, posSmooth.z);
    }

    #endregion
}