using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[XLua.LuaCallCSharp]
public static class UnityExtension
{
    public static void OnClickSet(this Button button,object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        button.onClick.RemoveAllListeners();//清空所有监听事件
        button.onClick.AddListener(
            () =>
            {
                if (func != null)
                    func.Call();
            });
    }

    public static void OnValueChangedSet(this Slider slider, object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        slider.onValueChanged.RemoveAllListeners();//清空所有监听事件
        slider.onValueChanged.AddListener(
            (float value) =>
            {
                if (func != null)
                    func.Call(value);
            });
    }

}
