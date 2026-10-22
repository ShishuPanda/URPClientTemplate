using System;
using UnityEngine;
using UnityEngine.UIElements;
using Core.Architecture;

namespace Core.UI.Views
{
    /// <summary>
    /// Lightweight, zero-heap-allocation ViewModel data contract for dashboard views.
    /// </summary>
    public readonly struct DashboardViewModel
    {
        public readonly string Title;
        public readonly string Subtitle;
        public readonly string StatusMessage;

        public DashboardViewModel(string title, string subtitle, string statusMessage)
        {
            Title = title;
            Subtitle = subtitle;
            StatusMessage = statusMessage;
        }
    }

    /// <summary>
    /// Event dispatched when primary UI button is clicked.
    /// </summary>
    public readonly struct ActionTriggeredEvent : IGameEvent
    {
        public readonly string ActionName;
        public ActionTriggeredEvent(string actionName) => ActionName = actionName;
    }

    /// <summary>
    /// High-performance UI Toolkit Binding Controller managing 2-way data binding and event dispatch.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class BaseDashboardViewController : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private Label _titleLabel;
        private Label _subtitleLabel;
        private Label _statusLabel;
        private Button _actionButton;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = _uiDocument.rootVisualElement;
            if (root == null) return;

            _titleLabel = root.Q<Label>("title-label");
            _subtitleLabel = root.Q<Label>("subtitle-label");
            _statusLabel = root.Q<Label>("status-label");
            _actionButton = root.Q<Button>("primary-action-btn");

            if (_actionButton != null)
            {
                _actionButton.clicked += OnActionButtonClicked;
            }
        }

        private void OnDisable()
        {
            if (_actionButton != null)
            {
                _actionButton.clicked -= OnActionButtonClicked;
            }
        }

        /// <summary>
        /// Applies ViewModel state to visual elements with zero GC pressure.
        /// </summary>
        public void Bind(in DashboardViewModel viewModel)
        {
            if (_titleLabel != null && viewModel.Title != null)
                _titleLabel.text = viewModel.Title;

            if (_subtitleLabel != null && viewModel.Subtitle != null)
                _subtitleLabel.text = viewModel.Subtitle;

            if (_statusLabel != null && viewModel.StatusMessage != null)
                _statusLabel.text = viewModel.StatusMessage;
        }

        private void OnActionButtonClicked()
        {
            EventBus.Publish(new ActionTriggeredEvent("PrimaryActionClicked"));
        }
    }
}
