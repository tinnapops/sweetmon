using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sinoze.Engine.Collections
{
	public class RecycleArray<T>
	{
		// for now, provide public access
		// in the future we have to improve this by providing enumeration api
		public T[] items;
		public List<int> activeSlots = new List<int>();
		private Queue<int> availableSlots = new Queue<int>();

		public int Alloc(ref T item)
		{
			if(availableSlots.Count == 0)
				Expand(100);

			var slotIndex = availableSlots.Dequeue();
			activeSlots.Add(slotIndex);
			items[slotIndex] = item;
			return slotIndex;
		}
		
		public int Alloc()
		{
			var val = default(T);
			return Alloc(ref val);
		}

		public void Free(int slotIndex)
		{
			items[slotIndex] = default(T);
			availableSlots.Enqueue(slotIndex);
			activeSlots.Remove(slotIndex);
		}
		
		void Expand(int amount)
		{
			int oldLength = items != null ? items.Length : 0;
			int newLength = oldLength + amount;
			
			if(items == null)
				items = new T[newLength];
			else
				Array.Resize(ref items, newLength);
			
			for(int i=oldLength; i<newLength; i++)
			{
				availableSlots.Enqueue(i);
			}
		}
	}
}
