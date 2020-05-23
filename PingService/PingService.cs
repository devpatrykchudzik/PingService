using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using Topshelf.Logging;
using Timer = System.Timers.Timer;

namespace PingService
{
    public class PingService
    {
        private static readonly LogWriter Logger = HostLogger.Get<PingService>();

        private Timer _timer;

        public bool Start()
        {
            try
            {
                Logger.Info("Starting service");
                var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
                Logger.Info($"Environment: {environment}");
                ConfigHelper.InitConfiguration(environment);

                _timer = new Timer();
                _timer.Elapsed += (sender, eventArgs) => DoWhenTimeout();
                _timer.Enabled = true;

                return true;
            }
            catch ( Win32Exception e )
            {
                Logger.Error(e);
                return true;
            }
            catch ( Exception e )
            {
                Logger.Error(e);
                return false;
            }

        }

        private void DoWhenTimeout()
        {
            _timer.Enabled = false;

            RequestApi();

            var interval = DateTime.Now.Date.Add(ConfigHelper.Config.WakingTimeParsed.TimeOfDay) - DateTime.Now;
            
            if ( interval.CompareTo(TimeSpan.Zero) < 0 )
                interval = interval.Add(TimeSpan.FromDays(1));

            _timer.Interval = interval.TotalMilliseconds;
            _timer.Enabled = true;

            Logger.Info($"Next wakeup scheduled after: {interval}");
        }

        private void RequestApi()
        {
            using ( var httpHandler = new HttpClientHandler() { UseDefaultCredentials = true } )
            using ( var httpClient = new HttpClient(httpHandler) )
            {
                for ( var i = 0 ; i < ConfigHelper.Config.TriesCount ; i++ )
                    try
                    {
                        Logger.Info($"Sending request: {ConfigHelper.Config.WakingUrl}");
                        Console.WriteLine("Request sent");
                        var response = httpClient.GetAsync(ConfigHelper.Config.WakingUrl).Result;
                        

                        Logger.Info($"Received response: {response.StatusCode}");
                        break;
                    }
                    catch ( Exception ex )
                    {
                        Logger.Error(ex);
                        if ( i < ConfigHelper.Config.TriesCount - 1 )
                        {
                            Logger.Info($"Waiting for retry number {i + 1}");
                            Thread.Sleep(5000);
                        }
                    }
            }
        }

        public bool Stop()
        {
            try
            {
                Logger.Info("Service stopped.");
                return true;
            }
            catch ( Exception e )
            {
                Logger.Error(e);
                return false;
            }

        }
    }




}

