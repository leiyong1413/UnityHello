﻿using UnityEngine;
using System.Collections;
using LuaInterface;
using System.IO;

public class LuaManager : Manager
{
    private LuaState mLuaState;

    private LuaFunction mUpdateFunction = null;
    private LuaFunction mLateUpdateFunction = null;
    private LuaFunction mFixedUpdateFunction = null;

    public LuaEvent UpdateEvent { get; private set; }

    public LuaEvent LateUpdateEvent { get; private set; }
    public LuaEvent FixedUpdateEvent { get; private set; }

    private void InitLuaLibrary()
    {
        mLuaState.OpenLibs(LuaDLL.luaopen_pb);
    }

    private void InitLuaPath()
    {
        if (GameSetting.DebugMode)
        {
            mLuaState.AddSearchPath(Application.dataPath + "/" + "Lua");
        }
        else
        {
            mLuaState.AddSearchPath(Tools.DataPath + "lua");
        }
    }

    private void Awake()
    {
        mLuaState = new LuaState();
        InitLuaLibrary();


        LuaBinder.Bind(mLuaState);
        LuaCoroutine.Register(mLuaState, this);
    }

    private void AddBundle(string bundleName)
    {
        string url = Tools.DataPath + bundleName;
        if (File.Exists(url))
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(url);
            if (bundle != null)
            {
                bundleName = bundleName.Replace("Lua/", "");
                bundleName = bundleName.Replace(".unity3d", "");
                LuaFileUtils.Instance.AddSearchBundle(bundleName.ToLower(), bundle);
            }
        }
    }

    /// <summary>
    /// 初始化LuaBundle
    /// </summary>
    void InitLuaBundle()
    {
        LuaFileUtils.Instance.beZip = GameSetting.LuaBundleMode;
        if (LuaFileUtils.Instance.beZip)
        {
            AddBundle("Lua/Lua.unity3d");
            AddBundle("Lua/Lua_math.unity3d");
            AddBundle("Lua/Lua_system.unity3d");
            AddBundle("Lua/Lua_u3d.unity3d");
            AddBundle("Lua/Lua_Common.unity3d");
            AddBundle("Lua/Lua_Logic.unity3d");
            AddBundle("Lua/Lua_View.unity3d");
            AddBundle("Lua/Lua_Controller.unity3d");
            AddBundle("Lua/Lua_Misc.unity3d");

            AddBundle("Lua/Lua_protobuf.unity3d");
            AddBundle("Lua/Lua_3rd_cjson.unity3d");
            AddBundle("Lua/Lua_3rd_luabitop.unity3d");
            AddBundle("Lua/Lua_3rd_pbc.unity3d");
            AddBundle("Lua/Lua_3rd_pblua.unity3d");
            AddBundle("Lua/Lua_3rd_sproto.unity3d");
        }
    }

    private LuaEvent GetEvent(string name)
    {
        LuaTable table = mLuaState.GetTable(name);
        LuaEvent e = new LuaEvent(table);
        table.Dispose();
        table = null;
        return e;
    }

    public void InitStart()
    {
        InitLuaPath();
        InitLuaBundle();

        mLuaState.Start();
        mLuaState.DoFile("Main.lua");

        mUpdateFunction = mLuaState.GetFunction("Update");
        mLateUpdateFunction = mLuaState.GetFunction("LateUpdate");
        mFixedUpdateFunction = mLuaState.GetFunction("FixedUpdate");

        LuaFunction main = mLuaState.GetFunction("Main");
        main.Call();
        main.Dispose();
        main = null;

        UpdateEvent = GetEvent("UpdateBeat");
        LateUpdateEvent = GetEvent("LateUpdateBeat");
        FixedUpdateEvent = GetEvent("FixedUpdateBeat");
    }

    public object[] CallFunction(string funcName, params object[] args)
    {
        LuaFunction func = mLuaState.GetFunction(funcName);
        if (func != null)
        {
            return func.Call(args);
        }
        return null;
    }

    private void Update()
    {
        if (mUpdateFunction != null)
        {
            mUpdateFunction.BeginPCall(TracePCall.Ignore);
            mUpdateFunction.Push(Time.deltaTime);
            mUpdateFunction.Push(Time.unscaledDeltaTime);
            mUpdateFunction.PCall();
            mUpdateFunction.EndPCall();
        }

        mLuaState.Collect();

#if UNITY_EDITOR
        mLuaState.CheckTop();
#endif
    }

    private void LateUpdate()
    {
        if (mLateUpdateFunction != null)
        {
            mLateUpdateFunction.BeginPCall(TracePCall.Ignore);
            mLateUpdateFunction.PCall();
            mLateUpdateFunction.EndPCall();
        }
    }

    private void FixedUpdate()
    {
        if (mFixedUpdateFunction != null)
        {
            mFixedUpdateFunction.BeginPCall(TracePCall.Ignore);
            mFixedUpdateFunction.Push(Time.fixedDeltaTime);
            mFixedUpdateFunction.PCall();
            mFixedUpdateFunction.EndPCall();
        }
    }

    public object[] DoFile(string filename)
    {
        return mLuaState.DoFile(filename);
    }

    public void LuaGC()
    {
        mLuaState.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
    }

    private void SafeRelease(ref LuaFunction luaRef)
    {
        if (luaRef != null)
        {
            luaRef.Dispose();
            luaRef = null;
        }
    }

    private void SafeRelease(ref LuaEvent luaEvent)
    {
        if (luaEvent != null)
        {
            luaEvent.Dispose();
            luaEvent = null;
        }
    }

    public LuaState GetLuaState()
    {
        return mLuaState;
    }

    public void Close()
    {
        if (mLuaState != null)
        {
            SafeRelease(ref mUpdateFunction);
            SafeRelease(ref mLateUpdateFunction);
            SafeRelease(ref mFixedUpdateFunction);

            if (UpdateEvent != null)
            {
                UpdateEvent.Dispose();
                UpdateEvent = null;
            }

            if (LateUpdateEvent != null)
            {
                LateUpdateEvent.Dispose();
                LateUpdateEvent = null;
            }

            if (FixedUpdateEvent != null)
            {
                FixedUpdateEvent.Dispose();
                FixedUpdateEvent = null;
            }

            mLuaState.Dispose();
            mLuaState = null;
        }
    }
}
