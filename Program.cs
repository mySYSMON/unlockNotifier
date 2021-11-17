using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
namespace ConsoleApp2
{
    class Program
    {
        public class Thread1
        {
            public static void DoWork()
            {
                while (true)
                {
                    Console.WriteLine("getting events");
                    var logName = "Security";
                    var query =
                    "*[System[(EventID=4801) and " +
                    "TimeCreated[timediff(@SystemTime) <= 300000]]]";
                    var logQuery = new EventLogQuery(logName,
                    PathType.LogName, query);
                    var logReader = new EventLogReader(logQuery);
                    var eventCounter = 0;
                    var records = new List<EventRecord>();
                    for (var er = logReader.ReadEvent(); null != er; er = logReader.ReadEvent())
                    {
                        records.Add(er);
                        eventCounter++;
                    }
                    Console.WriteLine(eventCounter + " events");
                    if (eventCounter > 0)
                    {
                        var accountSid = "ABC123-CHANGEME";
                        var authToken = "ABC123-CHANGEME";
                        TwilioClient.Init(accountSid, authToken);
                        var message = MessageResource.Create(
                        body: "Unlocked your PC",
                        from: new Twilio.Types.PhoneNumber("+12228675309"),
                        to: new Twilio.Types.PhoneNumber("+13338675309")
                        );
                    }
                    Thread.Sleep(250000);
                }
            }
        }
        static void Main(string[] args)
        {
            var p = Process.GetCurrentProcess();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            Console.WriteLine("Priority is set to: " + p.PriorityClass);
            var thread1 = new Thread(Thread1.DoWork);
            thread1.Start();
        }
    }
}
