using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetArgs;

namespace NetArgsTests
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[TestClass]
	public class CommandLineArgsTest
	{
		[TestMethod]
		public void AliasTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			args.AddFlag( "flag" );
			args.AddAlias( "flag", "alias" );

			Assert.IsTrue( args.Process( string.Empty ) );
			Assert.IsFalse( args.GetFlag( "alias" ) );

			Assert.IsTrue( args.Process( "-alias" ) );
			Assert.IsTrue( args.GetFlag( "alias" ) );

			ExceptionAssert.Assert<KeyNotFoundException>( () => args.AddAlias( "nonexisting", "test" ) );
		}

		[TestMethod]
		public void FlagTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			args.AddFlag( "flag" );

			Assert.IsTrue( args.Process( string.Empty ) );
			Assert.IsFalse( args.GetFlag( "flag" ) );
			ExceptionAssert.Assert<KeyNotFoundException>( () => args.GetFlag( "nonexisting" ) );

			Assert.IsTrue( args.Process( "-flag" ) );
			Assert.IsTrue( args.GetFlag( "flag" ) );

			Assert.IsTrue( args.Process( "--flag" ) );
			Assert.IsTrue( args.GetFlag( "flag" ) );

			Assert.IsTrue( args.Process( "/flag" ) );
			Assert.IsTrue( args.GetFlag( "flag" ) );
		}

		[TestMethod]
		public void GetArgNameTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			PrivateObject obj = new PrivateObject( args );

			string name = (string)obj.Invoke( "GetArgName", "/arg" );
			Assert.AreEqual( "arg", name );

			name = (string)obj.Invoke( "GetArgName", "-arg" );
			Assert.AreEqual( "arg", name );

			name = (string)obj.Invoke( "GetArgName", "arg" );
			Assert.AreEqual( "arg", name );

			name = (string)obj.Invoke( "GetArgName", "--arg" );
			Assert.AreEqual( "arg", name );

			name = (string)obj.Invoke( "GetArgName", "/arg-" );
			Assert.AreEqual( "arg-", name );

			name = (string)obj.Invoke( "GetArgName", "/arg/" );
			Assert.AreEqual( "arg/", name );

			name = (string)obj.Invoke( "GetArgName", "/arg--" );
			Assert.AreEqual( "arg--", name );

			name = (string)obj.Invoke( "GetArgName", "//arg" );
			Assert.AreEqual( "arg", name );

			name = (string)obj.Invoke( "GetArgName", "//arg--" );
			Assert.AreEqual( "arg--", name );

			name = (string)obj.Invoke( "GetArgName", "/--/-/-//arg" );
			Assert.AreEqual( "arg", name );
		}

		[TestMethod]
		public void OptionTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			args.AddOption( "option", false, "123" );

			Assert.IsTrue( args.Process( string.Empty ) );
			Assert.AreEqual( "123", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "/option=42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "/option:42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "/option 42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "--option=42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "--option:42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "--option 42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "-option 42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "-option=42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			Assert.IsTrue( args.Process( "-option:42" ) );
			Assert.AreEqual( "42", args.GetOption( "option" ) );

			ExceptionAssert.Assert<KeyNotFoundException>( () => args.GetOption( "nonexisting" ) );
		}

		[TestMethod]
		public void SplitCommandLineTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			PrivateObject obj = new PrivateObject( args );

			List<string> parsed = (List<string>)obj.Invoke( "SplitCommandLine", "this is a test" );
			CollectionAssert.AreEqual( new[] { "this", "is", "a", "test" }, parsed );

			parsed = (List<string>)obj.Invoke( "SplitCommandLine", "this \"is a test\"" );
			CollectionAssert.AreEqual( new[] { "this", "is a test" }, parsed );

			parsed = (List<string>)obj.Invoke( "SplitCommandLine", "this 'is a test'" );
			CollectionAssert.AreEqual( new[] { "this", "is a test" }, parsed );

			parsed = (List<string>)obj.Invoke( "SplitCommandLine", "this \"is 'a' test\"" );
			CollectionAssert.AreEqual( new[] { "this", "is 'a' test" }, parsed );

			parsed = (List<string>)obj.Invoke( "SplitCommandLine", "this 'is \"a\" test'" );
			CollectionAssert.AreEqual( new[] { "this", "is \"a\" test" }, parsed );

			parsed = (List<string>)obj.Invoke( "SplitCommandLine", "this  is    a  test " );
			CollectionAssert.AreEqual( new[] { "this", "is", "a", "test" }, parsed );
		}

		[TestMethod]
		public void ValidateFlagTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			args.AddFlag( "flag", true );

			Assert.IsFalse( args.Process( string.Empty ) );
		}
	}
}