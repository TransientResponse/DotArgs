using System.Collections.Generic;
using DotArgs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotArgsTests
{
	[TestClass]
	public class CommandLineHelperTests
	{
		[TestMethod]
		public void GetArgNameTest()
		{
			string name = CommandLineHelper.GetArgName("/arg");
			Assert.AreEqual("arg", name);

			name = CommandLineHelper.GetArgName("-arg");
			Assert.AreEqual("arg", name);

			name = CommandLineHelper.GetArgName("arg");
			Assert.AreEqual("arg", name);

			name = CommandLineHelper.GetArgName("--arg");
			Assert.AreEqual("arg", name);

			name = CommandLineHelper.GetArgName("/arg-");
			Assert.AreEqual("arg-", name);

			name = CommandLineHelper.GetArgName("/arg/");
			Assert.AreEqual("arg/", name);

			name = CommandLineHelper.GetArgName("/arg--");
			Assert.AreEqual("arg--", name);

			name = CommandLineHelper.GetArgName("//arg");
			Assert.AreEqual("arg", name);

			name = CommandLineHelper.GetArgName("//arg--");
			Assert.AreEqual("arg--", name);

			name = CommandLineHelper.GetArgName("/--/-/-//arg");
			Assert.AreEqual("arg", name);

			name = CommandLineHelper.GetArgName("/option=value");
			Assert.AreEqual("option", name);

			name = CommandLineHelper.GetArgName("/option:value");
			Assert.AreEqual("option", name);

			name = CommandLineHelper.GetArgName("--option=value");
			Assert.AreEqual("option", name);

			name = CommandLineHelper.GetArgName("--option:value");
			Assert.AreEqual("option", name);
		}

		[TestMethod]
		public void SplitCommandLineTest()
		{
			List<string> parsed = CommandLineHelper.SplitCommandLine("this is a test");
			CollectionAssert.AreEqual(new[] {"this", "is", "a", "test"}, parsed);

			parsed = CommandLineHelper.SplitCommandLine("this \"is a test\"");
			CollectionAssert.AreEqual(new[] {"this", "is a test"}, parsed);

			parsed = CommandLineHelper.SplitCommandLine("this 'is a test'");
			CollectionAssert.AreEqual(new[] {"this", "is a test"}, parsed);

			parsed = CommandLineHelper.SplitCommandLine("this \"is 'a' test\"");
			CollectionAssert.AreEqual(new[] {"this", "is 'a' test"}, parsed);

			parsed = CommandLineHelper.SplitCommandLine("this 'is \"a\" test'");
			CollectionAssert.AreEqual(new[] {"this", "is \"a\" test"}, parsed);

			parsed = CommandLineHelper.SplitCommandLine("this  is    a  test ");
			CollectionAssert.AreEqual(new[] {"this", "is", "a", "test"}, parsed);
		}
	}
}