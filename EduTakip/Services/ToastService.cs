namespace EduTakip.Services
{
    public enum ToastSeverity { Success, Error, Warning, Info }

    public class ToastItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ToastSeverity Severity { get; set; }
        public string Summary { get; set; } = "";
        public string Detail { get; set; } = "";
        public int Duration { get; set; } = 4000;
        public bool IsExiting { get; set; }
    }

    public class ToastService
    {
        public event Action? OnChange;
        public List<ToastItem> Toasts { get; } = new();

        public void Notify(ToastSeverity severity, string summary, string detail, int duration = 4000)
        {
            var toast = new ToastItem
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = duration
            };
            Toasts.Add(toast);
            OnChange?.Invoke();
        }

        public void Remove(string id)
        {
            Toasts.RemoveAll(t => t.Id == id);
            OnChange?.Invoke();
        }
    }
}
