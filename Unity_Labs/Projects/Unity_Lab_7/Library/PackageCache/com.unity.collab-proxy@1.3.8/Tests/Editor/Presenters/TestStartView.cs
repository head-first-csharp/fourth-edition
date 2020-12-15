using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Views;

namespace Unity.Cloud.Collaborate.Tests.Presenters
{
    internal class TestStartView : IStartView
    {
        public bool buttonVisible;

        public IStartPresenter Presenter { get; set; }

        public string Text { get; set; }

        public string ButtonText { get; set; }

        public void SetButtonVisible(bool isVisible)
        {
            buttonVisible = isVisible;
        }
    }
}
