using UnityEngine;
using UnityEngine.UI;
// using TMPro;
using UnityEngine.InputSystem;

public class GameUiManager : MonoBehaviour
{
    // public Button btnCanvas;
    // public Image imgCanvas;
    // public TextMeshProUGUI txtCanvas, txtContador;
    // public int contadorDeClic;
    // public Sprite[] spritesUi;
    
    #region VARIABLES ############################################################
    
    [SerializeField] Canvas uiCanvas;

    // COIN TALLY
    GameObject coinTally;
    Text coinTallyText;
    Image coinTallyIcon;

    [SerializeField] 
    Vector2 coinTallyPosCurrent;
    [SerializeField] 
    float coinTallyPosCurrentLerp;
    [SerializeField] 
    bool coinTallyIsShowing;
    [SerializeField] 
    float coinTallyMoveTimer;
    [SerializeField] 
    int coinTallyCoinsPrev;

    #endregion

    void Awake()
    {
        uiCanvas = GetComponentInChildren<Canvas>();
        coinTally = uiCanvas.transform.Find("cointally").gameObject;

        coinTallyText = coinTally.transform.Find("cointallytxt").GetComponent<Text>();
        coinTallyIcon = coinTally.transform.Find("cointallyicon").GetComponent<Image>();
    }

    void Update()
    {
        UiPositionsCalculate();
        UiElementsUpdate();
    }

    #region METODOS ############################################################

    void UiPositionsCalculate()
    {
        coinTallyMoveTimer -= Time.deltaTime;
        if (coinTallyCoinsPrev != GameStatsManager.CoinsCollected)
        {
            coinTallyCoinsPrev = GameStatsManager.CoinsCollected;
            coinTallyMoveTimer = 3f;
        }
        if (Keyboard.current.tabKey.isPressed)
            coinTallyMoveTimer = Mathf.Max(coinTallyMoveTimer, 1.5f);

        coinTallyIsShowing = coinTallyMoveTimer >= 0;

        coinTallyPosCurrentLerp = Mathf.Lerp(coinTallyPosCurrentLerp, coinTallyIsShowing ? 0 : 1, 0.08f);
        coinTallyPosCurrent = Vector2.Lerp( Vector2.zero,
            new Vector2(-128f, 256), coinTallyPosCurrentLerp);
    }
    
    void UiElementsUpdate()
    {
        coinTally.transform.position = new Vector2(coinTallyPosCurrent.x, coinTally.transform.position.y);
        coinTallyText.text = $"{GameStatsManager.CoinsCollected}";
    }

    #endregion

}
