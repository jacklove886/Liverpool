using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
        }

        public void Save(bool async=false)
        {
            if (async)//异步保存
            {
                entities.SaveChangesAsync();
            }
            else//同步保存
            {
                entities.SaveChanges();
            }
        }
    }
}
