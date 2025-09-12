using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour
{

    public Text title;
    public Text message;
    public Text tips;
    public Button buttonYes;
    public Button buttonNo;
    public InputField input;

    public Text buttonYesTitle;
    public Text buttonNoTitle;

    public delegate bool SubmitHandler(string inputText, out string tips);
    public event SubmitHandler OnSubmit;
    public UnityAction OnCancel;

    public string emptyTips;

    public void Init(string message, string title,MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.emptyTips = "输入不能为空";
        this.message.gameObject.SetActive(true);
        this.message.text = message;
        this.tips.text = "";
        this.OnSubmit = null;

        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = btnCancel;

        this.buttonYes.onClick.AddListener(OnClickYes);
        this.buttonNo.onClick.AddListener(OnClickNo);
    }

    void OnClickYes()
    {
        //清空提示文字
        this.tips.text = "";
        this.message.gameObject.SetActive(false);
        if (string.IsNullOrEmpty(input.text))
        {
            //显示空输入提示
            this.tips.text = this.emptyTips;
            return;
        }
        //如果有事件订阅
        if (OnSubmit != null)
        {
            string tips;
            //验证失败 提示信息
            if(!OnSubmit(input.text,out tips))
            {
                this.tips.text = tips;
                return;
            }
        }
        //验证成功 销毁对话框
        Destroy(this.gameObject);
    }

    void OnClickNo()
    {
        Destroy(this.gameObject);
        if (this.OnCancel != null)
            this.OnCancel();
    }
}
