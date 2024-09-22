using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Data.SqlClient;

namespace WorkerServiceBcp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string source = "localhost,1433";
            string U = "sa";
            string P = "qa852741";


            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("MyConnStr"));
            conn.Open();
            IEnumerable<string> tableNames = conn.Query<string>(@"SELECT name FROM SYS.tables WHERE name != 'Log'");

            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Bcp");
            Directory.CreateDirectory(basePath);
            string dataPath = Directory.CreateDirectory(Path.Combine(basePath, "Data")).FullName;
            string format = Directory.CreateDirectory(Path.Combine(basePath, "Format")).FullName;
            DateTime dt1 = DateTime.Now;    
            _logger.LogInformation($"開始時間: {dt1}");

            //foreach (string tbname in tableNames)
            //{
            //    //// 產生bcp
            //    //string cmd1 = $"/c bcp CopyNothwind.dbo.{tbname} out {Path.Combine(dataPath, tbname)}.bcp -S {source} -U {U} -P {P} -c";
            //    //cmd1.Excute("cmd.exe");
            //    //// 產生format
            //    //string cmd2 = $"/c bcp YuDB.dbo.{tbname} format nul -f {Path.Combine(format, tbname)}.fmt -S {source} -U {U} -P {P} -c";
            //    //cmd2.Excute("cmd.exe");
            //    // 寫入db
            //    string cmd3 = $"/c bcp YuDB.dbo.{tbname} in {Path.Combine(dataPath, tbname)}.bcp -f {Path.Combine(format, tbname)}.fmt -S {source} -U {U} -P {P}";
            //    cmd3.Excute("cmd.exe");
            //}

            Parallel.ForEach(tableNames, tbname =>
            {
                try
                {
                    //// 產生bcp
                    //string cmd1 = $"/c bcp CopyNothwind.dbo.{tbname} out {Path.Combine(dataPath, tbname)}.bcp -S {source} -U {U} -P {P} -c";
                    //cmd1.Excute("cmd.exe");
                    //// 產生format
                    //string cmd2 = $"/c bcp YuDB.dbo.{tbname} format nul -f {Path.Combine(format, tbname)}.fmt -S {source} -U {U} -P {P} -c";
                    //cmd2.Excute("cmd.exe");
                    // 寫入db
                    string cmd3 = $"/c bcp YuDB.dbo.{tbname} in {Path.Combine(dataPath, tbname)}.bcp -f {Path.Combine(format, tbname)}.fmt -S {source} -U {U} -P {P}";
                    cmd3.Excute("cmd.exe");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            });


            DateTime dt2 = DateTime.Now;
            _logger.LogInformation($"開始時間: {dt2}");
            _logger.LogInformation($"耗時: {(dt2 - dt1).TotalSeconds}");
            await Task.CompletedTask;
        }
    }
}
