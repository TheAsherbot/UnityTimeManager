using UnityEngine;

[ExecuteInEditMode]
public class YearCalculator : MonoBehaviour
{

    [SerializeField] private int days;

    [Header("^^^ days is: ")]
    [SerializeField] private int years;
    [SerializeField] private int months;
    [SerializeField] private int daysOfMonth;


    private void Update()
    {
        ConvertDaysToYear_Months_DayOfMonth(days, out years, out months, out daysOfMonth);
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

        numberOfDaysLeftInYears += isCurrentYearLeapYear ? 1 : 0;
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
