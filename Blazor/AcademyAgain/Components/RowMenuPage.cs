using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.JSInterop;

namespace AcademyAgain.Components
{
    public abstract class RowMenuPage<T> : ComponentBase, IAsyncDisposable where T : class
    {
        [Inject]
        private IJSRuntime Js { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider Auth { get; set; } = default!;

        private DotNetObjectReference<RowMenuPage<T>>? _ref;

        public PaginationState Pagination { get; } = new() { ItemsPerPage = 25 };

        public int TotalCount { get; protected set; }

        public string? SearchText { get; set; }

        protected string? SelectedKey { get; private set; }
        protected List<RowMenuItem>? MenuItems { get; private set; }
        protected double MenuX { get; private set; }
        protected double MenuY { get; private set; }
        protected bool MenuOpen => MenuItems is { Count: > 0 };

        protected bool CanWrite { get; private set; } = true;
        protected bool CanAdmin { get; private set; }

        protected abstract string GetKey(T item);

        protected abstract List<RowMenuItem> BuildMenu(string key);

        protected string RowClass(T item) => GetKey(item) == SelectedKey ? "row-selected" : "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var authTask = RefreshAuthAsync();
                _ref = DotNetObjectReference.Create(this);
                await Js.InvokeVoidAsync("AcademyRowMenu.register", _ref);
                await authTask;
            }
        }

        private async Task RefreshAuthAsync()
        {
            var user = (await Auth.GetAuthenticationStateAsync()).User;
            CanWrite = user.IsInRole("admin") || user.IsInRole("teacher") || user.IsInRole("moderator");
            CanAdmin = user.IsInRole("admin");
        }

        [JSInvokable]
        public void RowSelect(string key)
        {
            SelectedKey = key;
            MenuItems = null;
            StateHasChanged();
        }

        [JSInvokable]
        public void RowContextMenu(string key, double x, double y)
        {
            SelectedKey = key;
            MenuItems = BuildMenu(key).Where(AllowedItem).ToList();
            MenuX = x;
            MenuY = y;
            StateHasChanged();
        }

        private bool AllowedItem(RowMenuItem item)
        {
            if (item.Url.Contains("/create") || item.Url.Contains("/edit") || item.Url.Contains("/delete"))
            {
                return CanWrite;
            }
            if (item.Url.StartsWith("database/") || item.Url.StartsWith("admin/"))
            {
                return CanAdmin;
            }
            return true;
        }

        protected void CloseMenu()
        {
            MenuItems = null;
            StateHasChanged();
        }

        protected virtual bool MatchesSearch(T item) => true;

        protected bool TextContains(params string?[] values)
        {
            var text = SearchText;
            if (string.IsNullOrWhiteSpace(text)) return true;
            return values.Any(v => !string.IsNullOrWhiteSpace(v)
                && v.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        protected IEnumerable<T> ApplySearch(IEnumerable<T> source)
            => string.IsNullOrWhiteSpace(SearchText) ? source : source.Where(MatchesSearch);

        protected async Task ResetPaginationAsync() => await Pagination.SetCurrentPageIndexAsync(0);

        protected async Task HandlePageSizeChanged(int newSize)
        {
            Pagination.ItemsPerPage = newSize;
            await Pagination.SetCurrentPageIndexAsync(0);
            StateHasChanged();
        }

        protected async Task ExportCsvAsync(string fileName, string content)
        {
            await Js.InvokeVoidAsync("downloadTextFile", fileName, content);
        }

        protected async ValueTask UnregisterMenuAsync()
        {
            if (_ref is not null)
            {
                try
                {
                    await Js.InvokeVoidAsync("AcademyRowMenu.unregister");
                }
                catch
                {
                }
                _ref.Dispose();
                _ref = null;
            }
        }

        public virtual async ValueTask DisposeAsync()
        {
            await UnregisterMenuAsync();
        }
    }

    public sealed record RowMenuItem(string Label, string Url, bool Danger = false);
}