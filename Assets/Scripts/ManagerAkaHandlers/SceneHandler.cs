using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static bool PlayerIsAlive { get; private set; }
    
    private bool _playerToBeRespawned = false;
    private float _playerRespawnDelay;
    [SerializeField] private float RespawnTime = 1.5f;

    [Header("DEBUG TIME SLOW")]
    [SerializeField][Range(0f, 1f)] private float debugTimeScaleModify = 1.0f;
    private float debugDefaultFixedDeltaTime;

    void Awake()
    {
        debugDefaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        CountTimers();
        
        if (Keyboard.current.rKey.wasPressedThisFrame)
            Restart();
        
        if (Keyboard.current.mKey.wasPressedThisFrame)
            SceneManager.LoadScene("MainMenu");
        
        PlayerIsAlive = (GameObject.FindWithTag("Player") != null) ? true : false;

        Time.timeScale = debugTimeScaleModify;
        Time.fixedDeltaTime = debugDefaultFixedDeltaTime *debugTimeScaleModify;
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
