using System;
using System.Collections.Generic;
using UnityEngine;
#if PTSDK_IAP
using UnityEngine.Purchasing;
#endif
namespace PTSDKit
{
#if PTSDK_IAP
    [Serializable]
    public class PurchaseablePTSDItem
    {
        public static LogLevel ActiveLogLevel;
        public static string LogColorCode;

        IStoreController controller;
        [SerializeField][HideInInspector] bool dirtyID = false;
        [SerializeField] bool disableAutoGeneration= false;
        public string title;
        public IDKeeper productID;
#if PTSDK_IAP_VALIDATION
        public IDKeeper adjustEventToken;
#endif
        [HideInInspector] public int typeID;
        [NonSerialized] public bool isStoreSynced = false;

        protected Action nonConsumableItemAcquired;

        public void OnStoreInit(IStoreController controller)
        {
            
            this.controller = controller;
            isStoreSynced = true;
        }

        Action<bool> onPurchaseConfirmed;
        public void RequestPurchase(Action<bool> onPurchaseConfirmed)
        {
            if (!isStoreSynced)
            {
                Debug.LogError("product is not initialized yet");
            }
            else
            {
                InAppPurchaseMan.shield.SetActive(true);
                this.onPurchaseConfirmed = onPurchaseConfirmed;
                controller.InitiatePurchase(productID);
            }
        }

        public void ReportPurchaseResult(bool success)
        {
            if(success)HandlePurchaseConfirmed(onPurchaseConfirmed!=null);
            onPurchaseConfirmed?.Invoke(success);
            onPurchaseConfirmed = null;
        }
        protected virtual void HandlePurchaseConfirmed(bool playerWaiting)//overriden
        {
        }


        public virtual void ItemInit(ConfigurationBuilder builder)
        {
            throw new Exception("Please Override");
        }

        public virtual void OnItemAvailable(Action callback)
        {
            nonConsumableItemAcquired += callback;
        }
    }

    [Serializable]
    public class NonConsumablePTSDItem : PurchaseablePTSDItem
    {
        protected override void HandlePurchaseConfirmed(bool playerWaiting)
        {
            base.HandlePurchaseConfirmed(playerWaiting);
            if (isAvailable)
            {
                if ((int)ActiveLogLevel >= (int)LogLevel.Important) "Same product recieved twice".Log(LogColorCode);
                return;
            }
            hasPurchased.value = true;
            nonConsumableItemAcquired?.Invoke();
        }

        public override void OnItemAvailable(Action callback)
        {
            if (isAvailable)
            {
                callback?.Invoke();
            }
            else
            {
                nonConsumableItemAcquired += callback;
            }
        }

        public bool isAvailable { get { return hasPurchased.value; } }

        HardData<bool> hasPurchased;

        public override void ItemInit(ConfigurationBuilder builder)
        {
            builder.AddProduct(productID, ProductType.NonConsumable);
            hasPurchased = new HardData<bool>(string.Format("IAP-{0}", productID), false);
        }


    }

    [Serializable]
    public class ConsumablePTSDItem : PurchaseablePTSDItem
    {
        public int RewardPerPurchase;
        protected override void HandlePurchaseConfirmed(bool playerWaiting)
        {
            base.HandlePurchaseConfirmed(playerWaiting);
            if(!playerWaiting)unclaimedCount.value+=RewardPerPurchase;
        }

        public int unclaimedRewardAmount { get { return unclaimedCount.value; } }

        public int Claim()
        {
            int val = unclaimedRewardAmount;
            unclaimedCount.value = 0;
            return val ;
        }
        HardData<int> unclaimedCount;
        public override void ItemInit(ConfigurationBuilder builder)
        {
            builder.AddProduct(productID, ProductType.Consumable);
            unclaimedCount = new HardData<int>(string.Format("IAP-{0}", productID), 0);
        }
    }
#endif
    }