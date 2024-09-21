using Coravel.Invocable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServiceBcp
{
    public class SchedualWork : IInvocable
    {
        public async Task Invoke()
        {
            Console.WriteLine(DateTime.Now);
            await Task.CompletedTask;
        }
    }
}
