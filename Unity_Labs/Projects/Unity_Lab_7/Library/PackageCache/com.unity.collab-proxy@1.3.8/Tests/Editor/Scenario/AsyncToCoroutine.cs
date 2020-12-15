using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Collaborate.Tests
{
    public class CoroutineSynchronizationContext : SynchronizationContext
    {
        const int k_AwqInitialCapacity = 20;
        readonly Queue<WorkRequest> m_AsyncWorkQueue = new Queue<WorkRequest>(k_AwqInitialCapacity);
        readonly int m_MainThreadId = Thread.CurrentThread.ManagedThreadId;

        // Send will process the call synchronously. If the call is processed on the main thread, we'll invoke it
        // directly here. If the call is processed on another thread it will be queued up like POST to be executed
        // on the main thread and it will wait. Once the main thread processes the work we can continue
        public override void Send(SendOrPostCallback callback, object state)
        {
            if (m_MainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                callback(state);
            }
            else
            {
                using (var waitHandle = new ManualResetEvent(false))
                {
                    lock (m_AsyncWorkQueue)
                    {
                        m_AsyncWorkQueue.Enqueue(new WorkRequest(callback, state, waitHandle));
                    }
                    waitHandle.WaitOne();
                }
            }
        }

        // Post will add the call to a task list to be executed later on the main thread then work will continue asynchronously
        public override void Post(SendOrPostCallback callback, object state)
        {
            lock (m_AsyncWorkQueue)
            {
                m_AsyncWorkQueue.Enqueue(new WorkRequest(callback, state));
            }
        }

        // Exec will execute tasks off the task list
        public void Exec()
        {
            lock (m_AsyncWorkQueue)
            {
                var workCount = m_AsyncWorkQueue.Count;
                for (var i = 0; i < workCount; i++)
                {
                    var work = m_AsyncWorkQueue.Dequeue();
                    work.Invoke();
                }
            }
        }

        struct WorkRequest
        {
            readonly SendOrPostCallback m_DelegateCallback;
            readonly object m_DelegateState;
            readonly ManualResetEvent m_WaitHandle;

            public WorkRequest(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null)
            {
                m_DelegateCallback = callback;
                m_DelegateState = state;
                m_WaitHandle = waitHandle;
            }

            public void Invoke()
            {
                m_DelegateCallback(m_DelegateState);
                m_WaitHandle?.Set();
            }
        }
    }

    public class AsyncToCoroutine
    {
        readonly CoroutineSynchronizationContext m_Context;
        readonly bool m_Cleanup;

        public Func<Task> Before { get; set; }
        public Func<Task> After { get; set; }

        public AsyncToCoroutine(bool cleanup = true)
        {
            m_Context = new CoroutineSynchronizationContext();
            m_Cleanup = cleanup;
        }

        public IEnumerator Run(Func<Task> func)
        {
            return Run(func, null);
        }

        // This method exists so we can avoid getting compiler warnings
        // from non-async tests even if their Before/After are async
        public IEnumerator Run(Action action)
        {
            return Run(null, action);
        }

        IEnumerator Run(Func<Task> func, Action action)
        {
            var oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(m_Context);

            var task = RunAll();
            while (!task.IsCompleted)
            {
                // Run any code that''s queued up.
                m_Context.Exec();

                // Skip frames until we're given more code or the task is done.
                yield return null;
            }

            SynchronizationContext.SetSynchronizationContext(oldContext);

            // Rethrow any exception that happened inside the task.
            // Using a regular throw statement will lose the stack trace.
            // https://stackoverflow.com/a/20170583
            if (task.IsFaulted)
                ExceptionDispatchInfo.Capture(task.Exception?.InnerException ?? task.Exception ?? new Exception("Unknown exception!")).Throw();

            // Stupid hack to handle before/after methods that are async
            // Ideally NUnit would do that for us but it doesn't support
            // coroutines in it's Setup/Teardown methods.
            async Task RunAll()
            {
                if (Before != null) await Before();

                if (func != null)
                    await func();
                else
                    action();

                if (After != null) await After();
            }
        }
    }
}
