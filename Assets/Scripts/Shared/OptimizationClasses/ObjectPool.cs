using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
	// public static Dictionary<Type, ObjectPool<Type>> Pools = new();
	private readonly Queue<T> pool = new();
	private Func<T> createFunc;
	private readonly bool fixedSize;

	public int Count => pool.Count;

	public ObjectPool(Func<T> createFunc, int initialSize = 0, bool fixedSize = false)
	{
		this.createFunc = createFunc;
		this.fixedSize = fixedSize;
		if (initialSize > 0)
		{
			Preload(initialSize);
		}
	}

	public void SetCreateFunction(Func<T> createFunc)
	{
		this.createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
	}

	public void Preload(int count)
	{
		for (int i = 0; i < count; i++)
		{
			pool.Enqueue(createFunc());
		}
	}

	public T Get()
	{
		return pool.Count > 0 ? pool.Dequeue() : fixedSize ? null : createFunc();
	}

	public void Return(T obj)
		=> pool.Enqueue(obj);

	public void Clear()
	{
		while (pool.Count > 0)
		{
			var obj = pool.Dequeue();
			if (obj != null)
			{
				UnityEngine.Object.Destroy(obj.gameObject);
			}
		}
	}
}
