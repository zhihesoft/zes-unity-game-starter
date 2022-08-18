using System;
using System.Threading.Tasks;

namespace Zes
{
    public class PendingItem<T>
    {
        public T item = default(T);
        public bool pending = true;
        public long startTime = Util.Timestamp();

        public async Task Wait()
        {
            await Util.WaitUntil(() => !pending);
        }

        public void Set(T data)
        {
            item = data;
            pending = false;
        }
    }
}
