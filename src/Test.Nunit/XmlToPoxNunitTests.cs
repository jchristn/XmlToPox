namespace Test.Nunit
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Test.Shared;
    using Touchstone.Core;
    using Touchstone.NunitAdapter;

    /// <summary>
    /// NUnit host for all shared XmlToPox Touchstone suites.
    /// </summary>
    [TestFixture]
    public sealed class XmlToPoxNunitTests : TouchstoneNunitBase
    {
        /// <summary>
        /// Shared XmlToPox test suites.
        /// </summary>
        protected override IReadOnlyList<TestSuiteDescriptor> Suites
        {
            get
            {
                return XmlToPoxTestSuites.All;
            }
        }

        /// <summary>
        /// Execute every shared XmlToPox descriptor through the Touchstone NUnit adapter.
        /// </summary>
        /// <returns>Task.</returns>
        [Test]
        public async Task RunAll()
        {
            await RunAllAsync().ConfigureAwait(false);
        }
    }
}
