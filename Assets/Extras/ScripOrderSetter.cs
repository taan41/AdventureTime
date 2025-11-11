#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ScriptOrderSetter
{
	static readonly Dictionary<System.Type, int> scriptOrders = new();
	
    static ScriptOrderSetter()
    {
        scriptOrders[typeof(CameraManager)] = -1;
        
        scriptOrders[typeof(FloatingUIObjectManager)] = 1;
        scriptOrders[typeof(PickupManager)] = 1;
        scriptOrders[typeof(CursorOverlay)] = 1;
        scriptOrders[typeof(ItemTooltip)] = 1;
        scriptOrders[typeof(ItemTooltipManager)] = 1;

        scriptOrders[typeof(HeroParty)] = scriptOrders[typeof(FloatingUIObjectManager)] + 1;
        scriptOrders[typeof(SpawnerNew)] = scriptOrders[typeof(FloatingUIObjectManager)] + 1;

        scriptOrders[typeof(InfoOverlayManager)] = scriptOrders[typeof(HeroParty)] + 1;
        scriptOrders[typeof(MenuManager)] = scriptOrders[typeof(HeroParty)] + 1;

		foreach (var kvp in scriptOrders)
		{
			SetScriptOrder(kvp.Key, kvp.Value);
		}
	}

    static void SetScriptOrder(System.Type type, int order)
    {
        var mono = MonoImporter.GetAllRuntimeMonoScripts()
            .FirstOrDefault(m => m != null && m.GetClass() == type);
        if (mono == null) return;
        if (MonoImporter.GetExecutionOrder(mono) != order)
        {
            MonoImporter.SetExecutionOrder(mono, order);
            Debug.Log($"Set {type.Name} script order to {order}");
        }
    }
}
#endif
