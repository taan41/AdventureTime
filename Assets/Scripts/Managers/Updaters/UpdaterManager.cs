using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdaterManager : MonoBehaviour
{
    public static UpdaterManager Instance { get; private set; }
    private static readonly List<IUpdatable> firstUpdaterRegisterList = new();
    private static readonly List<IUpdatable> lastUpdaterRegisterList = new();

    public static void StaticRegisterFirstUpdater(IUpdatable updatable)
    {
        if (Instance == null)
        {
            firstUpdaterRegisterList.Add(updatable);
        }
        else
        {
            Instance.firstUpdater.Register(updatable);
        }
    }

    public static void StaticRegisterLastUpdater(IUpdatable updatable)
    {
        if (Instance == null)
        {
            lastUpdaterRegisterList.Add(updatable);
        }
        else
        {
            Instance.lastUpdater.Register(updatable);
        }
    }

    [SerializeField] private UpdaterConfig config;

    private readonly Dictionary<Type, IUpdater> updaters = new();
    private readonly Dictionary<Type, int> orders = new();
    public List<IUpdater> orderedUpdaters = new();

    private Updater<IUpdatable> firstUpdater;
    private Updater<IUpdatable> lastUpdater;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (config != null)
        {
            foreach (var entry in config.entries)
            {
                var type = Type.GetType(entry.typeName);
                if (type != null) orders[type] = entry.order;
            }

            foreach (var kvp in orders)
            {
                var type = kvp.Key;
                var order = kvp.Value;
                if (!updaters.ContainsKey(type))
                {
                    var updaterType = typeof(Updater<>).MakeGenericType(type);
                    var updater = (IUpdater)Activator.CreateInstance(updaterType);
                    if (updater != null)
                    {
                        updater.Order = order;
                        updaters[type] = updater;
                        orderedUpdaters.Add(updater);
                        orderedUpdaters.Sort((a, b) => a.Order.CompareTo(b.Order));
                    }
                }
            }
        }

        firstUpdater = new Updater<IUpdatable> { Order = int.MinValue };
        lastUpdater = new Updater<IUpdatable> { Order = int.MaxValue };

        for (int i = 0; i < firstUpdaterRegisterList.Count; i++)
        {
            firstUpdater.Register(firstUpdaterRegisterList[i]);
        }
        firstUpdaterRegisterList.Clear();

        for (int i = 0; i < lastUpdaterRegisterList.Count; i++)
        {
            lastUpdater.Register(lastUpdaterRegisterList[i]);
        }
        lastUpdaterRegisterList.Clear();
    }

    public Updater<T> GetUpdater<T>() where T : IUpdatable
    {
        var type = typeof(T);
        if (!updaters.TryGetValue(type, out var updater))
        {
            int order = orders.TryGetValue(type, out var o) ? o : 0;
            updater = new Updater<T> { Order = order };
            updaters[type] = updater;
            orderedUpdaters.Add(updater);
            orderedUpdaters.Sort((a, b) => a.Order.CompareTo(b.Order));

            orders[type] = order;
            if (config != null)
            {
                config.entries.Add(new UpdaterConfig.UpdaterOrder { typeName = type.AssemblyQualifiedName, order = order });
            }
        }
        return (Updater<T>)updater;
    }

    public Updater<IUpdatable> GetGeneralUpdater() => lastUpdater;

    void Update()
    {
        float unscaledDeltaTime = Time.unscaledDeltaTime;
        if (unscaledDeltaTime <= 0f) return;

        firstUpdater.DoUnscaledUpdate(unscaledDeltaTime);
        for (int i = 0; i < orderedUpdaters.Count; i++)
        {
            orderedUpdaters[i].DoUnscaledUpdate(unscaledDeltaTime);
        }
        lastUpdater.DoUnscaledUpdate(unscaledDeltaTime);

        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f) return;

        firstUpdater.DoUpdate(deltaTime);
        for (int i = 0; i < orderedUpdaters.Count; i++)
        {
            orderedUpdaters[i].DoUpdate(deltaTime);
        }
        lastUpdater.DoUpdate(deltaTime);
    }
    
    void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        if (fixedDeltaTime <= 0f) return;
        
        firstUpdater.DoFixedUpdate(fixedDeltaTime);
        for (int i = 0; i < orderedUpdaters.Count; i++)
        {
            orderedUpdaters[i].DoFixedUpdate(fixedDeltaTime);
        }
        lastUpdater.DoFixedUpdate(fixedDeltaTime);
    }
}
