using UnityEngine;

public class CrumbleBlockMain : MonoBehaviour
{
    Collider2D _checkColl;
    float _checkCollDist = 0.0625f;
    LayerMask _playerLayer; //LK ASKLDHASKLHDSAHDLI

    bool _isPlayerDetected;
    bool _CrumbleActive;


    void DetectPlayer()
    {
        Vector2 _origin = new Vector2(_checkColl.bounds.center.x, _checkColl.bounds.max.y);
        // RaycastHit2D _collhit = Physics2D.BoxCast(
            
        // )
        // if ()
    }
}
