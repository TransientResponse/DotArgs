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

namespace DotArgs
{
	/// <summary>Argument that acts as an alias for another argument.</summary>
	public class AliasArgument : Argument
	{
		/// <summary>Initializes a new instance of the <see cref="AliasArgument"/> class.</summary>
		/// <param name="entry">The argument this alias should mirror.</param>
		public AliasArgument( Argument entry )
			: base( entry.DefaultValue, entry.IsRequired )
		{
			Reference = entry;
		}

		/// <summary>Gets the value of this argument.</summary>
		/// <returns>The argument's value.</returns>
		public override object GetValue()
		{
			return Reference.GetValue();
		}

		/// <summary>Sets the value for this argument.</summary>
		/// <param name="value">The value to set.</param>
		public override void SetValue( object value )
		{
			Reference.SetValue( value );
		}

		/// <summary>
		/// The default value that will be used if no value was passed on the command line.
		/// </summary>
		/// <remarks>Using this when <see cref="IsRequired"/> is set will have no effect.</remarks>
		public new object DefaultValue
		{
			get { return Reference.DefaultValue; }
			protected internal set { Reference.DefaultValue = value; }
		}

		/// <summary>
		/// Flag indicating whether this argument is required, i.e. must be provided via the command line.
		/// </summary>
		public new bool IsRequired { get { return Reference.IsRequired; } }

		private Argument Reference;
	}

	/// <summary>
	/// Base class for an argument that can be registered with a <see cref="CommandLineArgs"/> .
	/// </summary>
	public abstract class Argument
	{
		/// <summary>Initializes a new instance of the <see cref="Argument"/> class.</summary>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="required">Flag indicating whether this argument is required.</param>
		public Argument( object defaultValue, bool required = false )
		{
			DefaultValue = defaultValue;
			IsRequired = required;
		}

		/// <summary>Gets the value of this argument.</summary>
		/// <returns>The argument's value.</returns>
		public virtual object GetValue()
		{
			return Value;
		}

		/// <summary>Resets this argument.</summary>
		public virtual void Reset()
		{
			Value = DefaultValue;
		}

		/// <summary>Sets the value for this argument.</summary>
		/// <param name="value">The value to set.</param>
		public virtual void SetValue( object value )
		{
			Value = value;
		}

		/// <summary>Validates the specified value.</summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value"/> is valid; otherwise <c>false</c> .
		/// </returns>
		protected internal virtual bool Validate( object value )
		{
			if( IsRequired )
			{
				return !string.IsNullOrWhiteSpace( value as string );
			}

			return true;
		}

		/// <summary>
		/// The default value that will be used if no value was passed on the command line.
		/// </summary>
		/// <remarks>Using this when <see cref="IsRequired"/> is set will have no effect.</remarks>
		public object DefaultValue { get; protected internal set; }

		/// <summary>
		/// Flag indicating whether this argument is required, i.e. must be provided via the command line.
		/// </summary>
		public bool IsRequired { get; protected set; }

		/// <summary>Indicates whether this argument requires an explicit option.</summary>
		public bool NeedsValue { get; protected set; }

		/// <summary>A method that can be used to validate a value for this argument.</summary>
		public Func<object, bool> Validator { get; set; }

		private object Value;
	}

	/// <summary>An option that can take multiple values.</summary>
	public class CollectionArgument : OptionArgument
	{
		/// <summary>Initializes a new instance of the <see cref="CollectionArgument"/> class.</summary>
		/// <param name="required">Flag indicating whether this argument is required.</param>
		public CollectionArgument( bool required = false )
			: base( null, required )
		{
			base.SetValue( new string[0] );
		}

		/// <summary>Resets this argument.</summary>
		public override void Reset()
		{
			Values.Clear();
		}

		/// <summary>Sets the value for this argument.</summary>
		/// <param name="value">The value to set.</param>
		public override void SetValue( object value )
		{
			Values.Add( value as string );
		}

		/// <summary>
		/// Gets the value of this argument.
		/// </summary>
		/// <returns>
		/// The argument's value.
		/// </returns>
		public override object GetValue()
		{
			return Values.ToArray();
		}

		private List<string> Values = new List<string>();
	}

	/// <summary>Class for defining, validating and processing command line arguments.</summary>
	public class CommandLineArgs
	{
		/// <summary>Initializes a new instance of the <see cref="CommandLineArgs"/> class.</summary>
		public CommandLineArgs()
		{
			OutputWriter = Console.Out;
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
				throw new KeyNotFoundException( string.Format( "An collection with the name {0} was not registered.", name ) );
			}

			Argument entry = Arguments[name];
			return (T)entry.GetValue();
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
				if( !Arguments.ContainsKey( arg ) )
				{
					//errors.Add( string.Format( "Unknown option: {0}", parts[i] ) );
					errors = true;
					continue;
				}

				Argument entry = Arguments[arg];

				// Simple case: a flag
				if( entry.NeedsValue )
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
						entry.SetValue( value );
					}
					else
					{
						// Missing argument for
						errors = true;
					}
				}
				else
				{
					entry.SetValue( true );
				}
			}

			if( !errors )
			{
				errors = !Validate();
			}

			return !errors;
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
				throw new KeyNotFoundException( string.Format( "An entry with the name {0} was not registered.", originalName ) );
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
			foreach( Argument entry in Arguments.Values )
			{
				entry.Reset();
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
			foreach( KeyValuePair<string, Argument> kvp in Arguments.Where( kvp => !( kvp.Value is AliasArgument ) ) )
			{
				Argument entry = kvp.Value;
				if( !entry.Validate( entry.GetValue() ) )
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

		private Dictionary<string, Argument> Arguments = new Dictionary<string, Argument>();
	}

	/// <summary>A simple argument flag.</summary>
	/// <remarks>
	/// A flag can be specified by passing <c>-FLAG</c> , <c>--flag</c> or <c>/FLAG</c> from the
	/// command line.
	/// </remarks>
	public class FlagArgument : Argument
	{
		/// <summary>Initializes a new instance of the <see cref="FlagArgument"/> class.</summary>
		/// <param name="defaultValue">The default value for this flag.</param>
		/// <param name="required">Flag indicating whether this argument is required.</param>
		public FlagArgument( bool defaultValue = false, bool required = false )
			: base( defaultValue, required )
		{
			if( IsRequired )
			{
				DefaultValue = null;
			}
		}

		/// <summary>Validates the specified value.</summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value"/> is valid; otherwise <c>false</c> .
		/// </returns>
		protected internal override bool Validate( object value )
		{
			bool valid = value is bool;

			if( IsRequired )
			{
				valid = valid && value != null;
			}

			return valid;
		}
	}

	/// <summary>An argument that can have any value.</summary>
	/// <remarks>
	/// An option can be specified by passing <c>-OPTION VALUE</c> , <c>-OPTION=VALUE</c> ,
	/// <c>-OPTION:VALUE</c> , <c>--OPTION VALUE</c> , <c>--OPTION=VALUE</c> , <c>--OPTION:VALUE</c>
	/// , <c>/OPTION VALUE</c> , <c>/OPTION:VALUE</c> or <c>/OPTION=VALUE</c> from the command line
	/// </remarks>
	public class OptionArgument : Argument
	{
		/// <summary>Initializes a new instance of the <see cref="OptionArgument"/> class.</summary>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="required">Flag indicating whether this argument is required.</param>
		public OptionArgument( string defaultValue, bool required = false )
			: base( defaultValue, required )
		{
			NeedsValue = true;
		}

		/// <summary>Validates the specified value.</summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value"/> is valid; otherwise <c>false</c> .
		/// </returns>
		protected internal override bool Validate( object value )
		{
			if( Validator != null )
			{
				return Validator( value );
			}

			return true;
		}
	}

	/// <summary>
	/// A set argument is an option that 
	/// </summary>
	public class SetArgument : OptionArgument
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SetArgument"/> class.
		/// </summary>
		/// <param name="validOptions">The valid options this argument may be given.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="required">Flag indicating whether this argument is required.</param>
		public SetArgument( string[] validOptions, string defaultValue, bool required = false )
			: base( defaultValue, required )
		{
			ValidOptions = validOptions;
		}

		/// <summary>
		/// Validates the specified value.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>
		///   <c>true</c> if <paramref name="value" /> is valid; otherwise <c>false</c> .
		/// </returns>
		protected internal override bool Validate( object value )
		{
			return ValidOptions.Contains( value as string );
		}

		string[] ValidOptions;
	}
}