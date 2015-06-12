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
		/// <param name="position">Position this argument is expected to be located in the command line.</param>
		public OptionArgument( string defaultValue, bool required = false, int? position = null )
			: base( defaultValue, required, position )
		{
			HelpPlaceholder = "OPTION";
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
}