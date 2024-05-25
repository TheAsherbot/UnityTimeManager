using System;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

using UnityEditor;

using UnityEngine;
using System.IO;
using UnityEditor.SceneManagement;
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
            time48Bit.minute = Convert.ToSByte((minutesMask & time32Bit.time) << 6);
            time48Bit.hours = Convert.ToSByte((hoursMask & time32Bit.time) << 12);
            time48Bit.days = Convert.ToUInt16((daysMask & time32Bit.time) << 17);
            time48Bit.years = Convert.ToByte((yearsMask & time32Bit.time) << 26);

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
            if (years != 0)
            {
                resalt += years + " years  ";
            }
            if (days != 0)
            {
                resalt += days + " days  ";
            }
            if (hours != 0)
            {
                resalt += hours + " hours  ";
            }
            if (minute != 0)
            {
                resalt += minute + " minute  ";
            }
            if (seconds != 0)
            {
                resalt += seconds + " seconds  ";
            }
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
            time32Bit.time |= time48Bit.seconds >> 0;
            time32Bit.time |= time48Bit.minute >> 6;
            time32Bit.time |= time48Bit.hours >> 12;
            time32Bit.time |= time48Bit.days >> 17;
            time32Bit.time |= time48Bit.years >> 26;
            
            return time32Bit;
        }
    }

    [Serializable]
    private struct SaveData
    {
        public DateTime startTime;
        public Time32Bit todaysElapsedTime;
        public Time32Bit totalElapsedTime;
        public DateTime lastUsedTime;
        public bool isCurrentlyOpen;
    }



    static TimeTracker()
    {
        Debug.Log("TimeTracker");
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        
        AssemblyReloadEvents.beforeAssemblyReload += () =>
        {
            Debug.Log("beforeAssemblyReload");
            // cancellationTokenSource.Cancel();
        };
        AssemblyReloadEvents.afterAssemblyReload += () =>
        {
            Debug.Log("afterAssemblyReload");
        };

        Task task = Task.Run(async () =>
        {
            SaveData saveData = await Load();
            // if (!saveData.isCurrentlyOpen)
            {
                TimeTracker timeTracker = new TimeTracker();

                cancellationToken.ThrowIfCancellationRequested();

                await timeTracker.Start();

                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Debug.Log("He");
                    cancellationToken.ThrowIfCancellationRequested();
                    timeTracker.Update();
                    await Task.Delay(100);
                }
            }
            return;
        });
    }



    // Import Functions  following.
    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    public static extern bool SetWindowText(IntPtr hwnd, string lpString);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();


    private static readonly string TIME_SPENT_FILE_PATH = Application.dataPath + '/';
    private static readonly string TIME_SPENT_FILE_NAME = "TimeSpent.txt";
    private string BaseTitleName
    {
        get
        {
            return Application.productName + " - " + EditorSceneManager.GetActiveScene().name + " - Windows, Mac, Linux - Unity " + Application.unityVersion + EditorSceneManager.GetActiveScene().isDirty + " <DX11>";
        }
    }
    private IntPtr unityApplication;




    private DateTime startSessionDateTime;
    private DateTime lastDatetime;
    private DateTime currentDatetime;

    private Time48Bit elapsedTime;
    private Time48Bit elapsedSessionTime;
    private Time48Bit todaysElapsedTime;
    private Time48Bit totalElapsedTime;


    private byte updateAmount;


    ~TimeTracker()
    {
        Save(false);

        EditorApplication.quitting -= OnEditorClose;
    }


    public async Task Start()
    {
        EditorApplication.quitting += OnEditorClose;

        unityApplication = GetForegroundWindow();

        Debug.Log("TEST!"); 

        startSessionDateTime = DateTime.Now;
        lastDatetime = DateTime.Now;

        elapsedTime = new Time48Bit();
        elapsedSessionTime = new Time48Bit();
        todaysElapsedTime = new Time48Bit();
        totalElapsedTime = new Time48Bit();

        SaveData saveData = new SaveData(); 
        // Try getting data
        if (Directory.Exists(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME))
        {
            string fileData = await File.ReadAllTextAsync(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME);
            saveData = JsonUtility.FromJson<SaveData>(fileData);
            if (DateTime.Now.Date == saveData.lastUsedTime.Date)
            {
                todaysElapsedTime = (Time48Bit)saveData.todaysElapsedTime;
            }
            totalElapsedTime = (Time48Bit)saveData.totalElapsedTime;
        }
        else
        {
            saveData.startTime = startSessionDateTime;

            File.WriteAllText(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME, JsonUtility.ToJson(saveData));
        }

        Save(false);
    }

    private void OnEditorClose()
    {
        Save(true);
    }

    public void Update()
    {
        currentDatetime = DateTime.Now;

        elapsedTime = Time48Bit.GetDifferenceFrom2DateTimes(currentDatetime, lastDatetime);
        elapsedSessionTime += elapsedTime;
        todaysElapsedTime += elapsedTime;
        totalElapsedTime += elapsedTime;

        lastDatetime = currentDatetime;


        updateAmount++;
        if (updateAmount == byte.MaxValue)
        {
            updateAmount = 0;
            Save(false);
        }

        UpdateTitle(elapsedSessionTime.ToString(), todaysElapsedTime.ToString(), totalElapsedTime.ToString());
    }

    private void UpdateTitle(string elapsedSessionTime, string todaysElapsedTime, string totalElapsedTime)
    {
        SetWindowText(unityApplication, /*BaseTitleName +*/ " Elapsed Session Time: " + elapsedSessionTime + " Todays Elapsed Time: " + todaysElapsedTime + " Total Elapsed Time: " + totalElapsedTime); 
    }

    private void Save(bool isClosing)
    {
        SaveData saveData = new SaveData();
        saveData.startTime = startSessionDateTime;
        saveData.totalElapsedTime = (Time32Bit)totalElapsedTime;
        saveData.todaysElapsedTime = (Time32Bit)totalElapsedTime;
        saveData.lastUsedTime = currentDatetime;
        saveData.isCurrentlyOpen = !isClosing;

        File.WriteAllText(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME, JsonUtility.ToJson(saveData));
    }

    private async static Task<SaveData> Load()
    {
        string fileData = await File.ReadAllTextAsync(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME);
        return JsonUtility.FromJson<SaveData>(fileData);
    }

    private static void ConvertDaysToYear_Months_DayOfMonth(int days, out int year, out int month, out int dayOfTheMonth)
    {
        year = 0;
        bool canAddMoreYears = true;
        bool isCurrentYearLeapYear = false;

        int numberOfDaysLeftInYears = days;
        int i = 0;
        while (canAddMoreYears)
        {
            if (numberOfDaysLeftInYears > 365)
            {
                isCurrentYearLeapYear = false;
                numberOfDaysLeftInYears -= 365;
                i++;
                year++;
                if (i % 4 == 0 && i % 400 != 0) // Check for leap year
                {
                    numberOfDaysLeftInYears--;
                    isCurrentYearLeapYear = true;
                }// 
            }
            else
            {
                canAddMoreYears = false;
            }
        }

        numberOfDaysLeftInYears += isCurrentYearLeapYear ? 1: 0;
        month = 0;
        for (i = 0; i < 12; i++)
        {
            month = i;
            switch (i)
            {
                case 0: // January
                    if (numberOfDaysLeftInYears > 31)
                    {
                        numberOfDaysLeftInYears -= 31;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 1: // February
                    if (numberOfDaysLeftInYears > (isCurrentYearLeapYear ? 29 : 28))
                    {
                        numberOfDaysLeftInYears -= (isCurrentYearLeapYear ? 29 : 28);
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 2: // March
                    if (numberOfDaysLeftInYears > 31)
                    {
                        numberOfDaysLeftInYears -= 31;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 3: // April
                    if (numberOfDaysLeftInYears > 30)
                    {
                        numberOfDaysLeftInYears -= 30;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 4: // May
                    if (numberOfDaysLeftInYears > 31)
                    {
                        numberOfDaysLeftInYears -= 31;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 5: // June
                    if (numberOfDaysLeftInYears > 30)
                    {
                        numberOfDaysLeftInYears -= 30;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 6: // July
                    if (numberOfDaysLeftInYears > 31)
                    {
                        numberOfDaysLeftInYears -= 31;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 7: // August
                    if (numberOfDaysLeftInYears > 31)
                    {
                        numberOfDaysLeftInYears -= 31;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 8: // September
                    if (numberOfDaysLeftInYears > 30)
                    {
                        numberOfDaysLeftInYears -= 30;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 9: // October
                    if (numberOfDaysLeftInYears > 31)
                    {
                        numberOfDaysLeftInYears -= 31;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 10: // November
                    if (numberOfDaysLeftInYears > 30)
                    {
                        numberOfDaysLeftInYears -= 30;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
                case 11: // December
                    if (numberOfDaysLeftInYears > 31)
                    {
                        numberOfDaysLeftInYears -= 31;
                        continue;
                    }
                    dayOfTheMonth = numberOfDaysLeftInYears;
                    return;
            }
        }
        dayOfTheMonth = -1;
    }

}