using UnityEngine;
// using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    #region REFS ############################################################

    // [SerializeField] bool IsInMainMenu = true;

    [SerializeField] string[] ScenesToSelect;
    public int SceneSelectIndex;

    [SerializeField] Text UiTxtSceneSelected;

    #endregion

    #region EVENTOS ############################################################
    #endregion

    void Start() => UpdateUiStrings();

    #region METODOS ############################################################
    #endregion

    public void UpdateUiStrings()
    {
        // if (UiTxtSceneSelected != null)
        UiTxtSceneSelected.text = GetSelectedSceneName();
    }

    public void BeginGameSelect()
    {
        SceneManager.LoadScene(GetSelectedSceneName());
    }

    public string GetSelectedSceneName() => ScenesToSelect[SceneSelectIndex];

    public int SceneSelectMaxIndex() => ScenesToSelect.Length -1;

    public void SceneAddToSelectIndex(int amount)
    {
        int _addindex = SceneSelectIndex + amount;

        if (_addindex < 0)
            _addindex = SceneSelectMaxIndex();
        if (_addindex > SceneSelectMaxIndex())
            _addindex = 0;

        SceneSelectIndex = _addindex;
        UpdateUiStrings();
    }
}
