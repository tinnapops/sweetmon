using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sinoze.Engine
{
	/// <summary>
	/// Siege Client
	/// 
	/// A standalone object. User can create as many Siege clients as they want on the fly at any time by calling Siege<T>.Create()
	/// Think of it as a telecommunication device where you can send and receive messages through it 
	/// but with more advanced features e.g. send to a specific recipient, or the sent message is persisted 
	/// through a couple game engine frames.
	/// 
	/// Note : When a Siege client is disposed, messages created by that client still live.
	/// 
	/// For more info about Siege
	/// See : https://docs.google.com/a/sinozegames.com/document/d/1uzRSE6I2TttUczxbC0LrN3N0cp53ziu40eQpLhP64Lk/edit?usp=sharing
	/// </summary>
	public sealed class Siege<T> : Siege, IDisposable
	{
		public string Name { get; private set; }
		public ReadOnlyCollection<string> Tags { get; private set; }

		/// <summary>
		/// Initializes a new instance of Siege client
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="tags">Tags.</param>
		public Siege(string name = null, params string[] tags)
		{
			this.Name = name;
			this.Tags = new ReadOnlyCollection<string>(tags);
		}
	
		#region Post
		/// <summary>
		/// Post an anonymous message to server (which forward to every Siege instances including itself)
		/// Note : the posting message will be sent to the other Siege instances in the next frame
		/// </summary>
		/// <param name="message">Message.</param>
		public void Post(T message, SiegePostOption postOption = SiegePostOption.SingleFrame, int groupId = 0)
		{
			if(_isDisposed)
				return;

			SiegeServer.Instance.EnqueuePost<T>(this, message, postOption, groupId);
		}
		#endregion

		#region Listen

		/// <summary>
		/// collect all unique listen ids for later disposal
		/// </summary>
		List<Args<int, int, int>> listenRefs = new List<Args<int, int, int>>();

		/// <summary>
		/// gotta seperate the count because we need to use this in the destructor
		/// </summary>
		int listenCount;

		/// <summary>
		/// Listen the specified callback and groupId.
		/// Note : all listen callbacks will be deregistered as soon as Dispose() is called
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="groupId">Group identifier.</param>
		public void Listen(Action<Siege, T> callback, SiegeListenOption listenOption = SiegeListenOption.SingleComsume, int groupId = 0)
		{
			if(_isDisposed)
			{
				Logger.LogWarning("This Siege was already disposed while calling Listen()");
				return;
			}

			var listenId = SiegeServer.Instance.EnqueueListen<T>(this, callback, listenOption, groupId);
			listenRefs.Add(listenId);
			listenCount++;
		}
		#endregion
		
		#region IDisposable implementation
		
		private bool _isDisposed;
		
		/// <summary>
		/// Creator of this object should call Dispose if it's no longer in use.
		/// </summary>
		public void Dispose ()
		{
			if(_isDisposed)
				return;

			if(SiegeServer.IsInstanceExists) // to prevent Unity error when creating gameObject while the game is stopping
			{
				foreach(var listenRef in listenRefs)
				{
					SiegeServer.Instance.TryUnregisterListen<T>(this, listenRef);
				}
				listenRefs.Clear ();
				listenCount = 0;
			}

			_isDisposed = true;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the Siege instance
		/// is reclaimed by garbage collection.
		/// 
		/// this will check whether the user have called Dispose()
		/// a warning message will be raised if this Siege instance used to register a listener
		/// </summary>
		~Siege()
		{
			if(!_isDisposed)
			{
				if(listenCount > 0)
				{
					Logger.LogError("Possible Error : a Siege client with registerd Listeners was not properly disposed.");
				}
			}
		}
		
		#endregion
	}
	
	public abstract class Siege
	{
		/// <summary>
		/// Post the specified message anonymously
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="postOption">Post option.</param>
		/// <param name="groupId">Group identifier.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void Post<T>(T message, SiegePostOption postOption = SiegePostOption.SingleFrame, int groupId = 0)
		{
			SiegeServer.Instance.EnqueuePost<T>(null, message, postOption, groupId);
		}
	}
}