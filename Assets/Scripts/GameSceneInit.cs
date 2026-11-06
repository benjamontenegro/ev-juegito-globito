using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneInit : MonoBehaviour
{
    void Awake()
    {
            SceneManager.LoadScene("SceneMainMenu");
    }
}
