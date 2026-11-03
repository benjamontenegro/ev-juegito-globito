using UnityEngine;

public class ItemGemMain : MonoBehaviour, IEntityItem
{
    #region REFS ############################################################

    GameEntity Ent;
    float MoveX { get => Ent.moveVel.x; set => Ent.moveVel.x = value; }
    float MoveY { get => Ent.moveVel.y; set => Ent.moveVel.y = value; }
    PlatformerStats stats;
    AudioManager _sfx;
    SpriteRenderer _spr;

    bool _isCollected = false;
    float _sprScaleTimer;

    #endregion ############################################################

    void Awake()
    {
        Ent = GetComponent<GameEntity>();
        stats = Ent.stats;
        
        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();
        _spr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // _sprScaleTimer += Time.deltaTime;

        // float yscale = Mathf.Sin(_sprScaleTimer *2f) *0.3f;

        // transform.localScale = new Vector2(1f -yscale, 1f +yscale);
    }

    public bool CanItemRespawn() => false;
    public bool IsCollected() => _isCollected;

    public void Collect(GameEntity TriggererEntity)
    {
        _isCollected = !_isCollected;
        _sfx.PlayAudioClip("coin");
        _spr.enabled = !_spr.enabled ;
        Ent.ColliderToggleEnable(!_isCollected);

        // TriggererEntity.moveVel.y = 6f;
    }

    public void Respawn() {}

    public string GetEntityName() => Ent.entityName;
}
