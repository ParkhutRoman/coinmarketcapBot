using System;
namespace coinmarketcapBot.Models
{
    [Serializable]
    public sealed class MyDateTime
    {
        public MyDateTime()
        {
            this.Now = DateTime.Now;
            this.IsDaylightSavingTime = this.Now.IsDaylightSavingTime();
            this.TimeZone = this.IsDaylightSavingTime
                ? System.TimeZone.CurrentTimeZone.DaylightName
                : System.TimeZone.CurrentTimeZone.StandardName;
        }

        public DateTime Now
        {
            get;

            set;
        }

        public string TimeZone
        {
            get;

            set;
        }

        public bool IsDaylightSavingTime
        {
            get;

            set;
        }
    }
}