using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmdbApi.Api
{
    public class Recurring_Job
    {
        public Recurring_Job()
        {
            //Recurring job
            RecurringJob.AddOrUpdate(() => ProcessRecurringJob(), "*/10 * * * *");
        }

        public void ProcessRecurringJob()
        {
            Console.WriteLine("I am a Recurring Job !!");
        }
    }
}
