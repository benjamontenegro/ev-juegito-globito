// using UnityEditor;
// using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

enum CameraMode { NONE, FOLLOW, FOLLOW_LOCK, TEST }

public class CameraMain : MonoBehaviour
{
    /// <summary> TAMAÑO DEL VIEWPORT (480, 256) </summary>
    public static Vector2 Size { get; } = new Vector2(30, 16);

    #region REFS ############################################################
    public Transform cameraTarget;
    // private Collider2D cameraBoundDetector;
    public LayerMask cameraBoundLayer;
    public RaycastHit2D lastCameraBound;

    public float camFollowSpd = 0.01f;
    private CameraMode camMode = CameraMode.NONE;
    
    [SerializeField] private bool camDebugToggle = false;
    private bool camDebug = false;
    private Vector3 cameraOffset = new(0, 0, -30);
    private Vector3 camPosDesired;
    
    // [SerializeField] private float FindTargetTime = 3f;
    // private float FindTargetDelay;

    // private float camTestSpd = 0;

    #endregion

    void Awake()
    {
        // GET PLAYER
        if (cameraTarget == null)
        {
            cameraTarget = GameObject.FindWithTag("Player").transform;
            // cameraTarget.TryGetComponent<Collider2D>(out cameraBoundDetector);
        }
    }

    #region EVENTOS ############################################################
    #endregion

    void Update()
    {
        SetCameraMode();
        
        switch(camMode)
        {
            default: break;
            case CameraMode.FOLLOW:
                CameraOnFollow();
            break;
            case CameraMode.FOLLOW_LOCK:
                CameraOnLockFollow();
            break;
            case CameraMode.TEST:
                CameraOnTest(InputHandler.Movement);
            break;
            case CameraMode.NONE:
                CameraOnStand();
            break;
        }
    }

    void OnDrawGizmos()
    {
        string msg_name = lastCameraBound == true ? 
            "yes, " + lastCameraBound.collider.gameObject.name : "nope";

        Handles.Label(transform.position, 
        $"CAMERA MODE {camMode}"
        + $"\nis it in zone ? {msg_name}"
        );
    }

    #region METODOS ############################################################

    private void SetCameraMode()
    {        
        if (camDebugToggle && Keyboard.current.gKey.wasPressedThisFrame)
            camDebug = !camDebug;

        if (camDebug)
        {
            camMode = CameraMode.TEST;
            return;
        }

        lastCameraBound = Physics2D.Raycast(cameraTarget.position, Vector2.zero, 0, cameraBoundLayer);
        
        if (cameraTarget != null)
        {
            camMode = (lastCameraBound == true) ? CameraMode.FOLLOW_LOCK : CameraMode.FOLLOW;
        }
        else
            camMode = CameraMode.NONE;
    }

    private void CameraOnFollow()
    {
        // Transform camFollowTarget = cameraTarget;

        camPosDesired = cameraTarget.position +cameraOffset;
        CameraMoveSmooth(transform.position, camPosDesired, camFollowSpd);
    }

    private void CameraOnStand()
    {
        CameraMoveSmooth(transform.position, camPosDesired, camFollowSpd);
    }

    private void CameraOnTest(Vector2 move)
    {
        float spd = 1 *60 *Time.deltaTime;
        camPosDesired += new Vector3(move.x *spd, move.y *spd, 0);

        CameraMoveSmooth(transform.position, camPosDesired, camFollowSpd);
    }
    
    void CameraMoveSmooth(Vector3 pos, Vector3 targetPos, float lerpSpd)
    {
        Vector3 posSmooth = Vector3.Lerp(pos, targetPos, lerpSpd);
        transform.position = new Vector3(posSmooth.x ,posSmooth.y, posSmooth.z);
    }

    private void CameraOnLockFollow()
    {
        RoomBounds _roomb;
        if (lastCameraBound.collider.gameObject.TryGetComponent<RoomBounds>(out _roomb))
        {
            Vector3 _posUnclamped = cameraTarget.position +cameraOffset;
            Vector2 _bmin = _roomb.CameraBoundMin();
            Vector2 _bmax = _roomb.CameraBoundMax();

            camPosDesired = new Vector3(
                Mathf.Clamp(_posUnclamped.x, _bmin.x, _bmax.x),
                Mathf.Clamp(_posUnclamped.y, _bmin.y, _bmax.y),
                _posUnclamped.z
            );
        }

        CameraMoveSmooth(transform.position, camPosDesired, camFollowSpd);
    }
    #endregion
}