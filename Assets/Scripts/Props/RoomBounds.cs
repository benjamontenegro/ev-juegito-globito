// using System.Numerics;
using UnityEditor.Timeline;
using UnityEngine;

public class RoomBounds : MonoBehaviour
{
    #region REFS ############################################################
    // Vector2 RoomSize; 

    // [SerializeField] Collider2D boundColl;

    #endregion

    #region EVENTS ############################################################
    #endregion

    // void Start()
    // {
    //     RoomSize = CalculateMinBoundSize();
    // }

    void OnDrawGizmos() => DrawSelf();

    #region METODOS ############################################################
    #endregion

    /// <summary> previz en el editor </summary>
    void DrawSelf()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        Gizmos.color = Color.greenYellow;
        Gizmos.DrawWireCube(transform.position, CalculateMinBoundSize());

        Vector3 _bmin = CameraBoundMin();
        Vector3 _bmax = CameraBoundMax();
        float _f = 0.25f;

        Gizmos.color = Color.darkGreen;
        Gizmos.DrawLine(new Vector3(_bmin.x -_f, _bmin.y), new Vector3(_bmax.x +_f, _bmin.y));
        Gizmos.DrawLine(new Vector3(_bmin.x, _bmin.y -_f), new Vector3(_bmin.x, _bmax.y +_f));
        Gizmos.DrawLine(new Vector3(_bmax.x +_f, _bmax.y), new Vector3(_bmin.x -_f, _bmax.y));
        Gizmos.DrawLine(new Vector3(_bmax.x, _bmax.y +_f), new Vector3(_bmax.x, _bmin.y -_f));
    }

    /// <summary> calcula tamaños </summary>
    Vector2 CalculateMinBoundSize()
    {
        Vector2 _minsize = CameraMain.Size;
        Vector2 _size = new Vector2(
            (transform.localScale.x >= _minsize.x) ? transform.localScale.x : _minsize.x,
            (transform.localScale.y >= _minsize.y) ? transform.localScale.y : _minsize.y
        );

        return _size;
    }
    /// <summary> LIMINES MINIMOS DEL ROOM (pa camara) </summary>
    public Vector3 CameraBoundMin()
    {
        Vector2 _boundmin = -(CalculateMinBoundSize() / 2f) +CameraMain.Size /2;
        return transform.position + new Vector3(_boundmin.x, _boundmin.y);

    }
    /// <summary> LIMITES MAXIMOS DEL ROOM (pa camara) </summary>
    public Vector3 CameraBoundMax()
    {
        Vector2 _boundmax = (CalculateMinBoundSize() / 2f) -CameraMain.Size /2;
        return transform.position + new Vector3(_boundmax.x, _boundmax.y);
    }
}
