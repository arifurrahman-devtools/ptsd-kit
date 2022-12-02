using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PTSDKit;

#if PTSDK_GAME_ANALYTICS
using GameAnalyticsSDK;
#endif

#if PTSDK_ADJUST
using com.adjust.sdk;
#endif
#if PTSDK_FIREBASE
using Firebase.Analytics;
#endif

#if PTSDK_BYTEBREW
using ByteBrewSDK;
#endif
public partial class AnalyticsAssistant
{
    /// <summary>
    /// Edit this function in a way so this can return your current level number
    /// It should look something like: 
    /// get 
    /// { 
    ///     return LevelManager.CurrentLevelNumber;
    /// }
    /// </summary>
    public static int SelectedLevelNumber
    {
        get
        {
            Debug.LogError("Please edit this property to provide the current level number for your game");
            return 9999;
        }

    }
    //======================================================================================================








    //public static void ReportClick(AnalyticClickType type, int levelNumber, string param = "")
    //{

    //    string logString = $"CLICK:{type}:LEVEL_{levelNumber}";
    //    if (!string.IsNullOrEmpty(param)) logString = $"{logString}:{param}";
    //    logString.Log("FF00FF");
    //    GameAnalytics.NewDesignEvent(logString);
    //}
    //public static void ReportCollect(AnalyticCollectType type, int levelNumber, string itemID = "", int itemValue = -1)
    //{
    //    string logString = "";
    //    if (string.IsNullOrEmpty(itemID))
    //    {
    //        logString = $"COLLECT:{type}:LEVEL_{levelNumber}";
    //    }
    //    else
    //    {
    //        string item_info = $"{itemID}";
    //        if (itemValue >= 0) item_info = $"{item_info}_{itemValue}";

    //        logString = $"COLLECT:{type}:{item_info}:LEVEL_{levelNumber}";
    //    }
    //    logString.Log("00FFFF");
    //    GameAnalytics.NewDesignEvent(logString);
    //}
    static Dictionary<LifeTimeEventID, string> tokenDictionary_LifeTime;
    public static void LogLifeTimeEvent(LifeTimeEventID lifeTimeEventID)
    {
        if (tokenDictionary_LifeTime == null)//initialize token dictionary
        {
            tokenDictionary_LifeTime = new Dictionary<LifeTimeEventID, string>();
#if UNITY_ANDROID
            tokenDictionary_LifeTime.Add(LifeTimeEventID.d3_retained, "fu3cb9");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.d7_retained, "lnkruj");
            //adjust_EventToken_Dictionary.Add("watch_inter_10", "");
            //adjust_EventToken_Dictionary.Add("watch_inter_25", "");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.watch_rewarded_5, "2q6bog");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.watch_rewarded_15, "vsirg9");
            //adjust_EventToken_Dictionary.Add("click_reward_5", "");
            //adjust_EventToken_Dictionary.Add("click_reward_15", "");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.level_complete_4, "pp7sil");
            //adjust_EventToken_Dictionary.Add("completed_all_levels", "");

            tokenDictionary_LifeTime.Add(LifeTimeEventID.level_complete, "jdusn8");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.watch_interstitial, "udkeuf");
#elif UNITY_IOS
            tokenDictionary_LifeTime.Add(LifeTimeEventID.d3_retained, "2ey4tc");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.d7_retained, "xgn4qe");
            //adjust_EventToken_Dictionary.Add("watch_inter_10", "");
            //adjust_EventToken_Dictionary.Add("watch_inter_25", "");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.watch_rewarded_5, "kbfq91");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.watch_rewarded_15, "k95xbj");
            //adjust_EventToken_Dictionary.Add("click_reward_5", "");
            //adjust_EventToken_Dictionary.Add("click_reward_15", "");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.level_complete_4, "sey49f");
            //adjust_EventToken_Dictionary.Add("completed_all_levels", "");

            tokenDictionary_LifeTime.Add(LifeTimeEventID.level_complete, "dkusya");
            tokenDictionary_LifeTime.Add(LifeTimeEventID.watch_interstitial, "8vq1ls");
#endif
        }

        if (tokenDictionary_LifeTime.ContainsKey(lifeTimeEventID))
        {

#if PTSDK_ADJUST
            string adjToken = tokenDictionary_LifeTime[lifeTimeEventID];
            if (!string.IsNullOrEmpty(adjToken))
            {
                AdjustEvent adjustEvent = new AdjustEvent(adjToken);
                Adjust.trackEvent(adjustEvent);
                $"Adjust Event- {lifeTimeEventID}: {adjToken}".Log("BB00FF");
            }
#endif
        }
        else
        {
            Debug.LogError("Adjust event key missing");
        }

        switch (lifeTimeEventID)
        {

            case LifeTimeEventID.level_complete:
            case LifeTimeEventID.watch_interstitial:
                break;
            default:
#if PTSDK_FIREBASE
                if (FireBaseMan.Instance && FireBaseMan.Instance.IsReady)
                {
                    FirebaseAnalytics.LogEvent(lifeTimeEventID.ToString());
                    $"Firebase Event- {lifeTimeEventID}".Log(FireBaseMan.Instance.LogColorCode);
                }
#endif
                break;
        }


    }

}
public enum AnalyticClickType
{
    ROOM_CUSTOMIZE_BUTTON = 0,//
    SKIN_CUSTOMIZE_BUTTON = 11,//
    LEVEL_RETRY = 1,//
    HOME_BUTTON = 2,
    SKIP_BONUS_LEVEL = 3,
    SKIP_COIN_MULTIPLIER = 4,//
    LEVEL_SELECTION = 5,//
    GAME_RATING = 21,
}
public enum AnalyticCollectType
{
    FREE_MULTIPLIED_COINS = 0,//
    RV_MULTIPLIED_COINS = 1,//

    RV_ENERGY_REFILL = 11,
    RV_BONUS_LEVEL = 21,//
    RV_ROOM_CUSTOMIZE = 101,//
    COINS_ROOM_CUSTOMIZE = 102,//

    RV_SKIN_CUSTOMIZE = 201,//
    COINS_SKIN_CUSTOMIZE = 202,//
}
public enum RVPlacement
{
    test = -1,
    bonus_level = 1,
    coin_multiplier = 2,
    energy_refill = 3,
    room_customization = 10,
    skin_customization = 11,
}
public enum LifeTimeEventID
{
    d3_retained = 3,
    d7_retained = 7,


    level_complete = 20,
    watch_interstitial = 21,

    watch_rewarded_5 = 105,
    watch_rewarded_15 = 115,
    level_complete_4 = 1005,
}