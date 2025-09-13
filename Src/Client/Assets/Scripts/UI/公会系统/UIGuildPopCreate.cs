using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow
{

    public InputField inputName;
    public InputField inputNotice;

    public void Start()
    {
        GuildService.Instance.OnGuildCreateResult += OnGuildCreated;
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult -= OnGuildCreated;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("请输入公会名字", "错误", MessageBoxType.Error);
            return;
        }

        if(inputName.text.Length<2|| inputName.text.Length > 8)
        {
            MessageBox.Show("公会名称为2-8个字符", "错误", MessageBoxType.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputNotice.text))
        {
            MessageBox.Show("请输入公会宣言", "错误", MessageBoxType.Error);
        }

        if (inputName.text.Length < 4 || inputName.text.Length > 50)
        {
            MessageBox.Show("公会名称为4-50个字符", "错误", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildCreate(inputName.text, inputNotice.text);
    }

    private void OnGuildCreated(bool result)//等待服务器返回成功结果后才关闭
    {
        if (result)
        {
            this.Close(WindowResult.Yes);
        }
    }
}
