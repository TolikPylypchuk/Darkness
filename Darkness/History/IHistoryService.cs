using System;
using System.Threading.Tasks;

namespace Darkness.History
{
    public interface IHistoryService<T>
    {
        public ValueTask Push(T value, string title);
        public ValueTask Back();
        public ValueTask<IAsyncDisposable> OnPop(Func<T, ValueTask> callback);
    }
}
