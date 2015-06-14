using System.Collections.Generic;
using System.Linq;

namespace DotArgs
{
	internal static class CommandLineHelper
	{
		internal static string GetArgName( string arg )
		{
			char[] seperators = { '=', ':' };
			char[] prefixes = { '-', '/' };

			int end = arg.Length;
			int remove = 0;
			bool atStart = true;

			for( int i = 0; i < arg.Length; ++i )
			{
				if( prefixes.Contains( arg[i] ) && atStart )
				{
					remove++;
				}
				else if( seperators.Contains( arg[i] ) )
				{
					end = i;
				}
				else
				{
					atStart = false;
				}
			}

			if( remove > 0 )
			{
				arg = arg.Substring( remove );
				end -= remove;
			}

			return arg.Substring( 0, end );
		}

		internal static List<string> SplitCommandLine( string args )
		{
			List<string> parts = new List<string>();

			string buffer = string.Empty;
			bool inDoubleString = false;
			bool inSingleString = false;

			foreach( char c in args )
			{
				if( c == '\'' )
				{
					if( !inDoubleString )
					{
						inSingleString = !inSingleString;
					}
					else
					{
						buffer += c;
					}
				}
				else if( c == '"' )
				{
					if( !inSingleString )
					{
						inDoubleString = !inDoubleString;
					}
					else
					{
						buffer += c;
					}
				}
				else if( c == ' ' )
				{
					if( !inDoubleString && !inSingleString )
					{
						if( !string.IsNullOrWhiteSpace( buffer ) )
						{
							parts.Add( buffer );
						}
						buffer = string.Empty;
					}
					else
					{
						buffer += c;
					}
				}
				else
				{
					buffer += c;
				}
			}

			if( !string.IsNullOrWhiteSpace( buffer ) )
			{
				parts.Add( buffer );
			}

			return parts;
		}
	}
}