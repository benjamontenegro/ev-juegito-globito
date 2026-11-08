using UnityEngine;

public class CrumbleBlockMain : MonoBehaviour
{
    #region REFS ############################################################

    [SerializeField] Collider2D _checkColl;
    float _checkCollDist = 0.125f;
    [SerializeField] LayerMask _playerLayer; //LK ASKLDHASKLHDSAHDLI

    [SerializeField] GameObject _spriteholder;
    [SerializeField] SpriteRenderer _sprite;
    bool _spriteholderExists;
    AudioManager _sfx;

    bool _CrumbleActive;
    [SerializeField] float _TimeToCrumble = 1f;
    float _CrumbleTimer = 1f; 
    // bool _IsCrumbled = false;

    #endregion

    void Awake()
    {
        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();

        _spriteholderExists = _spriteholder != null;
        _sprite = _spriteholderExists ? 
            _spriteholder.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
        
    }

    void Update()
    {
        DetectPlayer();
        CountTimers();
        Crumble();
    }

    void OnDrawGizmos ()
    {
        Vector2 _origin = new(_checkColl.bounds.center.x, _checkColl.bounds.max.y);
        Vector2 _size = new(_checkColl.bounds.size.x, _checkCollDist);
        Vector2 _offsetDir = Vector2.up;
        float _offsetDist = _checkCollDist /2;

        Gizmos.color = Color.red;
        Gizmos.DrawCube(_origin + (_offsetDir *_offsetDist), _size);
    }

    #region METHODS ############################################################

    void DetectPlayer()
    {
        if (CheckForPlayer() && !_CrumbleActive)
        {
            _CrumbleActive = true;
            _CrumbleTimer = _TimeToCrumble;
            Debug.Log($"PLAYER ON ME ({gameObject.name})");
        }
        
        if (!CheckForPlayer() && _CrumbleActive)
        {
            _CrumbleTimer = -1f;
        }
    }

    bool CheckForPlayer()
    {
        Vector2 _origin = new(_checkColl.bounds.center.x, _checkColl.bounds.max.y);
        Vector2 _size = new(_checkColl.bounds.size.x, _checkCollDist);
        Vector2 _offsetDir = Vector2.up;
        float _offsetDist = _checkCollDist /2;

        RaycastHit2D _collhit = Physics2D.BoxCast(_origin, _size, 0, _offsetDir, _offsetDist, _playerLayer);
        
        if (_collhit.collider != null)
        {
            GameEntity _entity = _collhit.collider.GetComponentInParent<GameEntity>();
            if (_entity.gameObject.CompareTag("Player") && _entity.GetOnGround())
            {
                return true;
            }
        }

        return false;
    }

    void CountTimers()
    {
        _CrumbleTimer -= Time.deltaTime;
    }

    void Crumble()
    {
        if (_CrumbleActive && _CrumbleTimer < 0)
        {
            // _IsCrumbled = true;
            _CrumbleActive = false;
            _CrumbleTimer = -1f;

            _checkColl.enabled = false;
            _sprite.enabled = false;
            _sfx.PlayAudioClip("break");
        }

        if (_spriteholderExists)
        {
            if (_CrumbleActive)
            {
                float _fval = _CrumbleTimer < 0.7f ? 0.25f : 0.125f;
                // _spriteholder.transform.position = transform.position + new Vector3(
                //     Random.Range(-_fval, _fval), Random.Range(-_fval, _fval));
                _spriteholder.transform.localScale = new Vector3(
                    1f +Random.Range(-_fval, _fval), 1f +Random.Range(-_fval, _fval));
            }
        }
    }

    #endregion
}
