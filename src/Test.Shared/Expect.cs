namespace Test.Shared
{
    using System;

    /// <summary>
    /// Minimal, framework-agnostic assertion helpers.  Touchstone treats a thrown
    /// exception as a failing case, so every helper simply throws
    /// <see cref="ExpectationException"/> with a descriptive message when the
    /// expectation is not met.  This keeps the shared suites free of any
    /// dependency on xUnit, NUnit, or the CLI runner.
    /// </summary>
    public static class Expect
    {
        /// <summary>
        /// Assert that a value is null.
        /// </summary>
        /// <param name="actual">The value under test.</param>
        /// <param name="because">Optional context describing the expectation.</param>
        public static void Null(string? actual, string? because = null)
        {
            if (actual != null)
                throw new ExpectationException($"Expected null but was \"{actual}\".{Suffix(because)}");
        }

        /// <summary>
        /// Assert that a value is not null.
        /// </summary>
        /// <param name="actual">The value under test.</param>
        /// <param name="because">Optional context describing the expectation.</param>
        public static void NotNull(string? actual, string? because = null)
        {
            if (actual == null)
                throw new ExpectationException($"Expected a non-null value but was null.{Suffix(because)}");
        }

        /// <summary>
        /// Assert that two strings are exactly equal (ordinal).
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The value under test.</param>
        /// <param name="because">Optional context describing the expectation.</param>
        public static void Equal(string? expected, string? actual, string? because = null)
        {
            if (!string.Equals(expected, actual, StringComparison.Ordinal))
                throw new ExpectationException(
                    $"Expected \"{expected ?? "<null>"}\" but was \"{actual ?? "<null>"}\".{Suffix(because)}");
        }

        /// <summary>
        /// Assert that a string contains the supplied substring.
        /// </summary>
        /// <param name="expectedSubstring">The substring that must be present.</param>
        /// <param name="actual">The value under test.</param>
        /// <param name="because">Optional context describing the expectation.</param>
        public static void Contains(string expectedSubstring, string? actual, string? because = null)
        {
            if (actual == null || !actual.Contains(expectedSubstring, StringComparison.Ordinal))
                throw new ExpectationException(
                    $"Expected value to contain \"{expectedSubstring}\" but was \"{actual ?? "<null>"}\".{Suffix(because)}");
        }

        /// <summary>
        /// Assert that a string does not contain the supplied substring.
        /// </summary>
        /// <param name="unexpectedSubstring">The substring that must be absent.</param>
        /// <param name="actual">The value under test.</param>
        /// <param name="because">Optional context describing the expectation.</param>
        public static void DoesNotContain(string unexpectedSubstring, string? actual, string? because = null)
        {
            if (actual != null && actual.Contains(unexpectedSubstring, StringComparison.Ordinal))
                throw new ExpectationException(
                    $"Expected value to NOT contain \"{unexpectedSubstring}\" but was \"{actual}\".{Suffix(because)}");
        }

        /// <summary>
        /// Assert that a condition is true.
        /// </summary>
        /// <param name="condition">The condition that must hold.</param>
        /// <param name="because">Optional context describing the expectation.</param>
        public static void True(bool condition, string? because = null)
        {
            if (!condition)
                throw new ExpectationException($"Expected condition to be true.{Suffix(because)}");
        }

        private static string Suffix(string? because)
        {
            return string.IsNullOrEmpty(because) ? string.Empty : $"  ({because})";
        }
    }

    /// <summary>
    /// Exception thrown when an <see cref="Expect"/> assertion fails.
    /// </summary>
    public sealed class ExpectationException : Exception
    {
        /// <summary>
        /// Instantiate.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public ExpectationException(string message) : base(message)
        {
        }
    }
}
