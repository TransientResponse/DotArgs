using System;

namespace DotArgs
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class Preconditions
	{
		// ReSharper disable once UnusedParameter.Global
		internal static void ThrowIfNull( this object value, string name )
		{
			if( value == null )
			{
				throw new ArgumentException( name );
			}
		}
	}
}