using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class LustEyeMaterialChangeScript : NetworkBehaviour
    {
        private Material targetMaterial;
        private Renderer rd;

        public void Awake()
        {
            rd = GetComponent<Renderer>();
            targetMaterial = rd.material;
        }

        public void ChangeToAttackMode()
        {
                        
        }
    }
}
