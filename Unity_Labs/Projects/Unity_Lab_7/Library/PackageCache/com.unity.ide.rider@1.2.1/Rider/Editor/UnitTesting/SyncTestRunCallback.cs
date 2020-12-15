#if TEST_FRAMEWORK
using NUnit.Framework.Interfaces;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine.TestRunner;

[assembly: TestRunCallback(typeof(SyncTestRunCallback))]

namespace Packages.Rider.Editor.UnitTesting
{
  public class SyncTestRunCallback : ITestRunCallback
  {
    public void RunStarted(ITest testsToRun)
    {
    }

    public void RunFinished(ITestResult testResults)
    {
      SyncTestRunEventsHandler.instance.OnRunFinished();
    }

    public void TestStarted(ITest test)
    {
      if (!test.IsSuite)
        SyncTestRunEventsHandler.instance.OnTestStarted(test.FullName);
    }

    public void TestFinished(ITestResult result)
    {
      if (!result.Test.IsSuite)
        SyncTestRunEventsHandler.instance.OnTestFinished();
    }
  }
}
#endif 