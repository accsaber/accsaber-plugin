
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace AccSaber.Utils
{
    public static class UnityExtensions
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOperation)
        {
            var taskCompletionSource = new TaskCompletionSource<byte>();
            asyncOperation.completed += (obj =>
            {
                taskCompletionSource.SetResult(0);
            });

            return ((Task)taskCompletionSource.Task).GetAwaiter();
        }
    }
}
