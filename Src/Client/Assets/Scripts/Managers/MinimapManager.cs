using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class MinimapManager:Singleton<MinimapManager>
    {
        public UIMiniMap minimap;

        private Collider minimapBoundingBox;
        public Collider MinimapBoundingBox
        {
            get{return minimapBoundingBox;}
        }   

        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterPlayerInput == null)
                    return null;
                return User.Instance.CurrentCharacterPlayerInput.transform;
            }
        }

        public Sprite LoadCurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }

        public void UpdateMinimap(Collider minimapBoundingBox)
        {
            this.minimapBoundingBox = minimapBoundingBox;
            if (minimap != null)
            {
                minimap.UpdateMap();
            }
        }
    }
}
