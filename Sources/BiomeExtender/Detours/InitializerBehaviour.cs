using System;
using UnityEngine;

namespace Detours
{
	internal class InitializerBehaviour : MonoBehaviour
	{
		protected bool reinjectNeeded;

		protected float reinjectTime;

		public void OnLevelWasLoaded(int level)
		{
			this.reinjectNeeded = true;
			this.reinjectTime = (float)((level >= 0) ? 1 : 0);
		}

		public void FixedUpdate()
		{
			if (this.reinjectNeeded)
			{
				this.reinjectTime -= Time.fixedDeltaTime;
				if (this.reinjectTime <= 0f)
				{
					this.reinjectNeeded = false;
					this.reinjectTime = 0f;
				}
			}
		}

		public void Start()
		{
			this.OnLevelWasLoaded(-1);
		}
	}
}
