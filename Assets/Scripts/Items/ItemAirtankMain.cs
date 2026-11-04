using UnityEngine;

public class ItemAirtankMain : MonoBehaviour, IEntityItem
{
    GameEntity Ent;
    float MoveX { get => Ent.moveVel.x; set => Ent.moveVel.x = value; }
    float MoveY { get => Ent.moveVel.y; set => Ent.moveVel.y = value; }
    PlatformerStats stats;
    AudioManager _sfx;
    SpriteRenderer _spr;

    bool _isCollected;
    float _isCollectedTimer;

    void Awake()
    {
        Ent = GetComponent<GameEntity>();
        stats = Ent.stats;
        
        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();
        _spr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_isCollected)
        {
            if (_isCollectedTimer > 0)
                _isCollectedTimer -= Time.deltaTime;
            else
            {
                Respawn();
                // _isCollectedTimer = -1f;
            }
        }
    }

    #region METODOS ############################################################

    public bool CanItemRespawn() => true;

    public void Collect(GameEntity TriggererEntity)
    {
        _isCollected = true;
        _sfx.PlayAudioClip("coin");
        _spr.enabled = false ;
        Ent.ColliderToggleEnable(false);
        // GameStatsManager.AddCoinCollected();

        _isCollectedTimer = 6f;
    }

    public string GetEntityName() => Ent.entityName;

    public bool IsCollected {get => _isCollected; }

    public void Respawn()
    {
        _isCollectedTimer = -1f;
        _isCollected = false;
        
        _spr.enabled = true ;
        Ent.ColliderToggleEnable(true);
    }


    #endregion

}
