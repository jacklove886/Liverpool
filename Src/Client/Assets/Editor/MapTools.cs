using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using Common.Data;
using SkillBridge.Message;

public class MapTools{

    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }

        foreach(var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("场景{0}不存在", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
            foreach(var teleporter in teleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图:{0}中配置的Teleporter:[{1}]中不存在", map.Value, teleporter.ID), "确定");
                    return;
                }

                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图:{0}中配置的Teleporter:[{1}] MapID:{2} 错误", map.Value, teleporter.ID,def.MapID), "确定");
                    return;
                }

                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                // 根据传送门ID设置特定方向
                if (def.LinkTo == 0) // 出口传送门
                {
                    switch (teleporter.ID)
                    {
                        case 2: // 落日森林->利物浦
                        case 8: // 利物浦->落日森林
                            def.Direction = new NVector3() { X = 0, Y = 100, Z = 0 }; 
                            break;
                        case 4: // 苍龙山脉->利物浦
                        case 10: // 利物浦->苍龙山脉
                            def.Direction = new NVector3() { X = 0, Y = 100, Z = 0 }; 
                            break;
                        case 6: // 失落神殿->利物浦
                        case 12: // 利物浦->失落神殿
                            def.Direction = new NVector3() { X = 0, Y = -100, Z = 0 }; 
                            break;
                        default:
                            def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
                            break;
                    }
                }
                else // 入口传送门
                {
                    def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
                }
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");

    }
	
}
