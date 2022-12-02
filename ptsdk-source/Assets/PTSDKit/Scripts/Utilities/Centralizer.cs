using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PTSDKit
{
    public class Centralizer : MonoBehaviour
    {
        #region singleton management

        private static Centralizer _instance;
        public static Centralizer Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("Centralization Manager");
                    _instance = go.AddComponent<Centralizer>();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        //private static void Init()
        //{
        //    if (Instance == null)
        //    {
        //        GameObject go = new GameObject("Centralization Manager");
        //        Instance = go.AddComponent<Centralizer>();
        //    }
        //}

        
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        #region updatable action
        private class UpdateableAction
        {
            private GameObject _go;

            public GameObject go
            {
                get
                {
                    if (_go == null) _go = mono.gameObject;
                    return _go;
                }
            }
            public MonoBehaviour mono;
            public Action act;
            public float timeGap;
            public float lastTime;

            public UpdateableAction(MonoBehaviour m, Action a, float tg, float ig)
            {
                mono = m;
                act = a;
                timeGap = tg;
                if (ig <= 0)
                {
                    lastTime = Time.time;
                    a();
                }
                else
                {
                    lastTime = Time.time - tg + ig;
                }

            }
        }


        public int actCount = 0;
        private Dictionary<int, UpdateableAction> updateableActionDictionary = new Dictionary<int, UpdateableAction>();
        public static int Add_Update(MonoBehaviour mono, System.Action act, float timeGap = 0, float initialGap = 0)
        {
            if (mono == null)
            {
                Debug.LogError("Tried to add null mono behaviour on unified update manager!");
                return -1;
            }
            if (act == null)
            {
                Debug.LogError("Tried to add null action on unified update manager!");
                return -1;
            }
            Instance.updateableActionDictionary.Add(Instance.actCount, new UpdateableAction(mono, act, timeGap, initialGap));
            Instance.actCount++;
            return Instance.actCount - 1;

        }
        public static bool Remove_Update(int key)
        {
            if (Instance.updateableActionDictionary.ContainsKey(key))
            {
                Instance.updateableActionDictionary.Remove(key);
                return true;
            }
            return false;
        }


        List<int> trashList = new List<int>();
        void Update_UpdatableActions()
        {
            ////CM_DebupdateableActionDictionary.Count);
            if (trashList.Count > 0) trashList.Clear();

            foreach (var item in updateableActionDictionary)
            {
                UpdateableAction ua = item.Value;
                if (ua.mono != null)
                {
                    if (ua.mono.enabled && ua.go.activeInHierarchy && ua.act != null)
                    {
                        if (Time.time >= (ua.lastTime + ua.timeGap))
                        {
                            ua.lastTime = Time.time;
                            try
                            {
                                ua.act();
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError(e.Message);
                                trashList.Add(item.Key);
                            }
                        }
                    }
                }
                else
                {
                    trashList.Add(item.Key);
                }
            }

            foreach (var key in trashList)
            {
                updateableActionDictionary.Remove(key);
            }

        }
        #endregion

        #region delayed action
        private class DelayedAct
        {
            public Action act;
            public float time;
            public bool useRealTime;
            public bool needsMono;
            public MonoBehaviour monoRef;
            public DelayedAct(Action act, float delay, bool useRealTime, bool needsMono, MonoBehaviour monoRef)
            {
                this.monoRef = monoRef;
                this.needsMono = needsMono;
                this.useRealTime = useRealTime;
                this.act = act;
                if (this.useRealTime)
                {
                    this.time = Time.realtimeSinceStartup + delay;
                }
                else
                {
                    this.time = Time.time + delay;
                }
                if (act == null)
                {
                    Debug.LogError("act is null!");
                }
            }
        }
        private List<DelayedAct> delayedActions = new List<DelayedAct>();

        public static void Add_DelayedMonoAct(MonoBehaviour mono, System.Action act, float delay, bool useRealTime = false)
        {
            if (act == null)
            {
                Debug.LogError("Tried to add null action on delayed act manager!");
                return;
            }


            Instance.delayedActions.Add(new DelayedAct(act, delay, useRealTime, true, mono));
        }

        public static void Add_DelayedAct(System.Action act, float delay, bool useRealTime = false)
        {
            if (act == null)
            {
                Debug.LogError("Tried to add null action on delayed act manager!");
                return;
            }

            Instance.delayedActions.Add(new DelayedAct(act, delay, useRealTime, false, null));
        }

        void Update_DelayedActions()
        {
            int N = delayedActions.Count;
            for (int loopIndex = 0, removedCount = 0; loopIndex < N; loopIndex++)
            {
                int correctedIndex = loopIndex - removedCount;
                DelayedAct da = delayedActions[correctedIndex];
                bool timePassed = (da.useRealTime ? (Time.realtimeSinceStartup >= da.time) : (Time.time >= da.time));

                if (da.needsMono && !da.monoRef)
                {
                    delayedActions.RemoveAt(correctedIndex);
                    removedCount++;
                }
                else if (timePassed)
                {
                    if (da.act != null)
                    {
                        try
                        {
                            da.act();
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError(e.StackTrace);
                        }
                        //#if UNITY_EDITOR
                        //                        da.act();
                        //#else
                        //                        try
                        //                        {
                        //                            da.act();
                        //                        }
                        //                        catch(System.Exception e)
                        //                        {
                        //                            Debug.LogError(e.Message);
                        //                        }
                        //#endif


                        delayedActions.RemoveAt(correctedIndex);
                        removedCount++;
                    }
                }
            }
        }
        #endregion

        private void Update()
        {
            Update_UpdatableActions();
            Update_DelayedActions();
        }

    }
}