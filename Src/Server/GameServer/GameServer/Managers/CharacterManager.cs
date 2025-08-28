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

        public Character CharacterEnterGame(TCharacter databaseCharacter)
        {
            Character gameCharacter = new Character(CharacterType.Player,databaseCharacter);
            Characters[databaseCharacter.ID] = gameCharacter;  
            return gameCharacter; 
        }

        public void CharacterRemove(int characerId)
        {
            Characters.Remove(characerId);
        }
    }
}