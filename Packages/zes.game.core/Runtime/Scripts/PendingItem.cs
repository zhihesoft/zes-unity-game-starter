using System.Threading.Tasks;

namespace Zes
{
    internal class PendingItem<T>
    {
        public T item = default(T);
        public bool pending = true;

        public async Task Wait()
        {
            await Util.WaitUntil(() => !pending);
        }

        public void SetDate(T data)
        {
            item = data;
            pending = false;
        }
    }
}
