// Copyright (c) 2014-2015 Matthias Specht
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotArgs
{
	/// <summary>Class for defining, validating and processing command line arguments.</summary>
	public class CommandLineArgs
	{
		/// <summary>Initializes a new instance of the <see cref="CommandLineArgs"/> class.</summary>
		public CommandLineArgs()
		{
			OutputWriter = Console.Out;
			ExecutableName = Path.GetFileNameWithoutExtension( Assembly.GetCallingAssembly().Location );
		}

		/// <summary>Adds an example that will be displayed on the help page.</summary>
		/// <param name="description">The name or description for this example.</param>
		/// <param name="commandLine">The command line to display for this example.</param>
		public void AddExample( string description, string commandLine )
		{
			Examples.Add( description, commandLine );
		}

		/// <summary>Gets the value of an argument.</summary>
		/// <param name="name">Name of the argument to read.</param>
		/// <returns>
		/// The effective value of the argument. If the argument was omitted in the arguments, the
		/// default value will be returned.
		/// </returns>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">
		/// An argument with the name <paramref name="name"/> was not registered.
		/// </exception>
		public T GetValue<T>( string name )
		{
			if( !Arguments.ContainsKey( name ) )
			{
				throw new KeyNotFoundException( string.Format( CultureInfo.CurrentCulture, "An collection with the name {0} was not registered.", name ) );
			}

			Argument entry = Arguments[name];
			return (T)entry.Value;
		}

		/// <summary>Prints a help message describing the effects of all available options.</summary>
		/// <param name="errorMessage">Optional error message to display.</param>
		public void PrintHelp( string errorMessage = null )
		{
			string argList = string.Join( " ", Arguments.OrderBy( k => k.Key ).Select( a => ArgumentToArgList( a.Key, a.Value ) ) );

			OutputWriter.WriteLine( ApplicationInfo );
			OutputWriter.WriteLine();
			if( !string.IsNullOrWhiteSpace( errorMessage ) )
			{
				OutputWriter.WriteLine( errorMessage );
				OutputWriter.WriteLine();
			}
			OutputWriter.WriteLine( "Usage:" );
			OutputWriter.WriteLine( "{0} {1}", ExecutableName, argList );

			foreach( KeyValuePair<string, Argument> kvp in Arguments.OrderBy( k => k.Key ) )
			{
				OutputWriter.WriteLine();
				OutputWriter.WriteLine( "{0,-10}{1}", kvp.Key, kvp.Value.HelpMessage );
				OutputWriter.WriteLine( "{0,-10}{1}", "", GetArgumentInfo( kvp.Value ) );
			}

			if( Examples.Any() )
			{
				OutputWriter.WriteLine();
				OutputWriter.WriteLine( "Examples:" );

				foreach( KeyValuePair<string, string> kvp in Examples.OrderBy( k => k.Key ) )
				{
					OutputWriter.WriteLine();
					OutputWriter.WriteLine( kvp.Key );
					OutputWriter.WriteLine( kvp.Value );
				}
			}
		}

		/// <summary>
		/// Processes all registered arguments that have their <see cref="Argument.Processor"/> set.
		/// </summary>
		public void Process()
		{
			foreach( Argument arg in Arguments.Values.Where( a => !( a is AliasArgument ) ) )
			{
				if( arg.Processor != null )
				{
					arg.Processor( arg.Value );
				}
			}
		}

		/// <summary>Registers an alias for an existing entry.</summary>
		/// <param name="originalName">Name of the original option.</param>
		/// <param name="alias">The alias to add for the option.</param>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">
		/// An entry with the name <paramref name="originalName"/> was not registered.
		/// </exception>
		public void RegisterAlias( string originalName, string alias )
		{
			if( !Arguments.ContainsKey( originalName ) )
			{
				throw new KeyNotFoundException( string.Format( CultureInfo.CurrentCulture, "An entry with the name {0} was not registered.", originalName ) );
			}

			AliasArgument entry = new AliasArgument( Arguments[originalName] );
			Arguments[alias] = entry;
		}

		/// <summary>Registers a new argument.</summary>
		/// <param name="name">Name of the argument to register.</param>
		/// <param name="arg">The argument's configuration.</param>
		public void RegisterArgument( string name, Argument arg )
		{
			Arguments[name] = arg;
		}

		/// <summary>
		/// Registers a help argument that will display the help page for the program if set by the user.
		/// </summary>
		/// <param name="name">Name of the flag. The default value is "help".</param>
		public void RegisterHelpArgument( string name = "help" )
		{
			FlagArgument arg = new FlagArgument();
			arg.Processor = ( v ) => PrintHelp();
			arg.HelpMessage = "Displays this help.";

			RegisterArgument( name, arg );
		}

		/// <summary>
		/// Sets the default argument that will be filled when no argument name is given.
		/// </summary>
		/// <param name="argument">Name of the argument to use as the default.</param>
		public void SetDefaultArgument( string argument )
		{
			if( !Arguments.ContainsKey( argument ) )
			{
				throw new ArgumentException( string.Format( CultureInfo.CurrentCulture, "Argument {0} was not registered", argument ), nameof( argument ) );
			}

			DefaultArgument = argument;
		}

		/// <summary>
		/// Try gets the value of an argument.
		/// </summary>
		/// <param name="name">Name of the argument to read.</param>
		/// <param name="value">The effective value of the argument.</param>
		public bool TryGetValue<T>( string name, out T value )
		{
			try
			{
				value = GetValue<T>( name );
				return true;
			}
			catch( KeyNotFoundException )
			{
				value = default(T);
			}
			return false;
		}

		/// <summary>Processes a set of command line arguments.</summary>
		/// <param name="args">
		/// Command line arguments to process. This is usally coming from your Main method.
		/// </param>
		/// <param name="outErrors">
		/// Optional "out" parameter that holds error strings for every encountered error.
		/// </param>
		/// <returns>
		/// <c>true</c> if the arguments in <paramref name="args"/> are valid; otherwise
		/// <c>false</c> .
		/// </returns>
		public bool Validate( string[] args, OptionalOut<string[]> outErrors = null )
		{
			return Validate( string.Join( " ", args ), outErrors );
		}

		/// <summary>Processes a set of command line arguments.</summary>
		/// <param name="args">
		/// Command line arguments to process. This is usally coming from your Main method.
		/// </param>
		/// <param name="outErrors">
		/// Optional "out" parameter that holds error strings for every encountered error.
		/// </param>
		/// <returns>
		/// <c>true</c> if the arguments in <paramref name="args"/> are valid; otherwise
		/// <c>false</c> .
		/// </returns>
		public bool Validate( string args, OptionalOut<string[]> outErrors = null )
		{
			Reset();

			bool ignoreAlreadyHandled = false;
			if( DefaultArgument != null )
			{
				ignoreAlreadyHandled = Arguments[DefaultArgument].SupportsMultipleValues;
			}

			bool handledDefault = false;
			bool errors = false;
			List<string> errorList = new List<string>();

			List<string> parts = SplitCommandLine( args );
			for( int i = 0; i < parts.Count; ++i )
			{
				string arg = GetArgName( parts[i] );
				if( !IsArgumentName( parts[i] ) )
				{
					Argument posArgument = GetArgumentForPosition( i );
					if( posArgument != null )
					{
						string argName = GetNameForArgument( posArgument );

						parts[i] = string.Format( CultureInfo.InvariantCulture, "/{0}={1}", argName, arg );
						arg = argName;
					}
					else if( DefaultArgument != null )
					{
						if( !handledDefault || ignoreAlreadyHandled )
						{
							parts[i] = string.Format( CultureInfo.InvariantCulture, "/{0}={1}", DefaultArgument, arg );
							arg = DefaultArgument;

							handledDefault = true;
						}
					}
				}

				if( !Arguments.ContainsKey( arg ) )
				{
					if( DefaultArgument != null && ( !handledDefault || ignoreAlreadyHandled ) )
					{
						parts[i] = string.Format( CultureInfo.InvariantCulture, "/{0}={1}", DefaultArgument, arg );
						arg = DefaultArgument;

						handledDefault = true;
					}
					else
					{
						errorList.Add( string.Format( CultureInfo.CurrentCulture, "Unknown option: '{0}'", arg ) );

						errors = true;
						continue;
					}
				}

				Argument entry = Arguments[arg];

				if( entry.NeedsValue )
				{
					// Not so simple cases: Collection and Option
					string value = ExtractValueFromArg( parts[i] );

					if( value == null && i < parts.Count - 1 )
					{
						value = parts[i + 1];

						if( Arguments.ContainsKey( GetArgName( value ) ) )
						{
							value = null;
						}
						else
						{
							i++;
						}
					}

					if( value != null )
					{
						entry.Value = value;
					}
					else
					{
						// Missing argument
						errorList.Add( string.Format( CultureInfo.CurrentCulture, "Missing value for option '{0}'", arg ) );
						errors = true;
					}
				}
				else // Simple case: a flag
				{
					entry.Value = true;
				}
			}

			foreach( KeyValuePair<string, Argument> kvp in Arguments )
			{
				Argument entry = kvp.Value;
				object value = entry.Value;

				if( entry.IsRequired && value == null )
				{
					errorList.Add( string.Format( CultureInfo.CurrentCulture, "Missing value for option '{0}'", kvp.Key ) );
					errors = true;
				}

				if( !entry.Validate( value ) )
				{
					errorList.Add( string.Format( CultureInfo.CurrentCulture, "{0}: Invalid value {1}", kvp.Key, value ) );
					errors = true;
				}
			}

			if( outErrors != null )
			{
				outErrors.Result = errorList.Distinct().ToArray();
			}

			return !errors;
		}

		private static string ArgumentToArgList( string name, Argument arg )
		{
			string desc = string.Format( CultureInfo.InvariantCulture, "/{0}", name );
			if( arg.NeedsValue )
			{
				desc += string.Format( CultureInfo.InvariantCulture, "={0}", arg.HelpPlaceholder );
			}

			if( arg.IsRequired )
			{
				return string.Format( CultureInfo.InvariantCulture, "<{0}>", desc );
			}
			else
			{
				if( arg.DefaultValue != null )
				{
					desc += string.Format( CultureInfo.InvariantCulture, ", {0}", arg.DefaultValue );
				}

				return string.Format( CultureInfo.InvariantCulture, "[{0}]", desc );
			}
		}

		private static string ExtractValueFromArg( string arg )
		{
			char[] seperators = new[] { '=', ':' };

			int idx = arg.IndexOfAny( seperators );
			if( idx == -1 )
			{
				return null;
			}

			return arg.Substring( idx + 1 );
		}

		private static string GetArgName( string arg )
		{
			char[] seperators = new[] { '=', ':' };
			char[] prefixes = new[] { '-', '/' };

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

		private static string GetArgumentInfo( Argument arg )
		{
			string str = "";

			if( arg.IsRequired )
			{
				str = "Required";
			}
			else
			{
				str = "Optional";

				if( arg.DefaultValue != null )
				{
					str += string.Format( CultureInfo.CurrentCulture, ", Default value: {0}", arg.DefaultValue );
				}
			}

			return str;
		}

		private static bool IsArgumentName( string arg )
		{
			char[] prefixes = new[] { '-', '/' };

			foreach( char p in prefixes )
			{
				if( arg.StartsWith( p.ToString(), StringComparison.OrdinalIgnoreCase ) )
				{
					return true;
				}
			}

			return false;
		}

		private static List<string> SplitCommandLine( string args )
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

		private Argument GetArgumentForPosition( int position )
		{
			return Arguments.Values.FirstOrDefault( a => a.Position.HasValue && a.Position.Value == position );
		}

		private string GetNameForArgument( Argument arg )
		{
			foreach( var kvp in Arguments )
			{
				if( kvp.Value == arg )
				{
					return kvp.Key;
				}
			}

			return null;
		}

		private void Reset()
		{
			foreach( Argument entry in Arguments.Values )
			{
				entry.Reset();
			}
		}

		/// <summary>Information about the application that will be displayed in the usage page.</summary>
		/// <example>MyCoolProgram v1.2 Copyright (C) John Smith &lt;smith@example.com&gt;</example>
		public string ApplicationInfo { get; set; }

		/// <summary>Name of the executeable that will be displayed in the usage page.</summary>
		/// <remarks>
		/// The default value for this is the name of the assembly containing the code that created
		/// this object.
		/// </remarks>
		public string ExecutableName { get; set; }

		/// <summary>
		/// The TextWriter that is used to write the output. The default value is to use <see cref="Console.Out"/>
		/// </summary>
		public TextWriter OutputWriter { get; set; }

		private Dictionary<string, Argument> Arguments = new Dictionary<string, Argument>();
		private string DefaultArgument = null;
		private Dictionary<string, string> Examples = new Dictionary<string, string>();
	}
}