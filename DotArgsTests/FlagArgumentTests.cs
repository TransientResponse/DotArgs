using DotArgs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotArgsTests
{
	[TestClass]
	public class FlagArgumentTests
	{
		[TestMethod]
		public void ResetSetsDefaultValue()
		{
			var arg = new FlagArgument( true );
			arg.Reset();
			Assert.AreEqual( true, arg.Value );

			arg = new FlagArgument( false );
			arg.Reset();
			Assert.AreEqual( false, arg.Value );
		}
	}
}