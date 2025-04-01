using System;
using System.Collections;
using System.Net;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
[RequireComponent(typeof(Swipe))]
public class GalleryManager : MonoBehaviour
{
    #region Delegates

    public delegate void ImageFromURLHandler(Texture2D tex, object[] parameters);

    #endregion Delegates

    #region Fields

    private const int MAX_COUNT = 66;

    /// <summary>
    /// Порог прокрутки
    /// </summary>
    private const float SCROLL_THRESHOLD = 0.15f;

    private int lastID = 1, countImageLoading;

    private Swipe swipe;
    [SerializeField]
    private GameObject conteinerFullScreen;

    [SerializeField]
    private Transform conteinerScrollImages;

    [SerializeField]
    private ScrollRect scrollImages;

    [SerializeField]
    private Button backFullScreen;

    [SerializeField]
    private Image imageFullScreen;

    [SerializeField]
    private ImageBlock imageBlockPrefab;

    #endregion Fields

    #region Methods
    /// <summary>
    /// Узнаёт существует ли ссылка на Uri (не использую, так как сильно нагружает)
    /// </summary>
    /// <param name="uri">ссылка</param>
    /// <returns>Существует ли ссылка на Uri (если возращает true, то ссылка существует)</returns>
    private static bool CheckUriExists(Uri uri)
    {
        try
        {
            // Создаем объект HttpWebRequest для отправки HTTP-запроса на Uri
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "HEAD"; // Используем метод HEAD для получения только заголовков ответа, без тела

            // Получаем ответ на HTTP-запрос
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Проверяем код состояния ответа
            if (response.StatusCode == HttpStatusCode.OK)
            {
                // Ссылка существует
                return true;
            }
            else
            {
                // Ссылка не существует
                return false;
            }
        }
        catch (Exception)
        {
            // Произошла ошибка при отправке HTTP-запроса или получении ответа
            return false;
        }
    }

    private void Start()
    {
        backFullScreen.onClick.AddListener(() => SetScrollImages());
        swipe = GetComponent<Swipe>();
        swipe.OnRightSwipe -= SetScrollImages;
        swipe.OnRightSwipe += SetScrollImages;
    }

    private void Update()
    {
        //при достижении порога подгружаются новые картинки
        if (scrollImages.verticalScrollbar.value < SCROLL_THRESHOLD /*&& countImageLoading == 0*/)
        {
            AddImageBlocks(2);
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
                SetScrollImages();
        }
    }

    /// <summary>
    /// Загрузка текстуры
    /// </summary>
    /// <param name="function">функция вызываемая при успешной загрузке</param>
    /// <param name="url">ссылка на текстуру</param>
    /// <param name="parameters">дополнительные параметры</param>
    /// <returns></returns>
    private IEnumerator DownloadTexture(ImageFromURLHandler function, Uri url, object[] parameters = null)
    {
        using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            // Получить загруженный пакет ресурсов
            Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
            function?.Invoke(texture, parameters);
        }
        else
        {
            countImageLoading--;
            Debug.LogError("UnityWebRequest not success: " + uwr.error);
        }
    }

    /// <summary>
    /// Реализация ImageBlock при успешной загрузке текстуры
    /// </summary>
    /// <param name="texture">текстура</param>
    /// <param name="parameters">дополнительные параметры (в данном случае ссылка на ImageBlock)</param>
    private void OnDownloadTextureImageBlock(Texture2D texture, object[] parameters)
    {
        ImageBlock imageBlock = (ImageBlock)parameters[0];
        imageBlock.SetTexture(texture);
        imageBlock.button.onClick.AddListener(() => SetFullScreen(imageBlock));
        countImageLoading--;
    }

    /// <summary>
    /// Подгрузка новых картинок
    /// </summary>
    /// <param name="count">количество картинок</param>
    public void AddImageBlocks(int count)
    {
        for (int id = 0; id < count; id++)
        {
            Uri uri = new($"http://data.ikppbb.com/test-task-unity-data/pics/{lastID}.jpg");
            if (lastID > MAX_COUNT/* && !CheckUriExists(uri)*/)
                return;
            ImageBlock imageBlock = Instantiate(imageBlockPrefab, conteinerScrollImages);
            imageBlock.name = $"{lastID}";
            countImageLoading++;
            _ = StartCoroutine(DownloadTexture(OnDownloadTextureImageBlock, new Uri($"http://data.ikppbb.com/test-task-unity-data/pics/{lastID}.jpg"), new object[1] { imageBlock }));
            lastID++;
        }
    }

    /// <summary>
    /// Выбранное изображение отображается на весь экран
    /// </summary>
    /// <param name="imageBlock">Выбранное изображение</param>
    public void SetFullScreen(ImageBlock imageBlock)
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        conteinerFullScreen.SetActive(true);
        scrollImages.gameObject.SetActive(false);
        imageFullScreen.sprite = imageBlock.sprite;
    }

    /// <summary>
    /// Возвращает в Галерею
    /// </summary>
    public void SetScrollImages()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        conteinerFullScreen.SetActive(false);
        scrollImages.gameObject.SetActive(true);
    }

    #endregion Methods
}
