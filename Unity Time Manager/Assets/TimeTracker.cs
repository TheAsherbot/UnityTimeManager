using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Unity.EditorCoroutines.Editor;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

using Debug = UnityEngine.Debug;
//  
[InitializeOnLoad]
public class TimeTracker
{
    [Serializable]
    /// <summary>
    /// This uses 48 bits of data to store the time. It give 0 bits to Milliseconds (AKA does not track milliseconds) 1 byte to seconds, 1 byte to minutes, 1 byte to hours, 2 bytes to days, 1 byte to years.
    /// </summary>
    private struct Time48Bit
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
            time48Bit.years = Convert.ToByte(minuend.Year- subtrahend.Year);


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
                resalt += "" + years + ":" + days + ":" + hours + ":" + minute + ":" + seconds;
                units += "Y:D:H:M:S";
            }
            else if (days != 0)
            {
                resalt += "" + days + ":" + hours + ":" + minute + ":" + seconds;
                units += "D:H:M:S";
            }
            else if(hours != 0)
            {
                resalt += "" + hours + ":" + minute + ":" + seconds;
                units += "H:M:S";
            }
            else if(minute != 0)
            {
                resalt += "" + minute + ":" + seconds;
                units += "M:S";
            }
            else if(seconds != 0)
            {
                resalt += "" + seconds;
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
    private struct Time32Bit
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

    private struct DateTime88Bit
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
                date |= (uint)(Year - 1) << 9;     // 16 bits. day is - 1. I want 0-65535 (max number of Unsinged short)
                return date;
            }
            set
            {
                // Make inline if needs more performance
                int daysMask = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8);
                int yearMask = ~daysMask & ~((1 << 25) | (1 << 26) | (1 << 27) | (1 << 28) | (1 << 29) | (1 << 30) | (1 << 31));

                Day = Convert.ToByte(value & daysMask);
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
            DateTime dateTime = new DateTime(dateTime448Bit.Year + 1, dateTime448Bit.Month + 1, dateTime448Bit.DayOfMonth, dateTime448Bit.Hour, dateTime448Bit.Minute, dateTime448Bit.Second, 0 + Convert.ToInt32(dateTime448Bit.Millisecond));


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


    [Serializable]
    private struct SaveData
    {
        public string startTime;
        public Time32Bit elapsedSessionTime;
        public Time32Bit todaysElapsedTime;
        public Time32Bit totalElapsedTime;
        public string lastUsedTime;
        public bool isFirstOpen;
    }



    static TimeTracker()
    {
        TimeTracker timeTracker = new TimeTracker();
    }


#if PLATFORM_STANDALONE_WIN
    private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumWindows(EnumWindowProc lpEnumFunc, IntPtr lParam);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    // Import Functions  following.
    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    public static extern bool SetWindowText(IntPtr hwnd, string lpString);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
#endif



    private static readonly string TIME_SPENT_FILE_PATH = Application.dataPath + '/';
    private static readonly string TIME_SPENT_FILE_NAME = "TimeSpent.txt";
    private string BaseTitleName
    {
        get
        {
            return Application.productName + " - " + EditorSceneManager.GetActiveScene().name + " - Windows, Mac, Linux - Unity " + Application.unityVersion + (EditorSceneManager.GetActiveScene().isDirty ? '*' : "") + " <DX11>";
        }
    }
    private IntPtr unityApplicationHandle;




    private DateTime startSessionDateTime;
    private DateTime lastDatetime;
    private DateTime currentDatetime;

    private Time48Bit elapsedTime;
    private Time48Bit elapsedSessionTime;
    private Time48Bit todaysElapsedTime;
    private Time48Bit totalElapsedTime;

    private SaveData saveData;

    private TimeTracker()
    {
        AssemblyReloadEvents.beforeAssemblyReload += () => Save();
        EditorApplication.wantsToQuit += OnEditorClose;

        unityApplicationHandle = GetUnityWindowHandle();
        


        startSessionDateTime = DateTime.Now;
        lastDatetime = DateTime.Now;

        elapsedTime = new Time48Bit();
        elapsedSessionTime = new Time48Bit();
        todaysElapsedTime = new Time48Bit();
        totalElapsedTime = new Time48Bit();

        saveData = new SaveData();
        // Try getting data
        if (File.Exists(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME))
        {
            saveData = Load();
            DateTime88Bit lastUsedTime = DateTime88Bit.FromJson(saveData.lastUsedTime);
            totalElapsedTime = (Time48Bit)saveData.totalElapsedTime;
            if (DateTime.Now.Date == ((DateTime)lastUsedTime).Date)
            {
                todaysElapsedTime = (Time48Bit)saveData.todaysElapsedTime;
                DateTime88Bit differenceBetweenLastSessionAndThisOne = startSessionDateTime - lastUsedTime;
                if (!saveData.isFirstOpen) // As long as there is only Addend difference of less then 1 minute, and 59 seconds seconds
                {
                    elapsedSessionTime = (Time48Bit)saveData.elapsedSessionTime;
                    elapsedSessionTime.seconds += Convert.ToSByte(differenceBetweenLastSessionAndThisOne.Second);
                    todaysElapsedTime.seconds += Convert.ToSByte(differenceBetweenLastSessionAndThisOne.Second);
                    totalElapsedTime.seconds += Convert.ToSByte(differenceBetweenLastSessionAndThisOne.Second);
                }
            }
        }
        else
        {
            saveData.startTime = ((DateTime88Bit)startSessionDateTime).ToJson();
            saveData.lastUsedTime = ((DateTime88Bit)startSessionDateTime).ToJson();
        }


        Save();

        EditorCoroutineUtility.StartCoroutineOwnerless(UpdateLoop());
    }

    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            Update();
            yield return new EditorWaitForSeconds(1);
        }
    }

    private bool OnEditorClose()
    {
        // EditorCoroutineUtility.StopCoroutine(updateLoopCoroutine);
        
        Save(true);

        return true;
    }

    private void Update()
    {
        currentDatetime = DateTime.Now;

        elapsedTime = Time48Bit.GetDifferenceFrom2DateTimes(currentDatetime, lastDatetime);
        elapsedSessionTime += elapsedTime;
        todaysElapsedTime += elapsedTime;
        totalElapsedTime += elapsedTime;

        lastDatetime = currentDatetime;

        Save();

        UpdateTitle(elapsedSessionTime.ToString(), todaysElapsedTime.ToString(), totalElapsedTime.ToString());
    }

    private void UpdateTitle(string elapsedSessionTime, string todaysElapsedTime, string totalElapsedTime)
    {
        // TO DO!!! Unity overrides what I have when the scene is switch from is dirty to is not dirty and the other way around.

        SetWindowText(unityApplicationHandle, BaseTitleName + " Elapsed Session Time: " + elapsedSessionTime + " Todays Elapsed Time: " + todaysElapsedTime + " Total Elapsed Time: " + totalElapsedTime); 
    }

    private void Save(bool isFirstOpen = false)
    {
        saveData.startTime = ((DateTime88Bit)startSessionDateTime).ToJson();
        saveData.elapsedSessionTime = (Time32Bit)elapsedSessionTime;
        saveData.todaysElapsedTime = (Time32Bit)todaysElapsedTime;
        saveData.totalElapsedTime = (Time32Bit)totalElapsedTime;
        saveData.lastUsedTime = ((DateTime88Bit)currentDatetime).ToJson();
        saveData.isFirstOpen = isFirstOpen;

        File.WriteAllText(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME, JsonUtility.ToJson(saveData));
    }

    private static SaveData Load()
    {
        string fileData = File.ReadAllText(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME);
        return JsonUtility.FromJson<SaveData>(fileData);
    }
#if PLATFORM_STANDALONE_WIN
    private static IntPtr GetUnityWindowHandle()
    {
        // TO DO: Bug at startup. The window has not opened so can not rename it...
        Process process = Process.GetCurrentProcess();


        IEnumerable <IntPtr> windowHandles = GetAllWindowHandlesForProcess(process.Id); 

        // Print each handle.
        foreach (IntPtr handle in windowHandles)
        {
            StringBuilder buffer = new StringBuilder(256);
            GetWindowText(handle, buffer, buffer.Capacity);
            if (buffer.ToString().Contains("Unity"))
            {
                return handle;
            }
        }

        return IntPtr.Zero;
    }

    public static IEnumerable<IntPtr> GetAllWindowHandlesForProcess(int processId)
    {
        List<IntPtr> handles = new List<IntPtr>();

        EnumWindows((hWnd, lParam) =>
        {
            GetWindowThreadProcessId(hWnd, out uint pid);

            if (pid == (uint)processId)
            {
                handles.Add(hWnd);
            }

            return true;
        }, IntPtr.Zero);

        return handles;
    }
#endif
    
}
