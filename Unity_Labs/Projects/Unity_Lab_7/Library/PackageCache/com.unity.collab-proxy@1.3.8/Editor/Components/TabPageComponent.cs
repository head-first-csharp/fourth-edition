using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    abstract class TabPageComponent : VisualElement
    {
        /// <summary>
        /// Current active status for this page.
        /// </summary>
        protected bool Active { get; private set; }

        /// <summary>
        /// Set active status of this page.
        /// </summary>
        /// <param name="active">True if the page is to be active.</param>
        public void SetActive(bool active)
        {
            Active = active;
            if (Active)
            {
                SetActive();
            }
            else
            {
                SetInactive();
            }
        }

        /// <summary>
        /// Set this tab page active.
        /// </summary>
        protected abstract void SetActive();

        /// <summary>
        /// Set this tab page inactive.
        /// </summary>
        protected abstract void SetInactive();
    }
}
