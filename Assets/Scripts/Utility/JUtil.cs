using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JUtil
{
    public static class JCollision
    {
        public static bool CheckLayer(int layer, int layerMask)
        {
            return (layerMask & (1 << layer)) != 0;
        }
    }
}
