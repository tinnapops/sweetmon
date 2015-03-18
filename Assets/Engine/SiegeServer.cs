using UnityEngine;
using System.Collections.Generic;
using System;
using Sinoze.Engine.Collections;

namespace Sinoze.Engine
{
	/// <summary>
	/// Siege server.
	/// </summary>
	[SinozeEngineComponent]
	public sealed class SiegeServer : MonoBehaviour 
	{
		#region Instance
		private static SiegeServer _instance;
		public static SiegeServer Instance
		{
			get
			{
				if(_instance == null)
				{
					// if Bootstracp was init, this component should already be exist
					_instance = Root.GameObject.GetComponent<SiegeServer>();

					// automatically create SiegeServer instance without using Bootstrap
					if(_instance == null)
						_instance = Root.GameObject.AddComponent<SiegeServer>();
				}
				return _instance;
			}
		}
		internal static bool IsInstanceExists
		{
			get { return _instance != null; }
		}
		#endregion

		Dictionary<int, SiegeGroup> siegeGroups = new Dictionary<int, SiegeGroup>();

		void Update()
		{
			foreach(var group in siegeGroups)
			{
				group.Value.Update();
			}
		}

		void LateUpdate()
		{
			foreach(var group in siegeGroups)
			{
				group.Value.LateUpdate();
			}
		}

		#region Internal API Call from Siege instances

		/// <summary>
		/// Enqueues the post.
		/// </summary>
		/// <returns>The post.</returns>
		/// <param name="sender">Sender, can be null if called by anonymous Siege.Post() API</param>
		/// <param name="message">Message.</param>
		/// <param name="optionOption">Option option.</param>
		/// <param name="groupId">Group identifier.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		internal Args<int, int, int> EnqueuePost<T>(Siege sender, T message, SiegePostOption optionOption, int groupId)
		{
			return GetSiegeGroup(groupId).EnqueuePost(sender, ref message, optionOption, groupId);
		}
		
		internal Args<int, int, int> EnqueueListen<T>(Siege sender, Action<Siege, T> callback, SiegeListenOption listenOption, int groupId)
		{
			return GetSiegeGroup(groupId).EnqueueListen(sender, callback, listenOption, groupId);
		}

		internal void TryUnregisterListen<T>(Siege sender, Args<int, int, int> listenRef)
		{
			GetSiegeGroup(listenRef.arg1).TryUnregisterListen<T>(sender, ref listenRef);
		}

		private SiegeGroup GetSiegeGroup(int groupId)
		{
			SiegeGroup group;
			if(!siegeGroups.TryGetValue(groupId, out group))
			{
				group = new SiegeGroup();
				siegeGroups.Add(groupId, group);
			}
			return group ;
		}
		#endregion

		#region Internal Logic
		class SiegeGroup
		{
			Dictionary<Type, SiegeGroupT> groupT = new Dictionary<Type, SiegeGroupT>();

			public void Update()
			{
				foreach(var g in groupT)
				{
					g.Value.Update();
				}
			}

			public void LateUpdate()
			{
				foreach(var g in groupT)
				{
					g.Value.LateUpdate();
				}
			}

			public Args<int, int, int> EnqueuePost<T>(Siege sender, ref T message, SiegePostOption postOption, int groupId)
			{
				return GetSiegeGroupT<T>().EnqueuePost(sender, ref message, postOption, groupId);
			}
			
			public Args<int, int, int> EnqueueListen<T>(Siege sender, Action<Siege, T> callback, SiegeListenOption listenOption, int groupId)
			{
				return GetSiegeGroupT<T>().EnqueueListen(sender, callback, listenOption, groupId);
			}

			public void TryUnregisterListen<T>(Siege sender, ref Args<int, int, int> listenRef)
			{
				GetSiegeGroupT<T>().TryUnregisterListen(sender, ref listenRef);
			}

			private SiegeGroupT<T> GetSiegeGroupT<T>()
			{
				var t = typeof(T);
				SiegeGroupT g;
				if(!groupT.TryGetValue(t, out g))
				{
					g = new SiegeGroupT<T>();
					groupT.Add(t, g);
				}
				return g as SiegeGroupT<T>;
			}

			abstract class SiegeGroupT
			{
				public abstract void Update();
				public abstract void LateUpdate();
			}

			class SiegeGroupT<T> : SiegeGroupT
			{
				// items pool
				SiegePostPool postPool = new SiegePostPool();
				SiegeListenPool listenPool = new SiegeListenPool();

				// add queue
				Queue<Args<Siege, T, SiegePostOption, Args<int, int, int>>> postQueue = new Queue<Args<Siege, T, SiegePostOption, Args<int, int, int>>>();
				Queue<Args<Siege, Action<Siege, T>, SiegeListenOption, Args<int, int, int>>> listenQueue = new Queue<Args<Siege, Action<Siege, T>, SiegeListenOption, Args<int, int, int>>>();

				// remove queue
				Queue<int> postToFree = new Queue<int>();
				Queue<int> listenToFree = new Queue<int>();

				public Args<int, int, int> EnqueuePost(Siege sender, ref T message, SiegePostOption postOption, int groupId)
				{
					var postRef = postPool.PreAlloc(groupId);
					postQueue.Enqueue(new Args<Siege, T, SiegePostOption, Args<int, int, int>>(ref sender, ref message, ref postOption, ref postRef));
					return postRef;
				}
				
				public Args<int, int, int> EnqueueListen(Siege sender, Action<Siege, T> callback, SiegeListenOption listenOption, int groupId)
				{
					var listenRef = listenPool.PreAlloc(groupId);
					listenQueue.Enqueue(new Args<Siege, Action<Siege, T>, SiegeListenOption, Args<int, int, int>>(ref sender, ref callback, ref listenOption, ref listenRef));
					return listenRef;
				}

				/// <summary>
				/// disable listen when a Siege is being disposed
				/// </summary>
				/// <param name="sender">Sender.</param>
				/// <param name="listenId">Listen identifier.</param>
				public void TryUnregisterListen(Siege sender, ref Args<int, int, int> listenRef)
				{
					var listenSlotIndex = listenRef.arg2;
					
					if(listenPool.items[listenSlotIndex].disable)
						return;
					
					if(listenPool.items[listenSlotIndex].listenId == listenRef.arg3)
					{
						// immediately free it (hoping that this is not in during update)
						listenPool.items[listenSlotIndex].disable = true;
						listenToFree.Enqueue(listenSlotIndex);
					}
				}

				public override void Update()
				{
					// send post to listeners
					for(int i=0;i<postPool.activeSlots.Count;i++)
					{
						var postSlotIndex = postPool.activeSlots[i];
						if(!postPool.items[postSlotIndex].active)
							continue;

						postPool.items[postSlotIndex].frameCount++;

						if(postPool.items[postSlotIndex].frameCount == 1)
						{
							for(int j=0;j<listenPool.activeSlots.Count;j++)
							{
								var listenSlotIndex = listenPool.activeSlots[j];
								
								if(!listenPool.items[listenSlotIndex].active)
									continue;
								if(listenPool.items[listenSlotIndex].disable)
									continue;

								// send message to listener
								listenPool.items[listenSlotIndex].callback(postPool.items[postSlotIndex].sender, postPool.items[postSlotIndex].message);

								if(listenPool.items[listenSlotIndex].listenOption == SiegeListenOption.SingleComsume)
								{
									// for SingleComsume listen option
									// immediately remove it as soon as it's received a message
									listenPool.items[listenSlotIndex].disable = true;
									listenToFree.Enqueue(listenSlotIndex);
								}

								if(postPool.items[postSlotIndex].postOption == SiegePostOption.SingleComsume)
								{
									// for SingleComsume post option
									// immediately remove it as soon as it's sent to a listener
									postPool.items[postSlotIndex].disable = true;
									postToFree.Enqueue(postSlotIndex);
									break;
								}
							}
						}

						if(postPool.items[postSlotIndex].postOption == SiegePostOption.SingleFrame)
						{
							// for SingleFrame post option
							// after send this post to all listeners, remove it
							postPool.items[postSlotIndex].disable = true;
							postToFree.Enqueue(postSlotIndex);
						}
					}
				}

				public override void LateUpdate()
				{
					// clean up
					while(postToFree.Count > 0)
					{
						postPool.Free(postToFree.Dequeue());
					}
					while(listenToFree.Count > 0)
					{
						listenPool.Free(listenToFree.Dequeue());
					}

					// pull new
					while(postQueue.Count > 0)
					{
						var post = postQueue.Dequeue();
						postPool.Add(ref post);
					}
					while(listenQueue.Count > 0)
					{
						var listen = listenQueue.Dequeue();
						listenPool.Add(ref listen);
					}
				}

				#region Post and Listen Data Structure
				
				struct SiegePost
				{
					public bool active; // we be true when dequeued
					public int frameCount; // number of frames passed after this post is being registered, zero mean first frame
					public Siege sender;
					public T message;
					public int groupId;
					public SiegePostOption postOption;
					public bool disable;
					public int postId;
				}
				
				class SiegePostPool : RecycleArray<SiegePost>
				{
					static int uniquePostId;
					
					public Args<int,int,int> PreAlloc(int groupId)
					{
						var slotIndex = base.Alloc();
						var postId = uniquePostId++;
						return new Args<int, int, int>(groupId, slotIndex, postId);
					}

					public void Add(ref Args<Siege, T, SiegePostOption, Args<int,int,int>> enqueuedData)
					{
						var post = new SiegePost()
						{
							active = true,
							sender = enqueuedData.arg1,
							message = enqueuedData.arg2,
							postOption = enqueuedData.arg3,
							postId = enqueuedData.arg4.arg3,
						};

						var slotIndex = enqueuedData.arg4.arg2;
						base.items[slotIndex] = post;
					}
				}
				
				struct SiegeListen
				{
					public bool active; // we be true when dequeued
					public Siege sender;
					public Action<Siege, T> callback;
					public SiegeListenOption listenOption;
					public bool disable;
					public int listenId;
				}

				class SiegeListenPool : RecycleArray<SiegeListen>
				{
					static int uniqueListenId;

					public Args<int,int,int> PreAlloc(int groupId)
					{
						var slotIndex = base.Alloc();
						var listenId = uniqueListenId++;
						return new Args<int, int, int>(groupId, slotIndex, listenId);
					}

					public void Add(ref Args<Siege, Action<Siege, T>, SiegeListenOption, Args<int, int, int>> enqueuedData)
					{
						var listen = new SiegeListen()
						{
							active = true,
							sender = enqueuedData.arg1,
							callback = enqueuedData.arg2,
							listenOption = enqueuedData.arg3,
							listenId = enqueuedData.arg4.arg3,
						};
						
						var slotIndex = enqueuedData.arg4.arg2;
						base.items[slotIndex] = listen;
					}
				}

				#endregion
			}
		}
		#endregion

	}
}