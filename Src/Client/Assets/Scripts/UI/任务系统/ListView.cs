using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


//外部类ListView
public class ListView : MonoBehaviour
{
    public UnityAction<ListViewItem> onItemSelected;//发布事件订阅

    //ListViewItem类 单个Item元素
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;
        public bool Selected//单个项目的选中状态
        {
            get { return selected; }
            set
            {
                selected = value;
                OnSelected(selected);//调用方法
            }
        }
        public virtual void OnSelected(bool selected)//子类可以重写被选中后发生什么 比如颜色改变
        {

        }

        public ListView owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.selected)//没被选中
            {
                this.Selected = true;//设置为被选中
            }
            if (owner != null && owner.SelectedItem != this)//如果当前选中不是自己
            {
                owner.SelectedItem = this;//设置当前选中为自己
            }
        }
    }

    //ListViewItem列表
    List<ListViewItem> items = new List<ListViewItem>();

    private ListViewItem selectedItem = null;
    public ListViewItem SelectedItem//标记当前选中的是具体哪个Item
    {
        get { return selectedItem; }
        private set
        {
            //如果有选中的物体 并且不是当前物体
            if (selectedItem != null && selectedItem != value)
            {
                //把之前选中的物体选中状态清除
                selectedItem.Selected = false;
            }
            //选中物体设为当前物体
            selectedItem = value;
            if (onItemSelected != null)
                //执行事件
                onItemSelected.Invoke(value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);//存进列表
    }

    public void RemoveAll()
    {
        foreach (var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();//清空列表
    }
}
