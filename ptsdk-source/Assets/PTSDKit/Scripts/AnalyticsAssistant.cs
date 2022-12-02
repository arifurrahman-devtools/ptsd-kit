using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PTSDKit;
using System;

#if PTSDK_GAME_ANALYTICS
using GameAnalyticsSDK;
#endif



#if PTSDK_BYTEBREW
using ByteBrewSDK;
#endif
public partial class AnalyticsAssistant : MonoBehaviour , IEarlyInitiatable
{
    public static AnalyticsAssistant Instance { get; private set; }
    public string LogColorCode => "FF00FF";

    public bool IsReady { get; set; }

    bool logDisabled = false;
    void Log(string str)
    {
        if (!logDisabled) str.Log(LogColorCode);
    }
    void IEarlyInitiatable.ForceDisableLogs()
    {
        logDisabled = true;
    }
    Action<IEarlyInitiatable> onModuleReadyToUse;
    void IEarlyInitiatable.InitializeSuperEarly(bool hasConsent, Action<IEarlyInitiatable> onModuleReadyToUse)
    {
        IsReady = true;
        this.onModuleReadyToUse = onModuleReadyToUse;

        Log("Analytics Assistant Initialized1");
#if PTSDK_GAME_ANALYTICS      
        if (GameAnalyticsMan.Instance.IsReady)
        {
            OnGAReady();
        }
        else
        {
            GameAnalyticsMan.onAnalyticsReady += OnGAReady;
        }
#else
            OnGAReady();
#endif

    }
    void OnGAReady()
    {

        Instance = this;
        Log("Analytics Assistant Initialized");
        onModuleReadyToUse?.Invoke(this);
    }

#if PTSDK_GAME_ANALYTICS || PTSDK_BYTEBREW
    [Header("Enable Setting for predefined Logs")]
#if PTSDK_GAME_ANALYTICS
    public bool gameAnalytics_logsEnabled = true;
#endif


#if PTSDK_BYTEBREW
    public bool byteBrew_logsEnabled = true;
#endif
#endif


    public void LevelStarted()
    {
        Log($"started {SelectedLevelNumber}");
#if PTSDK_BYTEBREW
        if (byteBrew_logsEnabled)
        {
            ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Started, "level", SelectedLevelNumber.ToString());
            ByteBrew.NewCustomEvent("LevelStarted", $"level={SelectedLevelNumber};");
        }
#endif

#if PTSDK_GAME_ANALYTICS
        if (gameAnalytics_logsEnabled) GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, SelectedLevelNumber.ToString());
#endif
    }
    public void LevelCompleted()
    {
        Log($"completed {SelectedLevelNumber}");
#if PTSDK_BYTEBREW
        if (byteBrew_logsEnabled)
        {
            ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Completed, "level", SelectedLevelNumber.ToString());
            ByteBrew.NewCustomEvent("LevelCompleted", $"level={SelectedLevelNumber};");
        }
#endif

#if PTSDK_GAME_ANALYTICS
        if (gameAnalytics_logsEnabled) GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, SelectedLevelNumber.ToString());
#endif
    }
    public void LevelFailed()
    {
        Log($"failed {SelectedLevelNumber}");
#if PTSDK_BYTEBREW
        if (byteBrew_logsEnabled)
        {
            ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Failed, "level", SelectedLevelNumber.ToString());
            ByteBrew.NewCustomEvent("LevelFailed", $"level={SelectedLevelNumber};");
        }
#endif

#if PTSDK_GAME_ANALYTICS
        if (gameAnalytics_logsEnabled) GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, SelectedLevelNumber.ToString());
#endif
    }
    public void LevelRestarted()
    {
        Log($"restarted {SelectedLevelNumber}");
#if PTSDK_BYTEBREW
        if (byteBrew_logsEnabled)
        {
            ByteBrew.NewCustomEvent("Restart", $"Level_{SelectedLevelNumber}");
            ByteBrew.NewCustomEvent("LevellRestart", $"level={SelectedLevelNumber};");
        }
#endif

#if PTSDK_GAME_ANALYTICS
        if (gameAnalytics_logsEnabled) GameAnalytics.NewDesignEvent($"Restart:Level_{SelectedLevelNumber}");
#endif
    }

    


    

}



