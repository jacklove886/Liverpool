using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopCharMenu : UIWindow,IDeselectHandler
{
    public int targetId;//目标ID
    public string targetName;//目标名字



    public void OnDeselect(BaseEventData eventData)//取消选择事件
    {
        var ed = eventData as PointerEventData;
        if (ed.hovered.Contains(this.gameObject))//检查鼠标是否还悬停在当前窗口上
        {
            return;
        }
        Close();
    }

    private void OnEnable()
    {
        //Select()方法作用是:有这个组件的物体变为已选择状态
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);//保证弹出位置在鼠标的右边
    }

    public void OnClickChat()//私聊
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);//开始私聊
        Close(WindowResult.No);
    }

    public void OnClickAddFriend()//作业
    {
        Close(WindowResult.No);
    }

    public void OnInviteTeam()//作业
    {
        Close(WindowResult.No);
    }


}
