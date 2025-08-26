using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterMessage : MonoBehaviour {


    public SkillBridge.Message.NCharacterInfo info;
    public Text charLevel;//角色等级
    public Text charClass;//角色职业
    public Text charName;//角色姓名
    public Button charButtonCreate;//添加角色的按钮
    public Image noneImage;//空的图像 然后将角色自拍照添加进去
    public Sprite[] charSprite;//角色自拍照
    public Image highlight;
    public ScrollRect scrollRect;
    public bool selected
    {
        get { return highlight.enabled; }
        set
        {
            highlight.enabled = value; ;
        }
    }
   
    
    // Use this for initialization
    void Start () {
        UpdateUI();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    

    void UpdateUI()
    {
        if (info != null)
        {
            int index = 0;
            switch (info.Class)
            {
                case SkillBridge.Message.CharacterClass.Warrior: index = 0; break;
                case SkillBridge.Message.CharacterClass.Wizard: index = 1; break;
                case SkillBridge.Message.CharacterClass.Archer: index = 2; break;
            }
            this.charLevel.text = this.info.Level.ToString()+"级";
            this.charClass.text = this.info.Class.ToString();
            this.charName.text = this.info.Name;
            this.charButtonCreate.gameObject.SetActive(false);
            noneImage.sprite = charSprite[index];
            noneImage.gameObject.SetActive(true);
            //scrollRect.verticalNormalizedPosition = 1f;

        }
    }
    public void SetInfo(SkillBridge.Message.NCharacterInfo newInfo)
    {
        this.info = newInfo;
        UpdateUI();
    }
}
