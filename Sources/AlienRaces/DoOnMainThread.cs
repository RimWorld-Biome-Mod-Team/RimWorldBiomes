using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlienRace
{
	public class DoOnMainThread : MonoBehaviour
	{
		public static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();

		public void Update()
		{
			while (DoOnMainThread.ExecuteOnMainThread.Count > 0)
			{
				DoOnMainThread.ExecuteOnMainThread.Dequeue()();
			}
		}
	}
}
