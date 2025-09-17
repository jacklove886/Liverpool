using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SoundDefine//配置文件
{
    [Header("背景音乐")]
    public const string Music_Login = "login";
    public const string Music_Select = "selectCharacter";
    public const string Music_Town = "town";
    public const string Music_Map01 = "map01";
    public const string Music_Map02 = "map02";
    public const string Music_Map03 = "map03";


    [Header("UI音效")]
    public const string Info = "info";
    public const string Accept = "accept";
    public const string Error = "error";
    public const string Win = "win";
    public const string Show = "show";
    public const string Fail = "fail";
    public const string LevelUp = "levelUp";
    public const string Click = "click";
    public const string Close = "close";

    [Header("角色持续音效")]
    public static string[] CharacterWalk = { "战士走路", "法师走路", "游侠走路" };
    public static string[] CharacterRun = { "战士跑步", "法师跑步", "游侠跑步" };


    [Header("角色短暂音效")]
    public static string[] CharacterBuild = { "战士创建", "法师创建", "游侠创建" };
    public static string[] CharacterSelect = { "战士选择", "法师选择", "游侠选择" };
    public static string[] CharacterJump = { "战士跳跃" , "法师跳跃" , "游侠跳跃" };    

}
