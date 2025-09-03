using Entities;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour {

    public Collider miniMapBoudingBox;//包装盒
    public Image minimap;//小地图
    public Image arrow;//角色箭头位置
    public Text mapName;//小地图名字

    private Transform playerTransform;//拿到角色的坐标
    private float realPlayerX;//角色世界位置X坐标
    private float realPlayerY;//角色世界位置Y坐标

    private float realMapWidth;//地图真实宽度
    private float realMapHeight;//地图真实高度

    private float pivotX; //中心点X
    private float pivotY; //中心点Y


    void Start ()
    {
        InitMap();
        realMapWidth = miniMapBoudingBox.bounds.size.x; //地图真实宽度
        realMapHeight = miniMapBoudingBox.bounds.size.z;//地图真实高度
    }

    void InitMap()
    {
        mapName.text = User.Instance.CurrentMapData.Name;
        if (this.minimap.overrideSprite == null)
        {
        minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMiniMap();// 加载当前地图的小地图的图片
        }
        minimap.SetNativeSize();// 设置小地图为原始尺寸
        minimap.transform.localPosition = Vector3.zero;// 重置小地图位置为原点

        StartCoroutine(waitTime());
        this.playerTransform = User.Instance.CurrentCharacterObject.transform;
    }
	
    IEnumerator waitTime()
    {
        yield return null;
    }
	
	void Update ()
    {
        realPlayerX = playerTransform.position.x - miniMapBoudingBox.bounds.min.x;//玩家相对地图左下角的位置
        realPlayerY = playerTransform.position.z - miniMapBoudingBox.bounds.min.z;//玩家相对地图最下面的位置

        pivotX = Mathf.Clamp01(realPlayerX / realMapWidth);//玩家在地图的X比例
        pivotY = Mathf.Clamp01(realPlayerY / realMapHeight);//玩家在地图的Y比例

        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);//玩家的位置就是地图中心点
        this.minimap.rectTransform.localPosition = Vector2.zero;//小地图位置在原点

        arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);//小箭头跟着角色旋转
    }
}
