using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static bool PlayerIsAlive { get; private set; }
    
    private bool _playerToBeRespawned = false;
    private float _playerRespawnDelay;
    [SerializeField] private float RespawnTime = 0.5f;
    
    void Update()
    {
        CountTimers();
        
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
            TriggerPlayerRespawn();
        
        PlayerIsAlive = (GameObject.FindWithTag("Player") != null) ? true : false;
    }

    #region EXTRA SHIT
    //####################################################
    //####################################################
    //####################################################

    private void CountTimers()
    {
        if (_playerToBeRespawned)
        {
            if (_playerRespawnDelay > 0f)
            {
                _playerRespawnDelay -= Time.deltaTime;
            }
            else
            {
                _playerToBeRespawned = false;
                _playerRespawnDelay = -1f;
                RespawnPlayer();
            }
        }
    }

    public void TriggerPlayerRespawn()
    {
        if (!_playerToBeRespawned)
        {
            _playerRespawnDelay = RespawnTime;
            _playerToBeRespawned = true;
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RespawnPlayer()
    {
        Restart();
        // CAMBIAR DSPS A *INSTANCIAR JUGADOR EN X POS*
    }

    #endregion
}
