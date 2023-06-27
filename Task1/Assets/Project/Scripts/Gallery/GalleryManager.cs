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
    /// ����� ���������
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
    /// ����� ���������� �� ������ �� Uri (�� ���������, ��� ��� ������ ���������)
    /// </summary>
    /// <param name="uri">������</param>
    /// <returns>���������� �� ������ �� Uri (���� ��������� true, �� ������ ����������)</returns>
    private static bool CheckUriExists(Uri uri)
    {
        try
        {
            // ������� ������ HttpWebRequest ��� �������� HTTP-������� �� Uri
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "HEAD"; // ���������� ����� HEAD ��� ��������� ������ ���������� ������, ��� ����

            // �������� ����� �� HTTP-������
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // ��������� ��� ��������� ������
            if (response.StatusCode == HttpStatusCode.OK)
            {
                // ������ ����������
                return true;
            }
            else
            {
                // ������ �� ����������
                return false;
            }
        }
        catch (Exception)
        {
            // ��������� ������ ��� �������� HTTP-������� ��� ��������� ������
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
        //��� ���������� ������ ������������ ����� ��������
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
    /// �������� ��������
    /// </summary>
    /// <param name="function">������� ���������� ��� �������� ��������</param>
    /// <param name="url">������ �� ��������</param>
    /// <param name="parameters">�������������� ���������</param>
    /// <returns></returns>
    private IEnumerator DownloadTexture(ImageFromURLHandler function, Uri url, object[] parameters = null)
    {
        using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            // �������� ����������� ����� ��������
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
    /// ���������� ImageBlock ��� �������� �������� ��������
    /// </summary>
    /// <param name="texture">��������</param>
    /// <param name="parameters">�������������� ��������� (� ������ ������ ������ �� ImageBlock)</param>
    private void OnDownloadTextureImageBlock(Texture2D texture, object[] parameters)
    {
        ImageBlock imageBlock = (ImageBlock)parameters[0];
        imageBlock.SetTexture(texture);
        imageBlock.button.onClick.AddListener(() => SetFullScreen(imageBlock));
        countImageLoading--;
    }

    /// <summary>
    /// ��������� ����� ��������
    /// </summary>
    /// <param name="count">���������� ��������</param>
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
    /// ��������� ����������� ������������ �� ���� �����
    /// </summary>
    /// <param name="imageBlock">��������� �����������</param>
    public void SetFullScreen(ImageBlock imageBlock)
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        conteinerFullScreen.SetActive(true);
        scrollImages.gameObject.SetActive(false);
        imageFullScreen.sprite = imageBlock.sprite;
    }

    /// <summary>
    /// ���������� � �������
    /// </summary>
    public void SetScrollImages()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        conteinerFullScreen.SetActive(false);
        scrollImages.gameObject.SetActive(true);
    }

    #endregion Methods
}