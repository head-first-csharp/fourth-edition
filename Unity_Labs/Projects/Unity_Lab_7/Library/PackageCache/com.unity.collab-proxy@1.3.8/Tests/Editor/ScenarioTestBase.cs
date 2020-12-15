using System;

namespace Unity.Cloud.Collaborate.Tests
{
    public class ScenarioTestBase
    {
//        protected AsyncToCoroutine atc;
//
//        [SetUp]
//        public void Setup()
//        {
//            atc = new AsyncToCoroutine();
//        }
//
//        [OneTimeSetUp]
//        public void OneTimeSetup()
//        {
//            SourceControlGitImplementation.IsRunningTests = true;
//        }
//
//        [OneTimeTearDown]
//        public void OneTimeTearDown()
//        {
//            SourceControlGitImplementation.IsRunningTests = false;
//        }
//
//        protected async Task EnsureCleanChangesPageInitially()
//        {
//            Threading.mainThreadId = Thread.CurrentThread.ManagedThreadId;
//
//            if (!BackendProvider.Instance.IsProviderInstalled())
//            {
//                BackendProvider.Instance.InstallProvider();
//            }
//
//            // set git identity - using RyanC test dedicated account.
//            BackendProvider.Instance.SetGitNameAndEmail("ryancas+collabtest", "ryancas+collabtest@unity3d.com");
//
//            // ensure clean state , todo - ahmad : add this to a setup/teardown pair of methods.
//            await BackendProvider.Instance.CreateRepository();
//            await BackendProvider.Instance.InitializeClient();
//
//            BackendProvider.Instance.DoesRepositoryExist().ShouldBe(true, "Repository is not initialized");
//            BackendProvider.Instance.IsClientInitialized().ShouldBe(true, "Git is not initialized");
//
//            BackendProvider.Instance.Start();
//
//            // ensure clean state by publishing everything.
//            await BackendProvider.Instance.Publish("initial publish");
//
//            // assert clean state.
//            (await BackendProvider.Instance.GetChanges()).Count.ShouldBe(0, "file change count is not zero");
//        }

    }
}
