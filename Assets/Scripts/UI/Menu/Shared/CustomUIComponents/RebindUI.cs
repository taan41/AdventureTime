using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
	[SerializeField] private Text labelText;
	[SerializeField] private CustomButton rebindButton;

	private InputAction action;
	private int bindingIndex;

	private void Awake()
	{
		rebindButton.OnClicked += StartRebind;
	}
	
	public void SetupRebind(InputAction action, int bindingIndex)
	{
		this.action = action;
		this.bindingIndex = bindingIndex;

		var savedPath = PlayerPrefs.GetString($"Rebind_{action.name}_{bindingIndex}", null);
		if (!string.IsNullOrEmpty(savedPath))
		{
			action.ApplyBindingOverride(bindingIndex, savedPath);
		}
		RefreshVisuals();
	}

    public void StartRebind()
    {
        StartCoroutine(DoRebind());
    }

	private IEnumerator DoRebind()
	{
		if (action == null) yield break;

		rebindButton.Text.text = "Press a key...";
		action.actionMap.Disable();

		var oldPath = action.bindings[bindingIndex].effectivePath;

		var rebind = action.PerformInteractiveRebinding(bindingIndex)
			.WithControlsExcluding("<Mouse>/position")
			.WithControlsExcluding("<Mouse>/delta")
			.WithControlsExcluding("<Pointer>/position")
			.WithControlsExcluding("<Pointer>/delta")
			.WithCancelingThrough("<Keyboard>/escape")
			.OnCancel(op =>
			{
				action.ApplyBindingOverride(bindingIndex, oldPath);
				RefreshVisuals(false);
				action.actionMap.Enable();
				op.Dispose();
			})
			.OnComplete(op =>
			{
				var path = op.selectedControl.path;
				action.ApplyBindingOverride(bindingIndex, path);
				RefreshVisuals(false);
				action.actionMap.Enable();
				PlayerPrefs.SetString($"Rebind_{action.name}_{bindingIndex}", path);
				PlayerPrefs.Save();
				op.Dispose();
			});

		rebind.Start();
		yield return null;
	}

	private void RefreshVisuals(bool refreshLabel = true)
	{
		if (action == null) return;

		if (refreshLabel)
		{
			if (action.bindings[bindingIndex].isPartOfComposite)
			{
				labelText.text = $"{action.name} {action.bindings[bindingIndex].name}";
			}
			else
			{
				labelText.text = action.name;
			}
		}

		rebindButton.Text.text = InputControlPath.ToHumanReadableString(
			action.bindings[bindingIndex].effectivePath,
			InputControlPath.HumanReadableStringOptions.OmitDevice
			| InputControlPath.HumanReadableStringOptions.UseShortNames);
	}
}