using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotArgs;

namespace DotArgsTest
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[TestClass]
	public class CommandLineArgsTest
	{
		[TestMethod]
		public void AliasTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			args.RegisterArgument( "flag", new FlagArgument(false) );
			args.RegisterAlias( "flag", "alias" );

			Assert.IsTrue( args.Process( string.Empty ) );
			Assert.IsFalse( args.GetValue<bool>( "alias" ) );

			Assert.IsTrue( args.Process( "-alias" ) );
			Assert.IsTrue( args.GetValue<bool>( "alias" ) );

			ExceptionAssert.Assert<KeyNotFoundException>( () => args.RegisterAlias( "nonexisting", "test" ) );

			args = new CommandLineArgs();
			args.RegisterArgument( "flag", new FlagArgument( false , true ) );
			args.RegisterAlias( "flag", "alias" );

			Assert.IsTrue( args.Process( "-alias" ) );
		}

		[TestMethod]
		public void FlagTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			args.RegisterArgument( "flag", new FlagArgument() );

			Assert.IsTrue( args.Process( string.Empty ) );
			Assert.IsFalse( args.GetValue<bool>( "flag" ) );
			ExceptionAssert.Assert<KeyNotFoundException>( () => args.GetValue<bool>( "nonexisting" ) );

			Assert.IsTrue( args.Process( "-flag" ) );
			Assert.IsTrue( args.GetValue<bool>( "flag" ) );

			Assert.IsTrue( args.Process( "--flag" ) );
			Assert.IsTrue( args.GetValue<bool>( "flag" ) );

			Assert.IsTrue( args.Process( "/flag" ) );
			Assert.IsTrue( args.GetValue<bool>( "flag" ) );
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

			name = (string)obj.Invoke( "GetArgName", "/option=value" );
			Assert.AreEqual( "option", name );

			name = (string)obj.Invoke( "GetArgName", "/option:value" );
			Assert.AreEqual( "option", name );

			name = (string)obj.Invoke( "GetArgName", "--option=value" );
			Assert.AreEqual( "option", name );

			name = (string)obj.Invoke( "GetArgName", "--option:value" );
			Assert.AreEqual( "option", name );
		}

		[TestMethod]
		public void CollectionTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			args.RegisterArgument( "option", new CollectionArgument() );

			Assert.IsTrue( args.Process( "/option=value1" ) );
			string[] values = args.GetValue<string[]>( "option" );
			CollectionAssert.AreEqual( new[] { "value1" }, args.GetValue<string[]>( "option" ) );

			Assert.IsTrue( args.Process( "/option=value1 /option=value2" ) );
			values = args.GetValue<string[]>( "option" );
			CollectionAssert.AreEqual( new[] { "value1", "value2" }, values );

			Assert.IsTrue( args.Process( "/option=value1 --option=value2" ) );
			values = args.GetValue<string[]>( "option" );
			CollectionAssert.AreEqual( new[] { "value1", "value2" }, values );

			ExceptionAssert.Assert<KeyNotFoundException>( () => args.GetValue<string[]>( "nonexisting" ) );
		}

		[TestMethod]
		public void OptionTest()
		{
			CommandLineArgs args = new CommandLineArgs();
			args.RegisterArgument( "option", new OptionArgument( "123", false ) );

			Assert.IsTrue( args.Process( string.Empty ) );
			Assert.AreEqual( "123", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "/option=42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "/option:42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "/option 42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "--option=42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "--option:42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "--option 42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "-option 42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "-option=42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			Assert.IsTrue( args.Process( "-option:42" ) );
			Assert.AreEqual( "42", args.GetValue<string>( "option" ) );

			ExceptionAssert.Assert<KeyNotFoundException>( () => args.GetValue<string>( "nonexisting" ) );

			Assert.IsTrue( args.Process( "-option:42 /option=444" ) );
			Assert.AreEqual( "444", args.GetValue<string>( "option" ) );
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
			args.RegisterArgument( "flag", new FlagArgument( true, true ) );

			Assert.IsFalse( args.Process( string.Empty ) );
		}
	}
}