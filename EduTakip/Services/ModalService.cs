using Microsoft.AspNetCore.Components;

namespace EduTakip.Services
{
    public class ModalOptions
    {
        public string Title { get; set; } = "";
        public string CssClass { get; set; } = "modal-md";
    }

    public class ModalService
    {
        public event Action? OnChange;
        public bool IsOpen { get; private set; }
        public string Title { get; private set; } = "";
        public string CssClass { get; private set; } = "modal-md";
        public Type? ComponentType { get; private set; }
        public Dictionary<string, object>? Parameters { get; private set; }
        private TaskCompletionSource<object?>? _tcs;

        public Task<object?> OpenAsync<T>(string title, Dictionary<string, object>? parameters = null, ModalOptions? options = null) where T : ComponentBase
        {
            ComponentType = typeof(T);
            Parameters = parameters;
            Title = title;
            CssClass = options?.CssClass ?? "modal-md";
            IsOpen = true;
            _tcs = new TaskCompletionSource<object?>();
            OnChange?.Invoke();
            return _tcs.Task;
        }

        public void Close(object? result = null)
        {
            IsOpen = false;
            ComponentType = null;
            Parameters = null;
            OnChange?.Invoke();
            _tcs?.TrySetResult(result);
        }
    }
}
