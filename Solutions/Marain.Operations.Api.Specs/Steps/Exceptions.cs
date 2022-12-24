// <copyright file="Exceptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Api.Specs.Steps
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    public static class Exceptions
    {
        public const string LastExceptionContextKey = "LastException";

        public static async Task ExecuteAndStoreExceptionAsync(SpecFlowContext context, Func<Task> target)
        {
            ClearLastException(context);

            try
            {
                await target().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.Set(ex, LastExceptionContextKey);
            }
        }

        public static Exception GetLastException(SpecFlowContext context)
        {
            if (context.TryGetValue(LastExceptionContextKey, out Exception ex))
            {
                return ex;
            }

            Assert.Fail("Expected an exception to have been thrown, but was not.");

            // This looks odd, but is only here to calm down the compiler. In practice, this code is unreachable.
            return null!;
        }

        public static void AssertLastExceptionWasOfType(SpecFlowContext context, string expectedExceptionTypeName)
        {
            if (!context.TryGetValue(LastExceptionContextKey, out Exception exception))
            {
                Assert.Fail($"Expected an exception of type '{expectedExceptionTypeName}' to have been thrown, but none was.");
            }

            Assert.AreEqual(expectedExceptionTypeName, exception.GetType().Name);
        }

        public static void AssertNoLastException(SpecFlowContext context)
        {
            if (context.TryGetValue(LastExceptionContextKey, out Exception ex))
            {
                Assert.Fail($"Expected no last exception, but an exception of type '{ex.GetType().Name}' was thrown: '{ex.Message}' - {ex}");
            }
        }

        public static void ClearLastException(SpecFlowContext context)
        {
            context.Remove(LastExceptionContextKey);
        }
    }
}