// using UnityEngine;

/// <summary> Para todos los items </summary>
public interface IEntityItem
{
    /// <summary> recolectar </summary>
    void Collect(GameEntity TriggererEntity);

    /// <summary> esta recolectado? </summary>
    bool IsCollected { get;}
    
    /// <summary> puede reaparecer? </summary>
    bool CanItemRespawn();

    /// <summary> reaparecer </summary>
    void Respawn();

    /// <summary> nombre de la entidad (GameEntity) </summary>
    string GetEntityName();
}

/// <summary> Para todos los enemigos </summary>
public interface IEntityEnemy
{
    bool CanEnemyDie();
    void Die();
    // string GetEntityName();
}