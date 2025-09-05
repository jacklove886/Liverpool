using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System.Collections.Generic; 


namespace GameServer.Managers
{
    public class CharacterManager:Singleton<CharacterManager>
    {
        public Dictionary<int,Character> Characters=new Dictionary<int,Character>();


        public void Clear()
        {
            Characters.Clear();
        }

        public Character CharacterAdd(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(cha.MapID, character);
            character.Info.Id = character.Id;
            Characters[character.Id] = character;  
            return character; 
        }

        public void CharacterRemove(int characerId)
        {
            //因为Disconnected也删除一次角色 所以要TrgGetValue
            if (Characters.TryGetValue(characerId, out Character cha))
            {
                EntityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
                Characters.Remove(characerId);
            }
        }
    }
}