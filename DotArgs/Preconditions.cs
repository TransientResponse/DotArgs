using System;

namespace DotArgs
{
	internal static class Preconditions
	{
		internal static void ThrowIfNull( this object value, string name )
		{
			if( value == null )
			{
				throw new ArgumentException( name );
			}
		}
	}
}