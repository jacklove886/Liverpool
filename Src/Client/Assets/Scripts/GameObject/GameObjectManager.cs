using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using SkillBridge.Message;
using Managers;
using Models;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();



    protected override void OnStart()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }


    //这句话永远不会执行
    void OnCharacterEnter(Character character)
    {
        CreateCharacterObject(character);
    }

    void OnCharacterLeave(Character character)
    {
        if (!Characters.ContainsKey(character.entityId)) return;

        if (Characters[character.entityId] != null)
        {
            Destroy(Characters[character.entityId]);
            Characters.Remove(character.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    private void CreateCharacterObject(Character character)
    {
        if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
        {
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            if (obj == null)
            {
                Debug.LogErrorFormat("角色：[{0}] 资源[{1}] 不存在.", character.Define.Name, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj, this.transform);
            go.name = character.Info.Name;
            Characters[character.entityId] = go;

            EntityController entityController = go.GetComponent<EntityController>();

            if (entityController != null)
            {
                entityController.entity = character;
                entityController.isPlayer = character.Info.Id == User.Instance.CurrentCharacter.Id;
            }
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);
            InitGameObject(Characters[character.entityId], character, entityController);
        }
    }

    //避免切换场景后人物状态出现错误
    private void InitGameObject(GameObject go,Character character, EntityController entityController)
    {
        go.transform.position = GameObjectTool.LogicToWorld(character.position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);

        PlayerInputController pc = go.GetComponent<PlayerInputController>();

        if (pc != null)
        {
            if (User.Instance.CurrentCharacter != null && character.Info.Id == User.Instance.CurrentCharacter.Id)
            {
                User.Instance.CurrentCharacterObject = go;
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = character;
                pc.entityController = entityController;
            }
            else
            {
                pc.enabled = false;
            }
        }
    }
}

