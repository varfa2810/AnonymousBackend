using AnonymousApplication.Interfaces;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;

namespace HangfireWorker
{
    public static class JobScheduler
    {
        public static void Register(IRecurringJobManager manager)
        {
            manager.AddOrUpdate<IHangFire>(
                "delete-old-messages",
                x => x.DeleteOldMessages(),
                Cron.Daily
            );

           
        }
    }
}
