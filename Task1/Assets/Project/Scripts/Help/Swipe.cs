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

    private Vector3 fp;   //Первая позиция касания
    private Vector3 lp;   //Последняя позиция касания
    private float dragDistance;  //Минимальная дистанция для определения свайпа
    private List<Vector3> touchPositions = new();//Храним все позиции касания в списке

    #endregion Fields

    #region Methods

#if UNITY_IOS
    private void Start() => dragDistance = Screen.height * MIN_PRECENT_DRAG_DISTANCE; //dragDistance это 20% высоты экрана
    private void Update()
    {
        foreach (Touch touch in Input.touches)  //используем цикл для отслеживания больше одного свайпа
        {
            if (touch.phase == TouchPhase.Moved) //добавляем касания в список, как только они определены
                touchPositions.Add(touch.position);

            if (touch.phase == TouchPhase.Ended) //проверяем, если палец убирается с экрана
            {
                fp = touchPositions[0]; //получаем первую позицию касания из списка касаний
                lp = touchPositions[^1]; //позиция последнего касания

                //проверяем дистанцию перемещения больше чем 20% высоты экрана
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {   //это перемещение
                    //проверяем, перемещение было вертикальным или горизонтальным
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //Если горизонтальное движение больше, чем вертикальное движение ...
                        if (lp.x > fp.x)  //Если движение было вправо
                            OnRightSwipe?.Invoke();//Свайп вправо
                        else
                            OnLeftSwipe?.Invoke();//Свайп влево
                    }
                    else
                    {   //Если вертикальное движение больше, чнм горизонтальное движение
                        if (lp.y > fp.y)  //Если движение вверх
                            OnUpSwipe?.Invoke();//Свайп вверх
                        else
                            OnDownSwipe?.Invoke();//Свайп вниз
                    }
                }
            }
            else
            {
                //Это ответвление, как расстояние перемещения составляет менее 20% от высоты экрана
            }
        }
    }
#endif
    #endregion Methods
}