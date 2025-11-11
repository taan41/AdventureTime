using System.Collections.Generic;
using UnityEngine;

public class BookmarkManager : MonoBehaviour
{
	public RectTransform rightPanel;
	public RectTransform leftPanel;

	private readonly List<Bookmark> rightBookmarks = new();
	private readonly List<Bookmark> leftBookmarks = new();
	private Bookmark selectedBookmark;

	private void Start()
	{
		for (int i = 0; i < rightPanel.childCount; i++)
		{
			if (rightPanel.GetChild(i).TryGetComponent<Bookmark>(out var bookmark))
			{
				rightBookmarks.Add(bookmark);
				bookmark.OnClick += ChangeBookmark;
			}
		}

		for (int i = 0; i < leftPanel.childCount; i++)
		{
			if (leftPanel.GetChild(i).TryGetComponent<Bookmark>(out var bookmark))
			{
				leftBookmarks.Add(bookmark);
				bookmark.OnClick += ChangeBookmark;
			}
		}
	}

	public void ChangeBookmark(Bookmark bookmark)
	{
		if (bookmark == selectedBookmark) return;

		if (selectedBookmark != null)
		{
			selectedBookmark.ClearVisuals();
		}

		selectedBookmark = bookmark;
	}

	public void OnMenuClose()
	{
		for (int i = 0; i < rightBookmarks.Count; i++)
		{
			rightBookmarks[i].ClearVisuals();
		}

		for (int i = 0; i < leftBookmarks.Count; i++)
		{
			leftBookmarks[i].ClearVisuals();
		}

		selectedBookmark = null;
	}
}