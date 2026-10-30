using UnityEditor.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    [System.Serializable]
    public struct Parallax
    {
        [Range(0f, 1f)] public float amount;
        public float timeFactor;
    }

    [Header("Referencias uwu")]
    [SerializeField] private Transform cameraPos;
    public float BackgroundTimeMove;
    private Vector3 lastCameraPos;

    [Header("Capas")]
    [Tooltip("Parallax por cada capa")]
    [SerializeField] private Parallax[] parallaxInfo;

    private SpriteRenderer[] backgrounds;
    private int backgroundChilds;
    private float[] backgroundTexWidth;

    private void Start()
    {
        // backgroundTimeMove = 0;

        if (cameraPos == null)
            cameraPos = Camera.current.transform;
        
        lastCameraPos = cameraPos.position;

        backgroundChilds = transform.childCount;
        backgrounds = new SpriteRenderer[backgroundChilds];
        backgroundTexWidth = new float[backgroundChilds];

        for (int i = 0; i < backgroundChilds; i++)
        {
            backgrounds[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();

            Texture2D bgTex = backgrounds[i].sprite.texture;
            float bgPixel = backgrounds[i].sprite.pixelsPerUnit;
            backgroundTexWidth[i] = (bgTex.width /bgPixel) * backgrounds[i].transform.localScale.x;
        }
    }

    void Update()
    {
        BackgroundTimeMove = Time.deltaTime;
        // +1 pixel por segundo
    }

    void LateUpdate()
    {
        Vector3 deltaMove = cameraPos.position - lastCameraPos;

        for (int i = 0; i < backgroundChilds; i++)
        {
            Transform child = transform.GetChild(i);
            Parallax parallax = parallaxInfo[i];
            
            float parallaxXPos = deltaMove.x * parallax.amount;
            float parallaxYPos = deltaMove.y;
            float parallaxXBg = (BackgroundTimeMove *parallax.timeFactor) *parallax.amount;
            
            // SUMAR BACKGROUND TIME MOVE AL X POS

            child.position += new Vector3(parallaxXPos +parallaxXBg, parallaxYPos);
            
            // float distMove = cameraPos.position.x * (1 -parallax.amount);
            
            // SI EL FONDO CENTRAL YA NO ESTA EN LA CAMARA
            if (Mathf.Abs(cameraPos.position.x -child.position.x) >= backgroundTexWidth[i])
            {
                // LOOPEAR FONDO
                // CALCULAR DISTANCIA QUE EL FONDO DEBE TRASLADARSE
                float offsetX = (cameraPos.position.x -child.position.x) % backgroundTexWidth[i];
                
                // SUMAR OFFSET AL FONDO
                child.position = new Vector3(cameraPos.position.x +offsetX, child.position.y);
            }
        }

        lastCameraPos = cameraPos.position;
    }
}
