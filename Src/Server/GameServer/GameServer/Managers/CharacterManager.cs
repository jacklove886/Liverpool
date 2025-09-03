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
            Characters[character.Id] = character;  
            return character; 
        }

        public void CharacterRemove(int characerId)
        {
            var cha = Characters[characerId];
            EntityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
            Characters.Remove(characerId);
        }
    }
}