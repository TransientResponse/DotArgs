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

		/// <summary>Validates the specified value.</summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value"/> is valid; otherwise <c>false</c> .
		/// </returns>
		protected internal override bool Validate( object value )
		{
			return Reference.Validate( value );
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

		/// <summary>Position this argument is expected to be located in the command line.</summary>
		public new int? Position { get { return Reference.Position; } }

		/// <summary>
		/// Flag indicating whether multplie calls to <see cref="SetValue" /> will add a value or overwrite the existing one.
		/// </summary>
		public new bool SupportsMultipleValues { get { return Reference.SupportsMultipleValues; } }

		private Argument Reference;
	}
}