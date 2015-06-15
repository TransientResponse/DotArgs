using DotArgs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotArgsTests
{
	[TestClass]
	public class AliasArgumentTests
	{
		[TestMethod]
		public void ReferenceIsUsedForProperties()
		{
			var reference = new OptionArgument( "default", false, 123 );
			var alias = new AliasArgument( reference );

			Assert.AreEqual( "default", alias.DefaultValue );
			Assert.AreEqual( false, alias.IsRequired );
			Assert.AreEqual( 123, alias.Position );
			Assert.AreEqual( reference.Processor, alias.Processor );
			Assert.AreEqual( reference.SupportsMultipleValues, alias.SupportsMultipleValues );
		}

		[TestMethod]
		public void ResetSetsDefaultValueOfReference()
		{
			var reference = new FlagArgument( true );
			var alias = new AliasArgument( reference );

			alias.Reset();
			Assert.AreEqual( true, alias.Value );

			reference = new FlagArgument();
			alias = new AliasArgument( reference );
			alias.Reset();
			Assert.AreEqual( false, alias.Value );
		}
	}
}