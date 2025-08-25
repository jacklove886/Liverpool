using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseRotation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    public Transform target;
    private float horizontal;
    private float x;
    public float xSpeed;//鼠标移动速度
    public float rotateSpeed;
    public bool isMouseInArea;
    public bool isReturning;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
        if (isMouseInArea) // 只有在区域内才响应
        {
            GetMouseInput();
        }
        if (isReturning)
        {
            ReturnToOriginal();
        }

    }

    void GetMouseInput()
    {
        horizontal = Input.GetAxis("Mouse X");
        x -= horizontal * xSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0, x, 0);
        target.rotation = rotation;
    }
    void ReturnToOriginal()
    {
        target.rotation = Quaternion.Lerp(target.rotation, Quaternion.identity, rotateSpeed * Time.deltaTime);

        // 检查是否已经接近原始旋转
        if (Quaternion.Angle(target.rotation, Quaternion.identity) < 0.1f)
        {
            target.rotation = Quaternion.identity;
            isReturning = false;
            x = 0; // 重置x值
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseInArea = true;
        isReturning = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseInArea = false;
        isReturning = true; // 开始返回动画
    }
}
