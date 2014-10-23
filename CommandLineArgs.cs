using System;
using System.Linq;

namespace NetArgs
{
	/// <summary>
	/// Class for defining, validating and processing command line arguments.
	/// </summary>
	public class CommandLineArgs
	{
		/// <summary>
		/// Registers an alias for an existing option or flag.
		/// </summary>
		/// <param name="originalName">Name of the original option.</param>
		/// <param name="alias">The alias to add for the option-</param>
		public void AddAlias( string originalName, string alias )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Registers a flag.
		/// </summary>
		/// <remarks>A flag can be specified by passing <code>-FLAG</code>, <code>--flag</code> or <code>/FLAG</code>
		/// from the command line with <code>FLAG</code> being the name given in <paramref name="name"/> or
		/// and alias thereof.</remarks>
		/// <param name="name">Name of the flag</param>
		/// <param name="required">Indicates whether the user must specifiy a value for this flag.</param>
		/// <param name="defaultValue">The default value for this flag when omitted by the user.</param>
		public void AddFlag( string name, bool required = false, bool defaultValue = false )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Registers an option.
		/// </summary>
		/// <remarks>An option can be specified by passing <code>-OPTION VALUE</code>, <code>--OPTION VALUE</code>,
		/// <code>--OPTION=VALUE</code>, <code>/OPTION:VALUE</code> or <code>/OPTION=VALUE</code> from the command line
		/// with <code>OPTION</code> being the name given in <paramref name="name"/> and <code>VALUE</code> being the
		/// value passed by the command line.</remarks>
		/// <param name="name">Name of the option.</param>
		/// <param name="required">Indicates whether the user must specifiy a value for this option.</param>
		/// <param name="defaultValue">The default value for this option when omitted by the user.</param>
		/// <param name="validator">An optional delegate that can be called to validate the value of the option.
		/// The delegate takes the value of the option its only parameter and returns a bool whether the value
		/// is valid or not.</param>
		public void AddOption( string name, bool required = false, string defaultValue = null, Func<string, bool> validator = null )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value of a flag.
		/// </summary>
		/// <param name="flag">Name of the flag to read.</param>
		/// <returns>The effective value of the flag. If the flag was omitted in the
		/// arguments, the default value will be returned.</returns>
		public bool GetFlag( string flag )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value of an option.
		/// </summary>
		/// <param name="name">Name of the option to read.</param>
		/// <returns>The effective value of the option. If the option was omitted in the
		/// arguments, the default value will be returned.</returns>
		public string GetOption( string name )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Prints a help message describing the effects of all available options.
		/// </summary>
		/// <param name="errorMessage">Optional error message to display.</param>
		public void PrintHelp( string errorMessage = null )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Processes a set of command line arguments.
		/// </summary>
		/// <param name="args">Command line arguments to process. This is usally coming from your Main method.</param>
		/// <returns><c>true</c> if the arguments in <paramref name="args"/> are valid; otherwise <c>false</c>.</returns>
		public bool Process( string[] args )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Defines a help message that describes the workings of a flag or option.
		/// </summary>
		/// <param name="name">Name of the flag/option the message applies to.</param>
		/// <param name="message">The help message for the flag/option.</param>
		public void SetHelpText( string name, string message )
		{
			throw new NotImplementedException();
		}
	}
}