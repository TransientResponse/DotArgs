using System;
using System.Linq;
using MSAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace DotArgsTests
{
	/// <summary>
	/// Helper class for testing methods that should throw exception. This class can do more than
	/// the ExpectedExceptionAttribute since it allows to
	/// a) make sure the exception is thrown at the right point and b) validate the thrown exception
	/// with user defined logic
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class ExceptionAssert
	{
		/// <summary>Execute an action and catch a given exception</summary>
		/// <typeparam name="TException">Type of the exception to catch</typeparam>
		/// <param name="action">The action to execute</param>
		/// <param name="validationAction">
		/// An optional action that can be executed to validate the thrown exception.
		/// </param>
		internal static void Assert<TException>( Action action, Action<TException> validationAction = null )
			where TException : Exception
		{
			TException ex = null;
			try
			{
				action();
			}
			catch( TException e )
			{
				ex = e;
			}

			if( ex == null )
			{
				MSAssert.Fail( "No exception was thrown although an exception was expected" );
				return;
			}

			if( validationAction != null )
			{
				validationAction( ex );
			}
		}
	}
}