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
using System.Collections.Generic;

namespace DotArgs
{
	/// <summary>An option that can take multiple values.</summary>
	public class CollectionArgument : OptionArgument
	{
		/// <summary>Initializes a new instance of the <see cref="CollectionArgument"/> class.</summary>
		/// <param name="required">Flag indicating whether this argument is required.</param>
		/// <param name="position">Position this argument is expected to be located in the command line.</param>
		public CollectionArgument( bool required = false, int? position = null )
			: base( null, required, position )
		{
			SupportsMultipleValues = true;
			HelpPlaceholder = "COLLECTION";
			base.Value = new string[0];
		}

		/// <summary>Resets this argument.</summary>
		public override void Reset()
		{
			Values.Clear();
		}

		/// <summary>
		/// Gets or sets the value for this argument.
		/// </summary>
		/// <value>
		/// The value to set.
		/// </value>
		public override object Value
		{
			get
			{
				return Values.ToArray();
			}

			set
			{
				Values.Add( value as string );
			}
		}

		private readonly List<string> Values = new List<string>();
	}
}