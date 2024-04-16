using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ObserverPattern;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker.Models;
using System.Threading.Tasks;
using System;
using Unity.Netcode.Transports.UTP;
using CloudService;

namespace GlobalManager
{
    public class NetworkGameManager : MonoBehaviour
    {
        public void Start()
        {
            CloudServiceManager.Singleton.Initialize();
        }
    }
}
