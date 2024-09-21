using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServiceBcp
{
    public static class ProcessHelper
    {
        public static void Excute(this string CMD, string fileName)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = fileName,
                    Arguments = CMD
                }
            };

            process.Start();
            process.WaitForExit();
            process.Kill();
            process.Dispose();
            process.Close();
        }
    }
}
