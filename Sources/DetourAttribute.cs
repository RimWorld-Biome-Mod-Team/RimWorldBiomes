using System;
using System.Reflection;

namespace StorageSearch
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class DetourAttribute : Attribute
	{
		public Type source;

		public BindingFlags bindingFlags;

		public DetourAttribute(Type source)
		{
			this.source = source;
		}
	}
}
