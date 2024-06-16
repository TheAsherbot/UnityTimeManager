using System;

namespace TheAshBotAssets.TimeTracker
{
    public struct DateTime88Bit
    {
        // The only thing that is set is Day of Year, Year, Hour, Minute, Second, Millisecond, 

        /// <summary>
        /// 9 bits (0-8) to days in year. 23 bits (9-24) to years
        /// </summary>
        public uint Date
        {
            get
            {
                uint date = 0;
                date |= (uint)(Day - 1) << 0;// 9 bits. day is Days - 1 in Year. I want 0-(Days in Year - 1)
                date |= (uint)(Year - 1) << 9;// 16 bits. day is - 1. I want 0-65535 (max number of Unsinged short)
                return date;
            }
            set
            {
                // Make inline if needs more performance
                int daysMask = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8);
                int yearMask = ~daysMask & ~((1 << 25) | (1 << 26) | (1 << 27) | (1 << 28) | (1 << 29) | (1 << 30) | (1 << 31));

                Day = Convert.ToInt16(value & daysMask);
                Year = Convert.ToUInt16((value & yearMask) >> 9);
            }
        }
        /// <summary>
        /// 1-365 (includes leap years)
        /// </summary>
        public short Day
        {
            get;
            set;
        }
        public ushort Year
        {
            get;
            private set;
        }
        public byte Month
        {
            get
            {
                short days = Day;
                bool moreDaysLeft = true;
                byte month = 0;
                while (moreDaysLeft)
                {
                    switch (month)
                    {
                        case 0: // January
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return month;
                        case 1: // February
                            if (days > ((Year % 4 == 0 && Year % 400 != 0) ? 29 : 28))
                            {
                                days -= (short)((Year % 4 == 0 && Year % 400 != 0) ? 29 : 28);
                                break;
                            }
                            return month;
                        case 2: // March
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return month;
                        case 3: // April
                            if (days > 30)
                            {
                                days -= 30;
                                break;
                            }
                            return month;
                        case 4: // May
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return month;
                        case 5: // June
                            if (days > 30)
                            {
                                days -= 30;
                                break;
                            }
                            return month;
                        case 6: // July
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return month;
                        case 7: // August
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return month;
                        case 8: // September
                            if (days > 30)
                            {
                                days -= 30;
                                break;
                            }
                            return month;
                        case 9: // October
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return month;
                        case 10: // November
                            if (days > 30)
                            {
                                days -= 30;
                                break;
                            }
                            return month;
                        case 11: // December
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return month;
                    }
                    month++;
                }
                return month;
            }
        }
        public byte DayOfMonth
        {
            get
            {
                short days = Day;
                bool moreDaysLeft = true;
                byte month = 0;
                while (moreDaysLeft)
                {
                    switch (month)
                    {
                        case 0: // January
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 1: // February
                            if (days > ((Year % 4 == 0 && Year % 400 != 0) ? 29 : 28))
                            {
                                days -= (short)((Year % 4 == 0 && Year % 400 != 0) ? 29 : 28);
                                break;
                            }
                            return Convert.ToByte(days);
                        case 2: // March
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 3: // April
                            if (days > 30)
                            {
                                days -= 30;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 4: // May
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 5: // June
                            if (days > 30)
                            {
                                days -= 30;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 6: // July
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 7: // August
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 8: // September
                            if (days > 30)
                            {
                                days -= 30;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 9: // October
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 10: // November
                            if (days > 30)
                            {
                                days -= 30;
                                break;
                            }
                            return Convert.ToByte(days);
                        case 11: // December
                            if (days > 31)
                            {
                                days -= 31;
                                break;
                            }
                            return Convert.ToByte(days);
                    }
                    month++;
                }
                return Convert.ToByte(days);
            }
        }
        /// <summary>
        /// 5 bits (0-4) to hours. 6 bits (5-10) to minutes. 6 bits (11-16) to seconds. 10 bits (17-26) to milliseconds
        /// </summary>
        public uint TimeOfDay
        {
            get
            {
                uint timeOfDay = 0;
                timeOfDay |= (uint)Hour << 0;
                timeOfDay |= (uint)Minute << 5;
                timeOfDay |= (uint)Second << 11;
                timeOfDay |= (uint)Millisecond << 17;
                return timeOfDay;
            }
            set
            {
                // Make inline if needs more performance
                int hourMask = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4);
                int minuteMask = (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10);
                int secondMask = (1 << 11) | (1 << 12) | (1 << 13) | (1 << 14) | (1 << 15) | (1 << 16);
                int millisecondMask = ~hourMask & ~minuteMask & ~secondMask & ~((1 << 27) | (1 << 28) | (1 << 29) | (1 << 30) | (1 << 31));

                Hour = Convert.ToByte(value & hourMask);
                Minute = Convert.ToByte((value & minuteMask) >> 5);
                Second = Convert.ToByte((value & secondMask) >> 11);
                Millisecond = Convert.ToInt16((value & millisecondMask) >> 17);
            }
        }
        public byte Hour
        {
            get;
            set;
        }
        public byte Minute
        {
            get;
            set;
        }
        public byte Second
        {
            get;
            set;
        }
        public short Millisecond
        {
            get;
            set;
        }




        public string ToJson()
        {
            return $"{{\"Date\":{Date},\"TimeOfDay\":{TimeOfDay}}}";
        }

        public static DateTime88Bit FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(DateTime88Bit);
            }

            DateTime88Bit dateTime88Bit = new DateTime88Bit();

            json = json.TrimStart('{').TrimEnd('}');
            string[] values = json.Split(',');

            dateTime88Bit.Date = Convert.ToUInt32(values[0].Split(':')[1]);
            dateTime88Bit.TimeOfDay = Convert.ToUInt32(values[1].Split(':')[1]);

            return dateTime88Bit;
        }

        private static DateTime88Bit Carry(DateTime88Bit dateTime88Bit)
        {
            while (dateTime88Bit.Millisecond >= 1000)
            {
                dateTime88Bit.Millisecond -= 1000;
                dateTime88Bit.Second++;
            }
            while (dateTime88Bit.Millisecond < 0)
            {
                dateTime88Bit.Millisecond += 60;
                dateTime88Bit.Second--;
            }

            while (dateTime88Bit.Second >= 60)
            {
                dateTime88Bit.Second -= 60;
                dateTime88Bit.Minute++;
            }
            while (dateTime88Bit.Second > 118) // Wrapped back to 255
            {
                dateTime88Bit.Second += 60;
                dateTime88Bit.Minute--;
            }

            while (dateTime88Bit.Minute >= 60)
            {
                dateTime88Bit.Minute -= 60;
                dateTime88Bit.Hour++;
            }
            while (dateTime88Bit.Second > 118) // Wrapped back to 255
            {
                dateTime88Bit.Minute += 60;
                dateTime88Bit.Hour--;
            }

            while (dateTime88Bit.Hour >= 24)
            {
                dateTime88Bit.Hour -= 24;
                dateTime88Bit.Day++;
            }
            while (dateTime88Bit.Hour > 46) // Wrapped back to 255
            {
                dateTime88Bit.Hour += 24;
                dateTime88Bit.Day--;
            }

            while (dateTime88Bit.Day >= 366)
            {
                dateTime88Bit.Day -= 365;
                dateTime88Bit.Year++;
            }
            while (dateTime88Bit.Day < 0)
            {
                dateTime88Bit.Day += 365;
                dateTime88Bit.Year--;
            }

            return dateTime88Bit;
        }


        public static explicit operator DateTime88Bit(DateTime dateTime)
        {
            DateTime88Bit dateTime448Bit = new DateTime88Bit();

            // Month Day Year
            dateTime448Bit.Day = Convert.ToByte(dateTime.DayOfYear);
            dateTime448Bit.Hour = Convert.ToByte(dateTime.Hour);
            dateTime448Bit.Millisecond = Convert.ToInt16(dateTime.Millisecond);
            dateTime448Bit.Minute = Convert.ToByte(dateTime.Minute);
            dateTime448Bit.Second = Convert.ToByte(dateTime.Second);
            dateTime448Bit.Year = Convert.ToUInt16(dateTime.Year);


            return dateTime448Bit;
        }
        public static explicit operator DateTime(DateTime88Bit dateTime448Bit)
        {
            DateTime dateTime = new DateTime(dateTime448Bit.Year + 1, dateTime448Bit.Month + 1, dateTime448Bit.DayOfMonth, dateTime448Bit.Hour, dateTime448Bit.Minute, dateTime448Bit.Second, Convert.ToInt32(dateTime448Bit.Millisecond));

            return dateTime;
        }
        public static bool operator ==(DateTime88Bit a, DateTime88Bit b)
        {
            return a.Date == b.Date && a.TimeOfDay == b.TimeOfDay;
        }
        public static bool operator !=(DateTime88Bit a, DateTime88Bit b)
        {
            return !(a == b);
        }
        public static DateTime88Bit operator -(DateTime Minuend, DateTime88Bit Subtrahend)
        {
            DateTime88Bit dateTime88BitMinuend = (DateTime88Bit)Minuend;
            dateTime88BitMinuend.Day--;
            dateTime88BitMinuend.Year--;

            dateTime88BitMinuend.Day -= Subtrahend.Day;
            dateTime88BitMinuend.Year -= Subtrahend.Year;
            dateTime88BitMinuend.Hour -= Subtrahend.Hour;
            dateTime88BitMinuend.Minute -= Subtrahend.Minute;
            dateTime88BitMinuend.Second -= Subtrahend.Second;

            dateTime88BitMinuend = Carry(dateTime88BitMinuend);

            return dateTime88BitMinuend;
        }


        public override string ToString()
        {
            string result =
                $"{{\n" +
                $"    \"Day\": {Day},\n" +
                $"    \"Year\": {Year},\n" +
                $"    \"Hour\": {Hour},\n" +
                $"    \"Minute\": {Minute},\n" +
                $"    \"Second\": {Second},\n" +
                $"    \"Millisecond\": {Millisecond},\n" +
                $"}}";
            return result;
        }
        public override int GetHashCode()
        {
            return (int)(TimeOfDay + Date);
        }
        public override bool Equals(object obj)
        {
            if (obj is DateTime88Bit time48Bit)
            {
                return time48Bit == this;
            }
            else if (obj is DateTime time32Bit)
            {
                return (DateTime88Bit)time32Bit == this;
            }
            return false;
        }
    }
}