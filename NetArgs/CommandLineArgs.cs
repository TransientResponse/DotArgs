// Copyright (c) 2014 Matthias Specht
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
using System.IO;
using System.Linq;

namespace NetArgs
{
	/// <summary>Class for defining, validating and processing command line arguments.</summary>
	public class CommandLineArgs
	{
		/// <summary>Initializes a new instance of the <see cref="CommandLineArgs"/> class.</summary>
		public CommandLineArgs()
		{
			OutputWriter = Console.Out;
		}

		/// <summary>Registers an alias for an existing entry.</summary>
		/// <param name="originalName">Name of the original option.</param>
		/// <param name="alias">The alias to add for the option.</param>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">
		/// An entry with the name <paramref name="originalName"/> was not registered.
		/// </exception>
		public void AddAlias( string originalName, string alias )
		{
			if( !Commands.ContainsKey( originalName ) )
			{
				throw new KeyNotFoundException( string.Format( "An entry with the name {0} was not registered.", originalName ) );
			}

			CommandEntryAlias entry = new CommandEntryAlias( Commands[originalName] );
			Commands[alias] = entry;
		}

		/// <summary>Registers a flag.</summary>
		/// <remarks>
		/// A flag can be specified by passing
		/// <code>
		/// -FLAG
		/// </code>,
		/// <code>
		/// --flag
		/// </code>or
		/// <code>
		/// /FLAG
		/// </code>from the command line with
		/// <code>
		/// FLAG
		/// </code>being the name given in <paramref name="name"/> or and alias thereof.
		/// </remarks>
		/// <param name="name">Name of the flag</param>
		/// <param name="required">Indicates whether the user must specifiy a value for this flag.</param>
		/// <param name="defaultValue">The default value for this flag when omitted by the user.</param>
		public void AddFlag( string name, bool required = false, bool defaultValue = false )
		{
			CommandEntryFlag entry = new CommandEntryFlag();
			entry.DefaultValue = defaultValue;
			entry.Required = required;
			if( !required )
			{
				entry.Value = defaultValue;
			}

			Commands[name] = entry;
		}

		/// <summary>Registers an option.</summary>
		/// <remarks>
		/// An option can be specified by passing
		/// <code>
		/// -OPTION VALUE
		/// </code>,
		/// <code>
		/// -OPTION=VALUE
		/// </code>,
		/// <code>
		/// --OPTION:VALUE
		/// </code>,
		/// <code>
		/// -OPTION:VALUE
		/// </code>
		/// <code>
		/// --OPTION VALUE
		/// </code>,
		/// <code>
		/// --OPTION=VALUE
		/// </code>,
		/// <code>
		/// /OPTION:VALUE
		/// </code>or
		/// <code>
		/// /OPTION=VALUE
		/// </code>from the command line with
		/// <code>
		/// OPTION
		/// </code>being the name given in <paramref name="name"/> and
		/// <code>
		/// VALUE
		/// </code>being the value passed by the command line.
		/// </remarks>
		/// <param name="name">Name of the option.</param>
		/// <param name="required">Indicates whether the user must specifiy a value for this option.</param>
		/// <param name="defaultValue">The default value for this option when omitted by the user.</param>
		/// <param name="validator">
		/// An optional delegate that can be called to validate the value of the option. The
		/// delegate takes the value of the option its only parameter and returns a bool whether the
		/// value is valid or not.
		/// </param>
		public void AddOption( string name, bool required = false, string defaultValue = null, Func<string, bool> validator = null )
		{
			CommandEntryOption entry = new CommandEntryOption();
			entry.DefaultValue = defaultValue;
			entry.Validator = validator;
			entry.Required = required;
			if( !required )
			{
				entry.Value = defaultValue;
			}

			Commands[name] = entry;
		}

		/// <summary>Gets the collection.</summary>
		/// <param name="name">Name of the collection to read.</param>
		/// <returns>
		/// The effective value of the collection. If the collection was omitted in the arguments,
		/// the default value will be returned.
		/// </returns>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">
		/// A collection with the name <paramref name="name"/> was not registered.
		/// </exception>
		public string[] GetCollection( string name )
		{
			if( !Commands.ContainsKey( name ) )
			{
				throw new KeyNotFoundException( string.Format( "An collection with the name {0} was not registered.", name ) );
			}

			CommandEntry entry = Commands[name];
			if( entry.Type != CommandType.Collection )
			{
				throw new KeyNotFoundException( string.Format( "An collection with the name {0} was not registered.", name ) );
			}

			return (string[])entry.Value;
		}

		/// <summary>Gets the value of a flag.</summary>
		/// <param name="flag">Name of the flag to read.</param>
		/// <returns>
		/// The effective value of the flag. If the flag was omitted in the arguments, the default
		/// value will be returned.
		/// </returns>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">
		/// A flag with the name <paramref name="flag"/> was not registered.
		/// </exception>
		public bool GetFlag( string flag )
		{
			if( !Commands.ContainsKey( flag ) )
			{
				throw new KeyNotFoundException( string.Format( "An flag with the name {0} was not registered.", flag ) );
			}

			CommandEntry entry = Commands[flag];
			if( entry.Type != CommandType.Flag )
			{
				throw new KeyNotFoundException( string.Format( "An flag with the name {0} was not registered.", flag ) );
			}

			return (bool)entry.Value;
		}

		/// <summary>Gets the value of an option.</summary>
		/// <param name="name">Name of the option to read.</param>
		/// <returns>
		/// The effective value of the option. If the option was omitted in the arguments, the
		/// default value will be returned.
		/// </returns>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">
		/// An option with the name <paramref name="name"/> was not registered.
		/// </exception>
		public string GetOption( string name )
		{
			if( !Commands.ContainsKey( name ) )
			{
				throw new KeyNotFoundException( string.Format( "An option with the name {0} was not registered.", name ) );
			}

			CommandEntry entry = Commands[name];
			if( entry.Type != CommandType.Option )
			{
				throw new KeyNotFoundException( string.Format( "An option with the name {0} was not registered.", name ) );
			}

			return entry.Value as string;
		}

		/// <summary>Prints a help message describing the effects of all available options.</summary>
		/// <param name="errorMessage">Optional error message to display.</param>
		public void PrintHelp( string errorMessage = null )
		{
			throw new NotImplementedException();
		}

		/// <summary>Processes a set of command line arguments.</summary>
		/// <param name="args">
		/// Command line arguments to process. This is usally coming from your Main method.
		/// </param>
		/// <returns>
		/// <c>true</c> if the arguments in <paramref name="args"/> are valid; otherwise
		/// <c>false</c> .
		/// </returns>
		public bool Process( string[] args )
		{
			return Process( string.Join( " ", args ) );
		}

		/// <summary>Processes a set of command line arguments.</summary>
		/// <param name="args">
		/// Command line arguments to process. This is usally coming from your Main method.
		/// </param>
		/// <returns>
		/// <c>true</c> if the arguments in <paramref name="args"/> are valid; otherwise
		/// <c>false</c> .
		/// </returns>
		public bool Process( string args )
		{
			Reset();

			bool errors = false;

			List<string> parts = SplitCommandLine( args );
			for( int i = 0; i < parts.Count; ++i )
			{
				string arg = GetArgName( parts[i] );
				if( !Commands.ContainsKey( arg ) )
				{
					//errors.Add( string.Format( "Unknown option: {0}", parts[i] ) );
					errors = true;
					continue;
				}

				CommandEntry entry = Commands[arg];

				// Simple case: a flag
				if( entry.Type == CommandType.Flag )
				{
					entry.Value = true;
				}
				else
				{
					// Not so simple cases: Collection and Option
					string value = ExtractValueFromArg( parts[i] );

					if( value == null && i < parts.Count - 1 )
					{
						value = parts[i + 1];
						i++;
						// TODO: Check if value is not a flag/option/whatever
					}

					if( value != null )
					{
						if( entry.Type == CommandType.Collection )
						{
							List<string> values = new List<string>( (string[])entry.Value );
							values.Add( value );
							entry.Value = values.ToArray();
						}
						else
						{
							entry.Value = value;
						}
					}
					else
					{
						// Missing argument for
						errors = true;
					}
				}
			}

			if( !errors )
			{
				errors = !Validate();
			}

			return !errors;
		}

		/// <summary>Defines a previously registered option as a collection.</summary>
		/// <remarks>
		/// A collection is an option that can have multiple values assigned. To specify multiple
		/// values you have to pass
		/// <code>
		/// /option=VALUE1 /option=VALUE2
		/// </code>from the command line which would set the values of option to
		/// <code>
		/// VALUE1
		/// </code>and
		/// <code>
		/// VALUE2
		/// </code>
		/// </remarks>
		/// <param name="name">The name of the option to define as a collection.</param>
		/// <param name="min">The minimum required number of values the collection must have.</param>
		/// <param name="max">The maximum number of values the collection can have.</param>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">
		/// An option with the name <paramref name="name"/> was not registered.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="min"/> must be smaller than <paramref name="max"/>
		/// </exception>
		public void SetCollection( string name, int min = 0, int max = int.MaxValue )
		{
			if( !Commands.ContainsKey( name ) )
			{
				throw new KeyNotFoundException( string.Format( "An option with the name {0} was not registered.", name ) );
			}

			CommandEntryCollection entry = new CommandEntryCollection( Commands[name] as CommandEntryOption );
			Commands[name] = entry;
		}

		/// <summary>Defines a help message that describes the workings of a flag or option.</summary>
		/// <param name="name">Name of the flag/option the message applies to.</param>
		/// <param name="message">The help message for the flag/option.</param>
		public void SetHelpText( string name, string message )
		{
			throw new NotImplementedException();
		}

		private string ExtractValueFromArg( string arg )
		{
			char[] seperators = new[] { '=', ':' };

			int idx = arg.IndexOfAny( seperators );
			if( idx == -1 )
			{
				return null;
			}

			return arg.Substring( idx + 1 );
		}

		private string GetArgName( string arg )
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

		private void Reset()
		{
			foreach( CommandEntry entry in Commands.Values )
			{
				entry.Value = entry.DefaultValue;
			}
		}

		private List<string> SplitCommandLine( string args )
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

		private bool Validate()
		{
			bool errors = false;
			foreach( KeyValuePair<string, CommandEntry> kvp in Commands.Where( kvp => !( kvp.Value is CommandEntryAlias ) ) )
			{
				CommandEntry entry = kvp.Value;
				if( !entry.Validate( entry.Value as string ) )
				{
					errors = true;
				}
			}

			return !errors;
		}

		/// <summary>The name of the application that will be displayed in the usage page.</summary>
		/// <example>MyCoolProgram v1.2 Copyright (C) John Smith &lt;smith@example.com&gt;</example>
		public string ApplicationName { get; set; }

		/// <summary>
		/// The TextWriter that is used to write the output. The default value is to use <see cref="Console.Out"/>
		/// </summary>
		public TextWriter OutputWriter { get; set; }

		private Dictionary<string, CommandEntry> Commands = new Dictionary<string, CommandEntry>();
	}

	internal abstract class CommandEntry
	{
		internal virtual bool Validate( string value )
		{
			if( Required )
			{
				return !string.IsNullOrWhiteSpace( value );
			}

			return true;
		}

		internal virtual object DefaultValue { get; set; }

		internal virtual bool Required { get; set; }

		internal abstract CommandType Type { get; }

		internal virtual object Value { get; set; }
	}

	internal class CommandEntryAlias : CommandEntry
	{
		public CommandEntryAlias( CommandEntry entry )
		{
			Reference = entry;
		}

		internal override object DefaultValue
		{
			get { return Reference.DefaultValue; }
			set { Reference.DefaultValue = value; }
		}

		internal override bool Required { get { return Reference.Required; } }

		internal override CommandType Type { get { return Reference.Type; } }

		internal override object Value { get { return Reference.Value; } set { Reference.Value = value; } }

		private CommandEntry Reference;
	}

	internal class CommandEntryCollection : CommandEntryOption
	{
		public CommandEntryCollection( CommandEntryOption option )
		{
			this.DefaultValue = new string[0];
			this.Required = option.Required;
			this.Value = new string[0];
		}

		internal override CommandType Type { get { return CommandType.Collection; } }
	}

	internal class CommandEntryFlag : CommandEntry
	{
		internal override CommandType Type { get { return CommandType.Flag; } }
	}

	internal class CommandEntryOption : CommandEntry
	{
		internal override bool Validate( string value )
		{
			if( Validator != null )
			{
				return Validator( value );
			}

			return true;
		}

		public Func<string, bool> Validator { get; set; }

		internal override CommandType Type { get { return CommandType.Option; } }
	}

	internal enum CommandType
	{
		Flag,
		Option,
		Collection
	}
}