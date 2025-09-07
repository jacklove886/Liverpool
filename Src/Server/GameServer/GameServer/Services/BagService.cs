using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class BagService:Singleton<BagService>
    {
        public BagService()//构造函数
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);
        }

        public void Init()
        {

        }

        private void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest request)
        {
            Character character = sender.Session.Character;//获得请求对象
            Log.InfoFormat("背包保存请求:角色:{0},Unlocked:{1}", character.Id, request.BagInfo.Unlocked);

            if (request.BagInfo != null)
            {
                character.Data.Bag.Items = request.BagInfo.Items;//更新背包数据
                DBService.Instance.Save();
            }
        }
    }
}
