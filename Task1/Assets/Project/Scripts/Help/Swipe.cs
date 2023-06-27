using System.Collections.Generic;

using UnityEngine;

public class Swipe : MonoBehaviour
{
    #region Events

    public event RightSwipeHandler OnRightSwipe;

    public event LeftSwipeHandler OnLeftSwipe;

    public event UpSwipeHandler OnUpSwipe;

    public event DownSwipeHandler OnDownSwipe;

    #endregion Events

    #region Delegates

    public delegate void RightSwipeHandler();

    public delegate void LeftSwipeHandler();

    public delegate void UpSwipeHandler();

    public delegate void DownSwipeHandler();

    private const int MIN_PRECENT_DRAG_DISTANCE = 20 / 100;

    #endregion Delegates

    #region Fields

    private Vector3 fp;   //������ ������� �������
    private Vector3 lp;   //��������� ������� �������
    private float dragDistance;  //����������� ��������� ��� ����������� ������
    private List<Vector3> touchPositions = new();//������ ��� ������� ������� � ������

    #endregion Fields

    #region Methods

#if UNITY_IOS
    private void Start() => dragDistance = Screen.height * MIN_PRECENT_DRAG_DISTANCE; //dragDistance ��� 20% ������ ������
    private void Update()
    {
        foreach (Touch touch in Input.touches)  //���������� ���� ��� ������������ ������ ������ ������
        {
            if (touch.phase == TouchPhase.Moved) //��������� ������� � ������, ��� ������ ��� ����������
                touchPositions.Add(touch.position);

            if (touch.phase == TouchPhase.Ended) //���������, ���� ����� ��������� � ������
            {
                fp = touchPositions[0]; //�������� ������ ������� ������� �� ������ �������
                lp = touchPositions[^1]; //������� ���������� �������

                //��������� ��������� ����������� ������ ��� 20% ������ ������
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {   //��� �����������
                    //���������, ����������� ���� ������������ ��� ��������������
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //���� �������������� �������� ������, ��� ������������ �������� ...
                        if (lp.x > fp.x)  //���� �������� ���� ������
                            OnRightSwipe?.Invoke();//����� ������
                        else
                            OnLeftSwipe?.Invoke();//����� �����
                    }
                    else
                    {   //���� ������������ �������� ������, ��� �������������� ��������
                        if (lp.y > fp.y)  //���� �������� �����
                            OnUpSwipe?.Invoke();//����� �����
                        else
                            OnDownSwipe?.Invoke();//����� ����
                    }
                }
            }
            else
            {
                //��� �����������, ��� ���������� ����������� ���������� ����� 20% �� ������ ������
            }
        }
    }
#endif
    #endregion Methods
}