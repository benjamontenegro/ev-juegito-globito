using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneInit : MonoBehaviour
{
    [SerializeField] Scene SceneToLoad;

    void Awake()
    {
        SceneManager.LoadScene(SceneToLoad.buildIndex);
            // SceneManager.LoadScene("SceneMainMenu");
    }
}
