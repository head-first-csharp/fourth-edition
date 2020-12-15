using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Unity.Cloud.Collaborate.Tests
{
    public static class TestHelpers
    {
        public const string TestDirectory = "SomePathName/";

        static readonly TaskFactory k_MyTaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return k_MyTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            k_MyTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void ThrowsAsync<T>(Func<Task> asyncDelegate) where T : Exception
        {
            Assert.Throws<T>(() => RunSync(asyncDelegate));
        }

        public static void ShouldBe<T>(this T expr1, T value, string msg = "")
        {
            if (!expr1.Equals(value))
                throw new InvalidOperationException($"Test expected {value}, but found : {expr1}. [{msg}]");
        }

        public static void ShouldBe(this object expr1, object value, string msg = "")
        {
            if (expr1 != value)
                throw new InvalidOperationException($"Test expected {value}, but found : {expr1}. [{msg}]");
        }

        public static void ShouldBeNull(object obj, string msg = "")
        {
            if (obj != null)
                throw new InvalidOperationException($"Test expected null value, but found : {obj}. [{msg}]" );
        }
    }
}
