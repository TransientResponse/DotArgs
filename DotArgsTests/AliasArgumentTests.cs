using DotArgs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotArgsTests
{
	[TestClass]
	public class AliasArgumentTests
	{
		[TestMethod]
		public void ResetSetsDefaultValueOfReference()
		{
			var reference = new FlagArgument(true);
			var alias = new AliasArgument(reference);

			alias.Reset();
			Assert.AreEqual(true, alias.Value);

			reference = new FlagArgument(false);
			alias = new AliasArgument(reference);
			alias.Reset();
			Assert.AreEqual(false, alias.Value);
		}
	}
}