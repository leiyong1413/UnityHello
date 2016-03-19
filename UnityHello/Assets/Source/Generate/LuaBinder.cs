﻿using System;
using UnityEngine;
using LuaInterface;

public static class LuaBinder
{
	public static void Bind(LuaState L)
	{
		float t = Time.realtimeSinceStartup;
		L.BeginModule(null);
		DebuggerWrap.Register(L);
		UIRootWrap.Register(L);
		LuaBehaviourWrap.Register(L);
		GameResFactoryWrap.Register(L);
		LuaHelperWrap.Register(L);
		L.BeginModule("UnityEngine");
		UnityEngine_ComponentWrap.Register(L);
		UnityEngine_BehaviourWrap.Register(L);
		UnityEngine_MonoBehaviourWrap.Register(L);
		UnityEngine_GameObjectWrap.Register(L);
		UnityEngine_TransformWrap.Register(L);
		UnityEngine_SpaceWrap.Register(L);
		UnityEngine_CameraWrap.Register(L);
		UnityEngine_CameraClearFlagsWrap.Register(L);
		UnityEngine_MaterialWrap.Register(L);
		UnityEngine_ParticleEmitterWrap.Register(L);
		UnityEngine_ParticleAnimatorWrap.Register(L);
		UnityEngine_ParticleSystemWrap.Register(L);
		UnityEngine_ColliderWrap.Register(L);
		UnityEngine_BoxColliderWrap.Register(L);
		UnityEngine_CharacterControllerWrap.Register(L);
		UnityEngine_AnimationWrap.Register(L);
		UnityEngine_AnimationClipWrap.Register(L);
		UnityEngine_TrackedReferenceWrap.Register(L);
		UnityEngine_AnimationStateWrap.Register(L);
		UnityEngine_AudioClipWrap.Register(L);
		UnityEngine_AudioSourceWrap.Register(L);
		UnityEngine_ApplicationWrap.Register(L);
		UnityEngine_InputWrap.Register(L);
		UnityEngine_KeyCodeWrap.Register(L);
		UnityEngine_ScreenWrap.Register(L);
		UnityEngine_TimeWrap.Register(L);
		UnityEngine_SleepTimeoutWrap.Register(L);
		UnityEngine_AsyncOperationWrap.Register(L);
		UnityEngine_AssetBundleWrap.Register(L);
		UnityEngine_QualitySettingsWrap.Register(L);
		UnityEngine_RenderTextureWrap.Register(L);
		UnityEngine_WrapModeWrap.Register(L);
		UnityEngine_TextureWrap.Register(L);
		UnityEngine_ShaderWrap.Register(L);
		UnityEngine_Texture2DWrap.Register(L);
		UnityEngine_WWWWrap.Register(L);
		UnityEngine_RectTransformWrap.Register(L);
		UnityEngine_RectTransformUtilityWrap.Register(L);
		L.BeginModule("UI");
		UnityEngine_UI_GraphicWrap.Register(L);
		UnityEngine_UI_MaskableGraphicWrap.Register(L);
		UnityEngine_UI_ImageWrap.Register(L);
		UnityEngine_UI_SelectableWrap.Register(L);
		UnityEngine_UI_ButtonWrap.Register(L);
		L.EndModule();
		L.BeginModule("EventSystems");
		UnityEngine_EventSystems_UIBehaviourWrap.Register(L);
		L.EndModule();
		L.BeginModule("Events");
		L.RegFunction("UnityAction", UnityEngine_Events_UnityAction);
		L.EndModule();
		L.EndModule();
		L.BeginModule("System");
		L.RegFunction("Action", System_Action);
		L.EndModule();
		L.EndModule();
		Debugger.Log("Register lua type cost time: {0}", Time.realtimeSinceStartup - t);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnityEngine_Events_UnityAction(IntPtr L)
	{
		try
		{
			LuaFunction func = ToLua.CheckLuaFunction(L, 1);
			Delegate arg1 = DelegateFactory.CreateDelegate(typeof(UnityEngine.Events.UnityAction), func);
			ToLua.Push(L, arg1);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int System_Action(IntPtr L)
	{
		try
		{
			LuaFunction func = ToLua.CheckLuaFunction(L, 1);
			Delegate arg1 = DelegateFactory.CreateDelegate(typeof(System.Action), func);
			ToLua.Push(L, arg1);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

