namespace EduTakip.Services
{
    public class ToastItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = "info"; // success, error, warning, info
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Duration { get; set; } = 3000; // milliseconds, 0 = no auto dismiss
    }

    public interface IToastService
    {
        void ShowSuccess(string title, string? message = null, int duration = 3000);
        void ShowError(string title, string? message = null, int duration = 4000);
        void ShowWarning(string title, string? message = null, int duration = 4000);
        void ShowInfo(string title, string? message = null, int duration = 3000);
        event Action<ToastItem>? OnShow;
    }

    public class ToastService : IToastService
    {
        private readonly List<Action<ToastItem>> _subscribers = new();
        private readonly object _lock = new();

        public event Action<ToastItem>? OnShow
        {
            add
            {
                lock (_lock)
                {
                    if (value != null)
                    {
                        _subscribers.Add(value);
                    }
                }
            }
            remove
            {
                lock (_lock)
                {
                    if (value != null)
                    {
                        _subscribers.Remove(value);
                    }
                }
            }
        }

        public void ShowSuccess(string title, string? message = null, int duration = 3000)
        {
            Show(new ToastItem { Type = "success", Title = title, Message = message ?? string.Empty, Duration = duration });
        }

        public void ShowError(string title, string? message = null, int duration = 4000)
        {
            Show(new ToastItem { Type = "error", Title = title, Message = message ?? string.Empty, Duration = duration });
        }

        public void ShowWarning(string title, string? message = null, int duration = 4000)
        {
            Show(new ToastItem { Type = "warning", Title = title, Message = message ?? string.Empty, Duration = duration });
        }

        public void ShowInfo(string title, string? message = null, int duration = 3000)
        {
            Show(new ToastItem { Type = "info", Title = title, Message = message ?? string.Empty, Duration = duration });
        }

        private void Show(ToastItem toast)
        {
            Action<ToastItem>[] subscribers;
            lock (_lock)
            {
                subscribers = _subscribers.ToArray();
            }

            foreach (var subscriber in subscribers)
            {
                try
                {
                    subscriber?.Invoke(toast);
                }
                catch
                {
                    // Ignore errors from individual subscribers
                }
            }
        }
    }
}

