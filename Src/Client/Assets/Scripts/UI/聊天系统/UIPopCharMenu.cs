using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopCharMenu : UIWindow
{
    public int targetId;//目标ID
    public string targetName;//目标名字

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //检查鼠标点是否在指定的rectTransform矩形区域内
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
            {
                Close();
            }
        }
    }

    private void OnEnable()
    {
        this.transform.position = Input.mousePosition + new Vector3(80, 0, 0);//保证弹出位置在右边
    }

    public void OnClickChat()//私聊
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);//开始私聊
        Close(WindowResult.No);
    }

    public void OnClickAddFriend()
    {
        FriendService.Instance.SendFriendAddRequest(targetId, targetName);
    }

    public void OnClickInviteTeam()
    {
        TeamService.Instance.SendTeamInviteRequest(targetId, targetName);
    }


}
