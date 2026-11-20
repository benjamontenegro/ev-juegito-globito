using System;
using UnityEngine;
// using UnityEngine.Timeline;

public class BalloonMain : MonoBehaviour
{
    #region REFS ############################################################
    [SerializeField] GameEntity Entity;
    float Xvel { get => Entity.moveVel.x; set => Entity.moveVel.x = value; }
    float Yvel { get => Entity.moveVel.y; set => Entity.moveVel.y = value; }
    AudioManager _sfx;

    [Header("Fisicas de resorte")]
    [SerializeField] float MoveSpringFactor = 2.1f;
    float XvelSpringFactorMult = 0.5f;
    [SerializeField] float MoveSpringDampening = 0.2f;
    float XvelSpringDampMult = 0.5f;
    [SerializeField] Transform posTarget;

    [Header("Ajuste horizontal")]
    bool doPosVerticalAdjust = true;
    [SerializeField] float posVerticalAdjustXMargin = 2f;
    [SerializeField] float posVerticalAdjustXMult = 0.3f;
    [SerializeField] float posVerticalAdjustXHeight = 1f;
    float posVerticalAdjustAngle = 135f;
    float posVerticalAdjust;
    [SerializeField] float posHorizontalOffset = 0f;

    [Header("weas de sprites")]
    
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Transform _spriteTransform; 

    float _sprVerticalAdjustAngle;
    Vector2 _sprScale;
    float sprAngleOffset = 90f;

    bool sprSpawnSequence = true;
    float BalloonTime = 0.0f;
    float sprSpawnSequenceTimeEnd = 0.017f *9;
    
    [SerializeField] LineRenderer balloonLine;

    #endregion ############################################################

    void Awake()
    {
        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();
    }

    void FixedUpdate()
    {
        // Entity.PreCollisions();
        MoveBalloon();
        Entity.MoveApplyVelocity();
        // Entity.GetCollisionChecks(); // ACTUALIZAR CHECKEO COLISIONES
        // Entity.PostCollisions(); // STOP SPEEDS BASED ON DETECTED COLLISIONS
    }

    void Update()
    {
        HandleSprites();
        HandleBalloonSequence();
    }

    #region METODOS ############################################################
    #endregion

    void MoveBalloon()
    {
        if (posTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 MovePosTarget = posTarget.position + new Vector3(0, posVerticalAdjust);
        Vector2 _diff = transform.position - MovePosTarget;
        
        if (doPosVerticalAdjust)
        {
            float tempAdjustHeight = !sprSpawnSequence ? posVerticalAdjustXHeight : -0.3f;
            
            bool _yDisplace = Mathf.Abs(_diff.x) <= posVerticalAdjustXMargin;
            posVerticalAdjust = _yDisplace ? Mathf.Cos(
                (_diff.x -posHorizontalOffset) *posVerticalAdjustAngle *
                Mathf.Deg2Rad *posVerticalAdjustXMult) * tempAdjustHeight : 0;
        }
        
        // Vector2 _moveDir = new( -Mathf.Sign(_diff.x), -Mathf.Sign(_diff.y));

        float _Xacc = (-(MoveSpringFactor *XvelSpringFactorMult) * _diff.x)
             -(MoveSpringDampening *XvelSpringDampMult * Xvel);
        float _Yacc = (-MoveSpringFactor * _diff.y) -(MoveSpringDampening * Yvel);

        Xvel += _Xacc;
        Yvel += _Yacc;
    }

    /// <summary> cambiar el transform objetivo del globo </summary>
    public void SetPosTarget(Transform newTarget) => posTarget = newTarget;

    /// <summary> darle un empujon al globo </summary>
    public void MoveForce(Vector2 Force)
    {
        Xvel = Force.x;
        Yvel = Force.y;
    }

    /// <summary> sprites globo y spritetransforms globo </summary>
    void HandleSprites()
    {
        // ANGULO DEL SPRITE

        float _angleToPos = GetAngleToPosition(transform.position, posTarget.position);
        _sprVerticalAdjustAngle = Mathf.Round(Mathf.LerpAngle(_angleToPos, 270f, 0.5f) /22.5f) *22.5f;
        float _currentSprEuler = _spriteTransform.eulerAngles.z;
        // float _adjustedAngleLerp = Mathf.LerpAngle(_sprVerticalAdjustAngle, 270f, 0.5f);
        _spriteTransform.Rotate(0f, 0f, _sprVerticalAdjustAngle - (_currentSprEuler - sprAngleOffset));

        // ESCALA DEL SPRITE

        if (!sprSpawnSequence) 
            _sprScale = Vector2.Lerp(_sprScale, Vector2.one, 0.25f);
        
        _spriteTransform.localScale = _sprScale;

        // LINEA/CUERDA

        float _lineanglerad = _sprVerticalAdjustAngle *Mathf.Deg2Rad ;
        Vector3 _linepos = transform.position + new Vector3(
            Mathf.Cos(_lineanglerad) *0.6f, Mathf.Sin(_lineanglerad) *0.6f, transform.position.z +2);

        balloonLine.SetPosition(0, _linepos);
        balloonLine.SetPosition(1, new Vector3(
            posTarget.position.x, posTarget.position.y, posTarget.position.z +2));
    }

    void HandleBalloonSequence()
    {
        if (BalloonTime < sprSpawnSequenceTimeEnd)
        {
            if (BalloonTime == 0)
            {
                _sprScale = new Vector2(0.3f, 0.15f);
                _spriteTransform.transform.position = 
                    _spriteTransform.transform.position + new Vector3(0, 0, -8f);
            }
            _sprScale = Vector2.Lerp(_sprScale, Vector2.one, 0.1f);
        }
        else
        {
            if (sprSpawnSequence)
            {
                sprSpawnSequence = false;
                _sprScale = new Vector2(2f, 2f);
                _spriteTransform.transform.position = 
                    new Vector3(
                    _spriteTransform.transform.position.x, 
                    _spriteTransform.transform.position.y, 0f);
            }
        }

        BalloonTime += Time.deltaTime;
    }

    /// <summary> literally. <br/> (in degrees) </summary>
    float GetAngleToPosition(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        return Mathf.Atan2(direction.y, direction.x) *Mathf.Rad2Deg;
    }

    /// <summary> reventar </summary>
    public void Die()
    {
        _sfx.PlayAudioClip("pop");
        Destroy(gameObject);
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
