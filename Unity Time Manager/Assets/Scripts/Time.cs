using System;

namespace TheAshBotAssets.TimeTracker
{
    [Serializable]
    /// <summary>
    /// This uses 48 bits of data to store the time. It give 0 bits to Milliseconds (AKA does not track milliseconds) 1 byte to seconds, 1 byte to minutes, 1 byte to hours, 2 bytes to days, 1 byte to years.
    /// </summary>
    public struct Time48Bit
    {


        public sbyte seconds;
        public sbyte minute;
        public sbyte hours;
        public ushort days;
        public byte years;


        public static Time48Bit GetDifferenceFrom2DateTimes(DateTime minuend, DateTime subtrahend)
        {
            Time48Bit time48Bit = new Time48Bit();

            int seconds = minuend.Second - subtrahend.Second;

            time48Bit.seconds = Convert.ToSByte(minuend.Second - subtrahend.Second);
            time48Bit.minute = Convert.ToSByte(minuend.Minute - subtrahend.Minute);
            time48Bit.hours = Convert.ToSByte(minuend.Hour - subtrahend.Hour);
            time48Bit.days = Convert.ToUInt16(minuend.DayOfYear - subtrahend.DayOfYear);
            time48Bit.years = Convert.ToByte(minuend.Year - subtrahend.Year);


            time48Bit = Carry(time48Bit);

            return time48Bit;
        }




        public static explicit operator Time48Bit(Time32Bit time32Bit)
        {
            // Make inline if needs more performance
            int secondsMask = 1 | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5);
            int minutesMask = (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11);
            int hoursMask = (1 << 12) | (1 << 13) | (1 << 14) | (1 << 15) | (1 << 16);
            int daysMask = (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20) | (1 << 21) | (1 << 22) | (1 << 23) | (1 << 24) | (1 << 25);
            int yearsMask = (1 << 26) | (1 << 27) | (1 << 28) | (1 << 29) | (1 << 30) | (1 << 31);

            Time48Bit time48Bit = new Time48Bit();
            time48Bit.seconds = Convert.ToSByte(secondsMask & time32Bit.time);
            time48Bit.minute = Convert.ToSByte((minutesMask & time32Bit.time) >> 6);
            time48Bit.hours = Convert.ToSByte((hoursMask & time32Bit.time) >> 12);
            time48Bit.days = Convert.ToUInt16((daysMask & time32Bit.time) >> 17);
            time48Bit.years = Convert.ToByte((yearsMask & time32Bit.time) >> 26);

            return time48Bit;
        }

        public static bool operator ==(Time48Bit a, Time48Bit b)
        {
            if (a.seconds == b.seconds && a.minute == b.minute && a.hours == b.hours && a.days == b.days && a.years == b.years)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Time48Bit a, Time48Bit b)
        {
            if (a.seconds != b.seconds || a.minute != b.minute || a.hours != b.hours || a.days != b.days || a.years != b.years)
            {
                return true;
            }
            return false;
        }
        public static Time48Bit operator +(Time48Bit a, Time48Bit b)
        {
            a.seconds += b.seconds;
            a.minute += b.minute;
            a.hours += b.hours;
            a.days += b.days;
            a.years += b.years;

            a = Carry(a);

            return a;
        }
        public static Time48Bit operator -(Time48Bit a, Time48Bit b)
        {
            a.seconds -= b.seconds;
            a.minute -= b.minute;
            a.hours -= b.hours;
            a.days -= b.days;
            a.years -= b.years;

            a = Carry(a);

            return a;
        }


        public static Time48Bit Carry(Time48Bit time48Bit)
        {
            while (time48Bit.seconds >= 60)
            {
                time48Bit.seconds -= 60;
                time48Bit.minute++;
            }
            while (time48Bit.seconds < 0)
            {
                time48Bit.seconds += 60;
                time48Bit.minute--;
            }

            while (time48Bit.minute >= 60)
            {
                time48Bit.minute -= 60;
                time48Bit.hours++;
            }
            while (time48Bit.minute < 0)
            {
                time48Bit.minute += 60;
                time48Bit.hours--;
            }

            while (time48Bit.hours >= 24)
            {
                time48Bit.hours -= 24;
                time48Bit.days++;
            }
            while (time48Bit.hours < 0)
            {
                time48Bit.hours += 24;
                time48Bit.days--;
            }

            while (time48Bit.days >= 365)
            {
                time48Bit.days -= 365;
                time48Bit.years++;
            }
            while (time48Bit.days < 0)
            {
                time48Bit.days += 365;
                time48Bit.years--;
            }

            return time48Bit;
        }

        public override string ToString()
        {
            string resalt = "";
            string units = "";
            if (years != 0)
            {
                resalt += years.ToString() + ":" + days.ToString() + ":" + hours.ToString("D2") + ":" + minute.ToString("D2") + ":" + seconds.ToString("D2");
                units += "Y:D:H:M:S";
            }
            else if (days != 0)
            {
                resalt += days.ToString() + ":" + hours.ToString("D2") + ":" + minute.ToString("D2") + ":" + seconds.ToString("D2");
                units += "D:H:M:S";
            }
            else if (hours != 0)
            {
                resalt += hours.ToString() + ":" + minute.ToString("D2") + ":" + seconds.ToString("D2");
                units += "H:M:S";
            }
            else if (minute != 0)
            {
                resalt += minute.ToString() + ":" + seconds.ToString("D2");
                units += "M:S";
            }
            else if (seconds != 0)
            {
                resalt += seconds.ToString("D1");
                units += "S";
            }
            resalt += " " + units;
            return resalt;
        }
        public override int GetHashCode()
        {
            return ((Time32Bit)this).time;
        }
        public override bool Equals(object obj)
        {
            if (obj is Time48Bit time48Bit)
            {
                return time48Bit == this;
            }
            else if (obj is Time32Bit time32Bit)
            {
                return (Time48Bit)time32Bit == this;
            }
            return false;
        }
    }

    [Serializable]
    /// <summary>
    /// This uses 32 bits of data to store the time. It give 0 bits to Milliseconds (AKA does not track milliseconds) 6 bits to seconds, 6 bits to minutes, 5 bits to hours, 9 bits to days, 6 bit to year
    /// </summary>
    public struct Time32Bit
    {
        /// <summary>
        /// 6 bits (0-5) are to the seconds. 6 bits (6-11) are to minutes. 5 bits (12-16) are to hours. 9 bits (17-25) are to days. 6 bits (26-31) are to days.
        /// </summary>
        public int time;


        public override string ToString()
        {
            Time48Bit time48Bit = (Time48Bit)this;
            return time48Bit.ToString();
        }

        public static explicit operator Time32Bit(Time48Bit time48Bit)
        {
            Time32Bit time32Bit = new Time32Bit();
            time32Bit.time = 0;
            time32Bit.time |= time48Bit.seconds << 0;
            time32Bit.time |= time48Bit.minute << 6;
            time32Bit.time |= time48Bit.hours << 12;
            time32Bit.time |= time48Bit.days << 17;
            time32Bit.time |= time48Bit.years << 26;

            return time32Bit;
        }
    }

}