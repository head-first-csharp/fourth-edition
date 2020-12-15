using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.Models.Providers;
using UnityEngine.TestTools;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    internal class StartModelTests
    {
        TestCollab m_Provider;
        AsyncToCoroutine m_Atc;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            m_Atc = new AsyncToCoroutine();
        }

        [SetUp]
        public void Setup()
        {
            m_Provider = new TestCollab();
        }

        [TearDown]
        public void TearDown()
        {
            m_Provider = null;
        }

        [UnityTest]
        public IEnumerator TestWhenProjectBoundSavesAssets()
        {
            return m_Atc.Run(async () =>
            {
                var saveAssetsCallCount = 0;

                //ensure we return true for isProjectBound
                m_Provider.isProjectBoundTestImpl = () => ProjectStatus.Bound;
                m_Provider.saveAssetsTestImpl = () => saveAssetsCallCount++;

                saveAssetsCallCount.ShouldBe(0);
                await m_Provider.TestRequestTurnOnService();
                saveAssetsCallCount.ShouldBe(1, $"Expected {nameof(saveAssetsCallCount)} to be 1");
            });
        }

        [UnityTest]
        public IEnumerator TestWhenGenesisReturnsForbiddenShowsCredentialsError()
        {
            return m_Atc.Run(async () =>
            {
                var showCredentialsErrorCallCount = 0;
                var putAsyncCallCount = 0;

                //ensure we return true for isProjectBound
                m_Provider.isProjectBoundTestImpl = () => ProjectStatus.Bound;
                m_Provider.showCredentialsErrorTestImpl = () => showCredentialsErrorCallCount++;

                m_Provider.putAsyncTestImpl = () =>
                {
                    putAsyncCallCount++;
                    return Task.Run(() => new HttpResponseMessage(HttpStatusCode.Forbidden));
                };

                putAsyncCallCount.ShouldBe(0);
                showCredentialsErrorCallCount.ShouldBe(0);
                await m_Provider.TestRequestTurnOnService();

                putAsyncCallCount.ShouldBe(1, $"Expected {nameof(putAsyncCallCount)} to be 1");
                showCredentialsErrorCallCount.ShouldBe(1, $"Expected {nameof(showCredentialsErrorCallCount)} to be 1");
            });
        }

        [UnityTest]
        public IEnumerator TestsWhenGenesisReturnsErrorShowsGenericError()
        {
            return m_Atc.Run(async () =>
            {
                var showGeneralErrorCallCount = 0;
                var putAsyncCallCount = 0;

                //ensure we return true for isProjectBound
                m_Provider.isProjectBoundTestImpl = () => ProjectStatus.Bound;
                m_Provider.showGeneralErrorTestImpl = () =>
                {
                    showGeneralErrorCallCount++;
                };

                m_Provider.putAsyncTestImpl = () =>
                {
                    putAsyncCallCount++;
                    return Task.Run(() => new HttpResponseMessage(HttpStatusCode.NotFound));
                };

                putAsyncCallCount.ShouldBe(0);
                showGeneralErrorCallCount.ShouldBe(0);
                await m_Provider.TestRequestTurnOnService();
                putAsyncCallCount.ShouldBe(1, $"Expected {nameof(putAsyncCallCount)} to be 1");
                showGeneralErrorCallCount.ShouldBe(1, $"Expected {nameof(showGeneralErrorCallCount)} to be 1");
            });
        }

        [UnityTest]
        public IEnumerator TestWhenGenesisReturnsOkCallsTurnOnCollabInternal()
        {
            return m_Atc.Run(async () =>
            {
                var putAsyncCallCount = 0;
                var turnOnCollabInternalCallCount = 0;

                // Ensure we return true for isProjectBound.
                m_Provider.isProjectBoundTestImpl = () => ProjectStatus.Bound;
                m_Provider.turnOnCollabInternalTestImpl = () => turnOnCollabInternalCallCount++;

                m_Provider.putAsyncTestImpl = () =>
                {
                    putAsyncCallCount++;
                    return Task.Run(() => new HttpResponseMessage(HttpStatusCode.OK));
                };

                putAsyncCallCount.ShouldBe(0);
                turnOnCollabInternalCallCount.ShouldBe(0);
                await m_Provider.TestRequestTurnOnService();
                putAsyncCallCount.ShouldBe(1, $"Expected {nameof(putAsyncCallCount)} to be 1");
                turnOnCollabInternalCallCount.ShouldBe(1, $"Expected {nameof(turnOnCollabInternalCallCount)} to be 1");
            });
        }

        class TestCollab : Collab
        {
            public Func<ProjectStatus> isProjectBoundTestImpl;
            public Action saveAssetsTestImpl;
            public Action turnOnCollabInternalTestImpl;
            public Func<Task<HttpResponseMessage>> putAsyncTestImpl;
            public Action showCredentialsErrorTestImpl;
            public Action showGeneralErrorTestImpl;

            public TestCollab()
            {
                isProjectBoundTestImpl = () => ProjectStatus.Unbound;
                saveAssetsTestImpl = () => { };
                turnOnCollabInternalTestImpl = () => { };
                showCredentialsErrorTestImpl = () => { };
                showGeneralErrorTestImpl = () => { };

                putAsyncTestImpl = () => Task.Run(() => new HttpResponseMessage());
            }

            public async Task TestRequestTurnOnService()
            {
                await RequestTurnOnServiceInternal();
            }

            protected override Task<HttpResponseMessage> PutAsync(HttpClient client, string fullUrl, StringContent content)
            {
                return putAsyncTestImpl?.Invoke();
            }

            protected override void TurnOnCollabInternal()
            {
                turnOnCollabInternalTestImpl?.Invoke();
            }

            protected override void SaveAssets()
            {
                saveAssetsTestImpl?.Invoke();
            }

            public override ProjectStatus GetProjectStatus()
            {
                return isProjectBoundTestImpl?.Invoke() ?? ProjectStatus.Unbound;
            }

            protected override void ShowCredentialsError()
            {
                showCredentialsErrorTestImpl?.Invoke();
            }

            protected override void ShowGeneralError()
            {
                showGeneralErrorTestImpl?.Invoke();
            }
        }
    }
}
