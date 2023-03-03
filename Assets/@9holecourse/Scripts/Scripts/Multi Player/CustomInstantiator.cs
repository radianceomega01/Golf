using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;

namespace Photon.Pun
{
    public class CustomInstantiator : IOnEventCallback
    {
        GameObject instantiatedPrefab;
        const byte customManualInstantiationEventCode = 0;

        public static Queue playerCache = new Queue();

        public event Action<Queue> OnEventRaised;

        public CustomInstantiator()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        ~CustomInstantiator()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, object[] specificData)
        {
            instantiatedPrefab = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
            PhotonView photonView = instantiatedPrefab.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(photonView))
            {
                object[] data = new object[]
                {
                    photonView.ViewID, specificData, position, rotation
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent(customManualInstantiationEventCode, data, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                UnityEngine.Object.Destroy(instantiatedPrefab);
            }
            return instantiatedPrefab;
        }

        public GameObject Instantiate(GameObject prefab, Transform parent, object[] specificData)
        {

            instantiatedPrefab = UnityEngine.Object.Instantiate(prefab, parent);
            PhotonView photonView = instantiatedPrefab.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(photonView))
            {
                object[] data = new object[]
                {
                    photonView.ViewID,
                    specificData
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent(customManualInstantiationEventCode, data, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");
                UnityEngine.Object.Destroy(instantiatedPrefab);
            }
            return instantiatedPrefab;
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == customManualInstantiationEventCode)
            {
                if (!playerCache.Contains(photonEvent))
                    playerCache.Enqueue(photonEvent);

                OnEventRaised?.Invoke(playerCache);
            }
        }
    }
}