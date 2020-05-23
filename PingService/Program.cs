using System;
using Topshelf;


namespace PingService
{
    class Program
    {


        static void Main()
        {
            try
            {

                HostFactory.Run(serviceConfig =>
                {
                    serviceConfig.UseNLog();

                    serviceConfig.Service<PingService>(serviceInstance =>
                    {
                        serviceInstance.ConstructUsing(() =>
                            new PingService());

                        serviceInstance.WhenStarted(execute => execute.Start());
                        serviceInstance.WhenStopped(execute => execute.Stop());
                    });

                    serviceConfig.SetServiceName("PingService");
                    serviceConfig.SetDisplayName("Ping service");
                    serviceConfig.SetDescription("Simple service for sending request for wake up IIS AppPool after recycling");
                    serviceConfig.EnableServiceRecovery(recovery =>
                    {
                        recovery.RestartService(5);
                        recovery.RestartService(10);
                        recovery.RestartService(15);
                    });

                    serviceConfig.StartAutomatically();
                    serviceConfig.RunAsLocalSystem();
                    serviceConfig.SetStartTimeout(new TimeSpan(0, 0, 0, 90));


                });

            }
            catch ( Exception e )
            {
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Data);
                throw;
            }
        }


    }
}
