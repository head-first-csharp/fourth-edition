using Unity.Cloud.Collaborate.Presenters;

namespace Unity.Cloud.Collaborate.Views
{
    internal interface IStartView : IView<IStartPresenter>
    {
        /// <summary>
        /// Set the text for the view.
        /// </summary>
        string Text { set; }

        /// <summary>
        /// Set the text for the button in the view.
        /// </summary>
        string ButtonText { set; }

        /// <summary>
        /// Set the visibility of the button.
        /// </summary>
        /// <param name="isVisible">True if the button should be visible.</param>
        void SetButtonVisible(bool isVisible);
    }
}
