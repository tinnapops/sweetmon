using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Pair
{
	public static Pair<T> Create<T>(T left,T right)
	{
		return new Pair<T>() {
			Left	= left,
			Right	= right
		};
	}
	public static Pair<T,U> Create<T,U>(T left,U right)
	{
		return new Pair<T,U>() {
			Left	= left,
			Right	= right
		};
	}

	public static KeyValuePair<T,U> KeyValue<T,U>(T key,U value)
	{
		return new KeyValuePair<T,U>(key,value);
	}
}

public static class Triple
{
	public static Triple<T> Create<T>(T left,T right)
	{
		return new Triple<T>() {
			Left	= left,
			Right	= right
		};
	}
	public static Triple<T,U,V> Create<T,U,V>(T left,U center,V right)
	{
		return new Triple<T,U,V>() {
			Left	= left,
			Center	= center,
			Right	= right
		};
	}
}

[System.Serializable]
public struct Pair<T>
{
	public T Left;
	public T Right;
	public static implicit operator KeyValuePair<T,T>(Pair<T> pair)
	{
		return new KeyValuePair<T,T>(pair.Left,pair.Right);
	}
	public static implicit operator Pair<T>(KeyValuePair<T,T> pair)
	{
		return Pair.Create<T>(pair.Key,pair.Value);
	}
}

[System.Serializable]
public struct Pair<T,U>
{
	public T Left;
	public U Right;

	public static implicit operator KeyValuePair<T,U>(Pair<T,U> pair)
	{
		return new KeyValuePair<T,U>(pair.Left,pair.Right);
	}
	public static implicit operator Pair<T,U>(KeyValuePair<T,U> pair)
	{
		return Pair.Create<T,U>(pair.Key,pair.Value);
	}
}

[System.Serializable]
public struct Triple<T>
{
	public T Left;
	public T Center;
	public T Right;
}

[System.Serializable]
public struct Triple<T,U,V>
{
	public T Left;
	public U Center;
	public V Right;
}