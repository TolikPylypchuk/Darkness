using System;
using System.Threading.Tasks;

using BrowserInterop.Extensions;

using Microsoft.JSInterop;

namespace Darkness.History
{
    public sealed class BrowserHistoryService<T> : IHistoryService<T>
    {
        private readonly IJSRuntime jsRuntime;

        public BrowserHistoryService(IJSRuntime jsRuntime) =>
            this.jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));

        public async ValueTask Push(T value, string title)
        {
            var window = await this.jsRuntime.Window();
            await window.History.PushState(value, title);
        }

        public async ValueTask<IAsyncDisposable> OnPop(Func<T, ValueTask> callback)
        {
            var window = await this.jsRuntime.Window();
            return await window.OnPopState<T>(state =>
                state != null ? callback(state) : ValueTask.CompletedTask);
        }
    }
}
