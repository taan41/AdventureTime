using System;
using System.Collections.Generic;

[Serializable]
public partial class Skill : IUpdatable
{
	public event Action<bool> OnEnabledChanged;
	public event Action OnDataChanged;

	private readonly Character owner;

	public SkillData Data { get; private set; } = null;
	public bool ValidData => Data != null && Data.IsValid();

	public StatContainerNew<SkillStat> Stats { get; private set; } = new();
	public SkillRules Rules { get; private set; } = new();
	public List<SkillPhaseData> Phases { get; private set; } = new();

	public bool Enabled { get; private set; } = false;
	public bool UseUpdate { get; private set; } = true;
	public bool UseFixedUpdate { get; private set; } = false;
	public bool UseUnscaledTime { get; private set; } = false;

	private readonly List<SkillData.ProjectileDataEntry> projectileDatas = new();
	private readonly Dictionary<SkillData.ProjectileDataEntry, ObjectPool<Projectile>> projectilePools = new();

	private Updater<Skill> updater;

	public Skill(Character owner)
	{
		if (owner == null) throw new ArgumentNullException();

		this.owner = owner;

		SubscribeToStatChanges();
	}

	public void Enable(bool enabled = true)
	{
		Enabled = enabled;

		updater ??= UpdaterManager.Instance.GetUpdater<Skill>();

		if (enabled)
		{
			updater.Register(this);
		}

		OnEnabledChanged?.Invoke(enabled);
	}

	public void SetData(SkillData newData, bool force = false)
	{
		if (Data == newData && !force) return;

		if (ValidData)
		{
			Data.OnDataChanged -= RefreshData;
		}

		Data = newData;

		if (!ValidData) return;

		RefreshData();

		Data.OnDataChanged += RefreshData;

		OnDataChanged?.Invoke();
	}

	public void DoUpdate(float deltaTime)
	{
		UpdateController(deltaTime);
	}

	public void DoFixedUpdate(float fixedDeltaTime) { }

	public void DoUnscaledUpdate(float unscaledDeltaTime) { }

	private void SubscribeToStatChanges()
	{
		// Stats[SkillStat.Cooldown].OnStatChanged += () =>
		// { UseCooldown = Stats[SkillStat.Cooldown].Final > 0; };
		// Stats[SkillStat.ManaCost].OnStatChanged += () =>
		// { UseMana = Stats[SkillStat.ManaCost].Final > 0; };
		// Stats[SkillStat.StaminaCost].OnStatChanged += () =>
		// { UseStamina = Stats[SkillStat.StaminaCost].Final > 0; };
		// Stats[SkillStat.CastTime].OnStatChanged += () =>
		// { UseCastTime = Stats[SkillStat.CastTime].Final > 0; };
	}

	private void RefreshData()
	{
		Stats.CopyFrom(Data.baseStats);
		Rules.CopyFrom(Data.rules);
		Phases.Clear();
		Phases.AddRange(Data.phaseData);

		bool newProjectileData = false;

		if (projectileDatas.Count != Data.projectileDatas.Count)
		{
			newProjectileData = true;
		}
		else
		{
			for (int i = 0; i < Data.projectileDatas.Count; i++)
			{
				var projData = Data.projectileDatas[i];
				if (projData == null || !projData.IsValid) continue;
				if (projectileDatas.Contains(projData)) continue;

				newProjectileData = true;
				break;
			}
		}

		if (!newProjectileData) return;

		projectileDatas.Clear();
		projectileDatas.AddRange(Data.projectileDatas);

		float cooldown = Stats[SkillStat.Cooldown].Final;
		foreach (var pool in projectilePools.Values)
		{
			pool.Clear();
		}

		for (int i = projectileDatas.Count - 1; i >= 0; i--)
		{
			var projData = projectileDatas[i];
			if (projData == null || !projData.IsValid)
			{
				projectileDatas.RemoveAt(i);
				continue;
			}
			
			int count = projData.specificData.count;
			if (projectilePools.ContainsKey(projData))
			{
				projectilePools[projData].Preload(count);
				continue;
			}

			projectilePools[projData] = new(() => CreateProjectile(projData), count);
		}
	}

	private Projectile CreateProjectile(SkillData.ProjectileDataEntry projData)
	{
		var proj = ProjectileFactory.Create(owner, this, projData.physicData, projData.specificData);
		proj.OnEnabledChanged += (enabled) =>
		{
			if (!enabled)
			{
				projectilePools[projData].Return(proj);
			}
		};

		return proj;
	}
}