using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EightWayDirection;
public class AnimationModule : ModuleBase
{
	public enum ClipDirectionType
	{
		EightWay,
		FourWay,
		DownOnly,
	}

	public enum FlipMode
	{
		None,
		RightAsBase,
		LeftAsBase,
	}

	[Serializable]
	public class AnimationClipData
	{
		[HideInInspector] public string clipName;
		public float frameRate = 8f;
		public ClipDirectionType directionType = ClipDirectionType.EightWay;
		public FlipMode flipMode = FlipMode.None;
		public Sprite[] Down;
		public Sprite[] Up;
		public Sprite[] Left;
		public Sprite[] Right;
		public Sprite[] LeftDown;
		public Sprite[] LeftUp;
		public Sprite[] RightDown;
		public Sprite[] RightUp;

		public AnimationClipData() { }

		public AnimationClipData(string name)
		{
			clipName = name;
		}

		public Sprite[] this[EightWayDirection direction]
		{
			get
			{
				return GetValidDirection(direction) switch
				{
					EightWayDirection.Down => Down,
					EightWayDirection.Up => Up,
					EightWayDirection.Left => Left,
					EightWayDirection.Right => Right,
					EightWayDirection.LeftDown => LeftDown,
					EightWayDirection.LeftUp => LeftUp,
					EightWayDirection.RightDown => RightDown,
					EightWayDirection.RightUp => RightUp,
					_ => null,
				};
			}
			private set
			{
				switch (direction)
				{
					case EightWayDirection.Down:
						Down = value;
						break;
					case EightWayDirection.Up:
						Up = value;
						break;
					case EightWayDirection.Left:
						Left = value;
						break;
					case EightWayDirection.Right:
						Right = value;
						break;
					case EightWayDirection.LeftDown:
						LeftDown = value;
						break;
					case EightWayDirection.LeftUp:
						LeftUp = value;
						break;
					case EightWayDirection.RightDown:
						RightDown = value;
						break;
					case EightWayDirection.RightUp:
						RightUp = value;
						break;
				}
			}
		}

		public Sprite[] this[Vector3 direction] => this[GetValidDirection(direction.ToEightWay())];

		public float GetDuration(EightWayDirection direction)
		{
			var sprites = this[direction];
			if (sprites == null || sprites.Length == 0 || frameRate <= 0f) return 0f;
			return sprites.Length / frameRate;
		}

		public bool IsFlipped(Vector3 direction) => IsFlipped(direction.ToEightWay());
		public bool IsFlipped(EightWayDirection direction)
		{
			switch (flipMode)
			{
				case FlipMode.RightAsBase:
					if (direction.IsLeft()) return true;
					break;
				case FlipMode.LeftAsBase:
					if (direction.IsRight()) return true;
					break;
			}
			return false;
		}

		public void CopyFrom(AnimationClipData other)
		{
			if (other == null) return;

			clipName = other.clipName;
			frameRate = other.frameRate;
			directionType = other.directionType;
			flipMode = other.flipMode;

			for (int i = 0; i < EightWayDirectionExtension.AllDirections.Length; i++)
			{
				var dir = EightWayDirectionExtension.AllDirections[i];
				this[dir] = new Sprite[other[dir]?.Length ?? 0];

				if (other[dir] != null)
				{
					Array.Copy(other[dir], this[dir], other[dir].Length);
				}
			}
		}

		private EightWayDirection GetValidDirection(EightWayDirection direction)
		{
			if (IsFlipped(direction))
			{
				direction = direction.FlipX();
			}
			return directionType switch
			{
				ClipDirectionType.DownOnly => EightWayDirection.Down,
				ClipDirectionType.FourWay => direction.ToFourWay(),
				_ => direction,
			};
		}
	}

	[Serializable]
	public class AnimationClip
	{
		public float frameRate = 8f;
		public Sprite[] sprites;
		public bool IsValid => sprites != null && sprites.Length > 0 && frameRate > 0f;

		public AnimationClip(Sprite[] sprites = null, float frameRate = 8f)
		{
			this.sprites = sprites;
			this.frameRate = frameRate;
		}
	}

	public event Action<object> OnOneTimeClipEnd;

	private readonly SpriteRenderer spriteRenderer;
	private Sprite defaultSprite = null;
	public Sprite[] sprites = null;
	public float frameDuration = 0f;
	private float clipDuration = 0f;
	private int currentIndex = 0;
	private int frameCount = 0;
	private float frameTimer = 0f;
	private bool loop = true;
	private bool oneTimeClip = false;
	private bool forcePlay = false;
	private object oneTimeClipToken = null;

	public AnimationModule(SpriteRenderer spriteRenderer) : base(true, false)
	{
		this.spriteRenderer = spriteRenderer;
	}

	public override void DoUpdate(float deltaTime)
	{
		if (sprites == null) return;

		if (clipDuration > 0f)
		{
			clipDuration -= deltaTime;

			if (clipDuration <= 0f)
			{
				clipDuration = 0f;
				forcePlay = false;
				if (oneTimeClip) OnOneTimeClipEnd?.Invoke(oneTimeClipToken);
			}
		}

		frameTimer += deltaTime;
		if (frameTimer >= frameDuration)
		{
			frameTimer = 0f;
			currentIndex++;
			if (currentIndex >= frameCount)
			{
				if (loop)
				{
					currentIndex = 0;
				}
				else
				{
					if (clipDuration <= 0f)
					{
						forcePlay = false;
						OnOneTimeClipEnd?.Invoke(oneTimeClipToken);
					}
					currentIndex = frameCount - 1;
				}
			}

			spriteRenderer.sprite = sprites[currentIndex];
		}
	}

	public void SetDefaultSprite(Sprite sprite)
	{
		defaultSprite = sprite;
	}

	public void SetSprite(Sprite sprite)
	{
		sprites = null;
		spriteRenderer.sprite = sprite;
	}

	public void SetFlipX(bool flip)
	{
		spriteRenderer.flipX = flip;
	}

	public bool PlayClip(AnimationClip clip, bool resetFrame = false, float duration = 0f, bool oneTime = false, bool dynamicFrameRate = false, bool force = false)
	{
		if (clip == null || !clip.IsValid) return false;
		if (!force && forcePlay) return false;

		// defaultClip ??= clip;

		// CurrentClip = clip;
		// frameCount = CurrentClip.sprites.Length;
		// frameDuration = dynamicFrameRate && duration > 0f
		// 	? duration / frameCount
		// 	: 1f / CurrentClip.frameRate;

		sprites = clip.sprites;
		frameCount = sprites.Length;
		frameDuration = dynamicFrameRate && duration > 0f
			? duration / frameCount
			: 1f / clip.frameRate;

		clipDuration = duration;
		oneTimeClip = oneTime;
		forcePlay = force;
		loop = !oneTime;

		if (resetFrame)
		{
			currentIndex = 0;
			frameTimer = 0f;
		}
		else
		{
			currentIndex %= frameCount;
		}

		spriteRenderer.sprite = sprites[currentIndex];
		return true;
	}

	public bool Play(Sprite[] sprites, float frameRate, bool resetFrameIndex = false, bool loop = true)
	{
		if (sprites == null || sprites.Length == 0)
		{
			this.sprites = null;
			spriteRenderer.sprite = defaultSprite;
			return false;
		}

		this.sprites = sprites;
		this.loop = loop;

		frameCount = this.sprites.Length;
		frameDuration = 1f / frameRate;

		if (resetFrameIndex)
		{
			currentIndex = 0;
			frameTimer = 0f;
		}
		else
		{
			currentIndex %= frameCount;
		}

		spriteRenderer.sprite = this.sprites[currentIndex];
		return true;
	}

	public bool PlayOneTimeClip(AnimationClip clip, object token = null, float duration = 0f, bool dynamicFrameRate = false, bool forcePlay = false)
	{
		if (PlayClip(clip, true, duration, true, dynamicFrameRate, forcePlay))
		{
			oneTimeClipToken = token;
			return true;
		}
		return false;
	}
}

public class AnimationModuleLite : IUpdatable
{
	public event Action<bool> OnEnabledChanged;

	private readonly SpriteRenderer spriteRenderer;
	private readonly Image image;
	private readonly bool rendererMode = true;

	// private AnimationModule.AnimationClip clip = null;
	private Sprite[] sprites = null;
	private int currentIndex = 0;
	private int frameCount = 0;
	private float frameDuration = 0f;
	private float frameTimer = 0f;

	public bool Enabled { get; private set; } = false;

	public bool UseUpdate { get; private set; } = false;

	public bool UseFixedUpdate { get; private set; } = false;

	public bool UseUnscaledTime { get; private set; } = false;

	public AnimationModuleLite(SpriteRenderer spriteRenderer, bool useUnscaled)
	{
		this.spriteRenderer = spriteRenderer;
		rendererMode = true;

		UseUpdate = !useUnscaled;
		UseUnscaledTime = useUnscaled;

		UpdaterManager.StaticRegisterLastUpdater(this);
	}

	public AnimationModuleLite(Image image, bool useUnscaled = true)
	{
		this.image = image;
		rendererMode = false;

		UseUpdate = !useUnscaled;
		UseUnscaledTime = useUnscaled;

		UpdaterManager.StaticRegisterLastUpdater(this);
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;

		OnEnabledChanged?.Invoke(enabled);
	}

	public void DoUpdate(float deltaTime)
	{
		if (sprites == null) return;

		frameTimer += deltaTime;
		if (frameTimer >= frameDuration)
		{
			frameTimer = 0f;
			currentIndex++;
			if (currentIndex >= frameCount) currentIndex = 0;

			if (rendererMode) spriteRenderer.sprite = sprites[currentIndex];
			else 
			{
				image.sprite = sprites[currentIndex];
				image.SetNativeSize();
			}
		}
	}

	public void DoFixedUpdate(float fixedDeltaTime) { }

	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		if (sprites == null) return;

		frameTimer += unscaledDeltaTime;
		if (frameTimer >= frameDuration)
		{
			frameTimer = 0f;
			currentIndex++;
			if (currentIndex >= frameCount) currentIndex = 0;

			if (rendererMode) spriteRenderer.sprite = sprites[currentIndex];
			else
			{
				image.sprite = sprites[currentIndex];
				image.SetNativeSize();
			}
		}
	}

	public void Play(Sprite[] sprites, float frameRate)
	{
		// clip = clipToPlay;
		// sprites = clip.sprites;
		// frameCount = clip.sprites.Length;
		// frameDuration = 1f / clip.frameRate;
		this.sprites = sprites;
		frameCount = sprites.Length;
		frameDuration = 1f / frameRate;
		currentIndex = 0;

		if (rendererMode) spriteRenderer.sprite = sprites[currentIndex];
		else image.sprite = sprites[currentIndex];
	}
}

#region Data Class
[Serializable]
public class AnimationData<TEnum> where TEnum : Enum
{
	private readonly Array enumArray;

	public Sprite DefaultSprite;
	public AnimationModule.AnimationClip DefaultClip;
	public bool UseAnimation = true;
	public bool UseDownOnly = false;
	public bool UseFourWayOnly = false;
	public bool UseFlip = false;
	public bool FlipAsLeft = true;

	public AnimationModule.AnimationClipData[] ClipEntries;
	public Dictionary<(TEnum type, EightWayDirection direction), AnimationModule.AnimationClip> Clips = new();

	public AnimationData()
	{
		enumArray = Enum.GetValues(typeof(TEnum));

		ClipEntries = new AnimationModule.AnimationClipData[enumArray.Length];

		for (int i = 0; i < enumArray.Length; i++)
		{
			ClipEntries[i] = new AnimationModule.AnimationClipData
			{
				clipName = enumArray.GetValue(i).ToString()
			};
		}
	}

	public bool HasClip(TEnum type, EightWayDirection direction = EightWayDirection.Down)
	{
		return Clips.ContainsKey((type, direction));
	}

	public EightWayDirection ToValidDirection(Vector3 direction)
	{
		if (UseDownOnly || direction == Vector3.zero) return Down;
		if (UseFourWayOnly) return direction.ToFourWay();
		return direction.ToEightWay();
	}

	public void ConvertToDictionary()
	{
		for (int i = 0; i < ClipEntries.Length; i++)
		{
			var entry = ClipEntries[i];
			var type = (TEnum)enumArray.GetValue(i);

			for (int dir = 0; dir < 8; dir++)
			{
				var direction = (EightWayDirection)dir;
				var sprites = entry[direction];
				ApplyToDictionaryElement(type, direction, sprites, entry.frameRate);
			}
		}
	}

	public void Clear()
	{
		DefaultSprite = null;
		DefaultClip = null;
		ClipEntries = new AnimationModule.AnimationClipData[enumArray.Length];
		for (int i = 0; i < enumArray.Length; i++)
		{
			ClipEntries[i] = new AnimationModule.AnimationClipData
			{
				clipName = enumArray.GetValue(i).ToString()
			};
		}
		Clips.Clear();
	}

	public void CopyFrom(AnimationData<TEnum> other)
	{
		if (other == null) return;

		for (int i = 0; i < other.ClipEntries.Length; i++)
		{
			ClipEntries[i].CopyFrom(other.ClipEntries[i]);
		}

		DefaultSprite = other.DefaultSprite;
		DefaultClip = other.DefaultClip;
		UseAnimation = other.UseAnimation;
		UseDownOnly = other.UseDownOnly;
		UseFourWayOnly = other.UseFourWayOnly;
		UseFlip = other.UseFlip;
		FlipAsLeft = other.FlipAsLeft;

		ConvertToDictionary();
	}

	public void OnScriptEnable()
	{
		ConvertToDictionary();
	}

	public void Validate()
	{
		if (ClipEntries.Length != enumArray.Length)
		{
			Array.Resize(ref ClipEntries, enumArray.Length);
		}

		for (int i = 0; i < enumArray.Length; i++)
		{
			if (ClipEntries[i] == null)
			{
				ClipEntries[i] = new AnimationModule.AnimationClipData
				{
					clipName = enumArray.GetValue(i).ToString()
				};
			}
			else
			{
				ClipEntries[i].clipName = enumArray.GetValue(i).ToString();
			}
		}

		ConvertToDictionary();
	}

	private void ApplyToDictionaryElement(TEnum type, EightWayDirection direction, Sprite[] entrySprites, float frameRate)
	{
		if (entrySprites != null && entrySprites.Length > 0 && frameRate > 0f)
		{
			if (Clips.ContainsKey((type, direction)))
			{
				Clips[(type, direction)].sprites = entrySprites;
				Clips[(type, direction)].frameRate = frameRate;
			}
			else
			{
				Clips.Add((type, direction), new(entrySprites, frameRate));
			}
		}
		else
		{
			if (Clips.ContainsKey((type, direction)))
			{
				Clips.Remove((type, direction));
			}
		}
	}
}
#endregion