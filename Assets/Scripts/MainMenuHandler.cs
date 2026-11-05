using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
// using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    #region REFS ############################################################

    [SerializeField] bool IsInMainMenu = true;
    // Button btnBeginGame;
    [SerializeField] string SceneNameToLoad = "SceneLevel";

    #endregion

    void Update()
    {
        if (IsInMainMenu && Keyboard.current.anyKey.isPressed)
            BeginGameSelect();
    }

    public void BeginGameSelect()
    {
        SceneManager.LoadScene(SceneNameToLoad);
        Debug.Log("PRESSBUTTON " +SceneNameToLoad);
    }

}
