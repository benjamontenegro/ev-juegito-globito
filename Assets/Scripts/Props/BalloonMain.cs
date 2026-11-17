using System;
using UnityEngine;
// using UnityEngine.Timeline;

public class BalloonMain : MonoBehaviour
{
    [SerializeField] GameEntity Entity;
    float Xvel { get => Entity.moveVel.x; set => Entity.moveVel.x = value; }
    float Yvel { get => Entity.moveVel.y; set => Entity.moveVel.y = value; }

    [Header("Fisicas de resorte")]
    [SerializeField] float MoveSpringFactor = 2.1f;
    [SerializeField] float XvelSpringFactorMult = 0.5f;
    [SerializeField] float MoveSpringDampening = 0.2f;
    [SerializeField] float XvelSpringDampMult = 0.5f;
    [SerializeField] Transform posTarget;

    [Header("Ajuste horizontal")]
    [SerializeField] bool doPosVerticalAdjust = true;
    [SerializeField] float posVerticalAdjustXMargin = 2f;
    [SerializeField] float posVerticalAdjustXMult = 0.3f;
    [SerializeField] float posVerticalAdjustXHeight = 1f;
    [SerializeField] float posVerticalAdjustAngle = 135f;
    float posVerticalAdjust;
    [SerializeField] float posHorizontalOffset = 0f;

    [Header("weas de sprites")]
    
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Transform _spriteTransform; 

    float _sprVerticalAdjustAngle;
    Vector2 _sprScale;
    float sprAngleOffset = 90f;
    
    [SerializeField] LineRenderer balloonLine;

    void FixedUpdate()
    {
        // Entity.PreCollisions();
        MovimientoResorte();
        Entity.MoveApplyVelocity();
        // Entity.GetCollisionChecks(); // ACTUALIZAR CHECKEO COLISIONES
        // Entity.PostCollisions(); // STOP SPEEDS BASED ON DETECTED COLLISIONS
    }

    void Update()
    {
        HandleSprites();
    }

    #region METODOS ############################################################
    #endregion

    void MovimientoResorte()
    {
        Vector3 MovePosTarget = posTarget.position + new Vector3(0, posVerticalAdjust);
        Vector2 _diff = transform.position - MovePosTarget;
        
        if (doPosVerticalAdjust)
        {
            bool _yDisplace = Mathf.Abs(_diff.x) <= posVerticalAdjustXMargin;
            posVerticalAdjust = _yDisplace ? Mathf.Cos(
                (_diff.x -posHorizontalOffset) *posVerticalAdjustAngle *
                Mathf.Deg2Rad *posVerticalAdjustXMult) * posVerticalAdjustXHeight : 0;
        }
        
        // Vector2 _moveDir = new( -Mathf.Sign(_diff.x), -Mathf.Sign(_diff.y));

        float _Xacc = (-(MoveSpringFactor *XvelSpringFactorMult) * _diff.x)
             -(MoveSpringDampening *XvelSpringDampMult * Xvel);
        float _Yacc = (-MoveSpringFactor * _diff.y) -(MoveSpringDampening * Yvel);

        Xvel += _Xacc;
        Yvel += _Yacc;
    }

    void OnDrawGizmos()
    {
        // Vector3 MovePosTarget = posTarget.position + new Vector3(0, posVerticalAdjust);
        // Vector2 _diff = transform.position - MovePosTarget;

        // Vector2 _moveDir = new( -Mathf.Sign(_diff.x), -Mathf.Sign(_diff.y));
        // Gizmos.DrawSphere(new Vector3(MovePosTarget.x, MovePosTarget.y, 0), 0.1f);
        // Gizmos.DrawLine(MovePosTarget, MovePosTarget + new Vector3(0, posVerticalAdjust));
        // Gizmos.DrawLine(MovePosTarget + new Vector3(0, posVerticalAdjust)
        // , MovePosTarget + new Vector3(posVerticalAdjustXMargin, posVerticalAdjust));

        float _angleToPos = GetAngleToPosition(transform.position, posTarget.position);
        float _angleAdjusted = Mathf.Round(_angleToPos /22.5f) *22.5f;

        Gizmos.DrawRay(transform.position, new Vector3(Xvel /4, Yvel /4));
        Gizmos.DrawRay(transform.position, 
            new Vector3(Mathf.Cos(_angleToPos *Mathf.Deg2Rad),
             Mathf.Sin(_angleToPos *Mathf.Deg2Rad)));

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, 
            new Vector3(Mathf.Cos(_angleAdjusted *Mathf.Deg2Rad),
             Mathf.Sin(_angleAdjusted *Mathf.Deg2Rad)));
            
        // Gizmos.color = Color.blue;
        // DrawArrowRay(transform.position +new Vector3(0, 0), new Vector3(0, _moveDir.y));
        // DrawArrowRay(transform.position +new Vector3(0, 0), new Vector3(_moveDir.x, 0));
    }

    void HandleSprites()
    {
        // ANGULO DEL SPRITE

        float _angleToPos = GetAngleToPosition(transform.position, posTarget.position);
        _sprVerticalAdjustAngle = Mathf.Round(Mathf.LerpAngle(_angleToPos, 270f, 0.5f) /22.5f) *22.5f;
        float _currentSprEuler = _spriteTransform.eulerAngles.z;
        // float _adjustedAngleLerp = Mathf.LerpAngle(_sprVerticalAdjustAngle, 270f, 0.5f);
        _spriteTransform.Rotate(0f, 0f, _sprVerticalAdjustAngle - (_currentSprEuler - sprAngleOffset));

        // ESCALA DEL SPRITE
        

        // _spriteTransform.localScale = _sprScale;

        // LINEA/CUERDA
        float _lineanglerad = _sprVerticalAdjustAngle *Mathf.Deg2Rad ;
        Vector3 _linepos = transform.position + new Vector3(
            Mathf.Cos(_lineanglerad) *0.6f, Mathf.Sin(_lineanglerad) *0.6f, transform.position.z +1);

        balloonLine.SetPosition(0, _linepos);
        balloonLine.SetPosition(1, new Vector3(
            posTarget.position.x, posTarget.position.y, posTarget.position.z +1));
    }

    /// <summary> literally. <br/> (in degrees) </summary>
    float GetAngleToPosition(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        return Mathf.Atan2(direction.y, direction.x) *Mathf.Rad2Deg;
    }

    // void DrawArrowRay(Vector3 origin, Vector3 direction)
    // {
    //     float _rayaim = Mathf.Atan2(direction.y, direction.x);
    //     float _aim1 = _rayaim +((135f +20f) *Mathf.Deg2Rad);
    //     float _aim2 = _rayaim +((225f s-20f)*Mathf.Deg2Rad);
    //     float _s = 0.25f;

    //     Ray _r = new Ray(origin, direction);
        
    //     Gizmos.DrawRay(origin, direction);
    //     Gizmos.DrawRay(origin +direction, new Vector3(Mathf.Cos(_aim1) *_s, Mathf.Sin(_aim1) *_s));
    //     Gizmos.DrawRay(origin +direction, new Vector3(Mathf.Cos(_aim2) *_s, Mathf.Sin(_aim2) *_s));
    // }
}
