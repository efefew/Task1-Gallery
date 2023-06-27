using UnityEngine;
using UnityEngine.UI;

public class ImageBlock : MonoBehaviour
{
    #region Fields

    public Button button;
    [SerializeField]
    private Image image;
    public Sprite sprite { get; set; }
    #endregion Fields
    public void SetTexture(Texture2D texture)
    {
        sprite = texture.ToSprite();
        image.sprite = sprite;
    }
}