namespace Test.Xunit
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Test.Shared;
    using Touchstone.Core;
    using Touchstone.XunitAdapter;
    using Xunit;

    /// <summary>
    /// xUnit host for all shared XmlToPox Touchstone suites.
    /// </summary>
    public sealed class XmlToPoxFactTests : TouchstoneFactBase
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
        /// Execute every shared XmlToPox descriptor through the Touchstone xUnit adapter.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task RunAll()
        {
            await RunAllAsync();
        }
    }
}
