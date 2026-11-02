using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    public Button btnCanvas;
    public Image imgCanvas;
    public TextMeshProUGUI txtCanvas, txtContador;
    public int contadorDeClic;
    public Sprite[] spritesUi;

    public string msgMostrar;

    public void CambiarTexto()
    {
        txtCanvas.text = msgMostrar;
    }
    public void CambiarImagenAleatoria()
    {
        imgCanvas.sprite = spritesUi[Random.Range(0, spritesUi.Length)];
    }
    public void AumentarContador()
    {
        contadorDeClic += 1;
        txtContador.text = contadorDeClic.ToString();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //CambiarTexto();
        //CambiarImagenAleatoria();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
