namespace Unity.Cloud.Collaborate.Presenters
{
    internal interface IPresenter
    {
        /// <summary>
        /// Called when the view is ready to receive data. For example when it comes into view.
        /// </summary>
        void Start();

        /// <summary>
        /// Called when the view is no longer available to receive data. For example when it goes out of view.
        /// </summary>
        void Stop();
    }
}
