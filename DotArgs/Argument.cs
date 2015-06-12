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

namespace DotArgs
{
	/// <summary>
	/// Base class for an argument that can be registered with a <see cref="CommandLineArgs"/> .
	/// </summary>
	public abstract class Argument
	{
		/// <summary>Initializes a new instance of the <see cref="Argument"/> class.</summary>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="required">Flag indicating whether this argument is required.</param>
		/// <param name="position">Position this argument is expected to be located in the command line.</param>
		protected Argument( object defaultValue, bool required = false, int? position = null )
		{
			DefaultValue = defaultValue;
			IsRequired = required;
			SupportsMultipleValues = false;
			Position = position;

			if( IsRequired )
			{
				DefaultValue = null;
			}
		}

		/// <summary>Resets this argument.</summary>
		public virtual void Reset()
		{
			Value = DefaultValue;
		}

		/// <summary>Validates the specified value.</summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value"/> is valid; otherwise <c>false</c> .
		/// </returns>
		protected internal abstract bool Validate( object value );

		/// <summary>
		/// The default value that will be used if no value was passed on the command line.
		/// </summary>
		/// <remarks>Using this when <see cref="IsRequired"/> is set will have no effect.</remarks>
		public object DefaultValue { get; protected internal set; }

		/// <summary>The message that will be displayed in the help page for your program.</summary>
		public string HelpMessage { get; set; }

		/// <summary>
		/// Value that will be shown (in upper case) in the usage page for this argument. Setting
		/// this to <c>null</c> will display the default value (i.e. OPTION, COLLECTION, etc.).
		/// </summary>
		public string HelpPlaceholder { get; set; }

		/// <summary>
		/// Flag indicating whether this argument is required, i.e. must be provided via the command line.
		/// </summary>
		public bool IsRequired { get; protected set; }

		/// <summary>Indicates whether this argument requires an explicit option.</summary>
		public bool NeedsValue { get; protected set; }

		/// <summary>Position this argument is expected to be located in the command line.</summary>
		public int? Position { get; set; }

		/// <summary>A method that can be executed when the command line arguments are processed.</summary>
		public Action<object> Processor { get; set; }

		/// <summary>
		/// Flag indicating whether multplie writes to <see cref="Value"/> will add a value or overwrite the existing one.
		/// </summary>
		public bool SupportsMultipleValues { get; protected set; }

		/// <summary>A method that can be used to validate a value for this argument.</summary>
		public Func<object, bool> Validator { get; set; }

		/// <summary>
		/// Gets or sets the value for this argument.
		/// </summary>
		/// <value>
		/// The value to set.
		/// </value>
		public virtual object Value { get; set; }
	}
}