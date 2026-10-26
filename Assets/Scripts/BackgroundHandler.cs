using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    [Header("Referencias uwu")]
    [SerializeField] private Transform cameraPos;
    private Vector3 lastCameraPos;

    [Header("Capas")]
    [Tooltip("Parallax por cada capa")]
    [SerializeField] private float[] parallaxFactor;

    private SpriteRenderer[] backgrounds;
    private int backgroundChilds;
    private float[] backgroundTexWidth;

    private void Start()
    {
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

    private void LateUpdate()
    {
        Vector3 deltaMove = cameraPos.position - lastCameraPos;

        for (int i = 0; i < backgroundChilds; i++)
        {
            Transform child = transform.GetChild(i);
            float parallax_y = deltaMove.y; //* parallaxFactor[i];
            float parallax_x = deltaMove.x * parallaxFactor[i];
            child.position += new Vector3(parallax_x, parallax_y);

            float distMove = cameraPos.position.x * (1 -parallaxFactor[i]);
            if (Mathf.Abs(cameraPos.position.x -child.position.x) >= backgroundTexWidth[i])
            {
                float offsetX = (cameraPos.position.x -child.position.x) % backgroundTexWidth[i];
                child.position = new Vector3(cameraPos.position.x +offsetX, child.position.y);
            }
        }

        lastCameraPos = cameraPos.position;
    }
}
