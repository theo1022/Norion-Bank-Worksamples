using System;
using System.Globalization;
using TollFeeCalculator;

public class TollCalculator
{

    /**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     */

    public int GetTollFee(Vehicle vehicle, DateTime[] passes)
    {
        if (IsTollFreeVehicle(vehicle)) return 0;

        var totalFee = 0;
        var maxFee = 60;

        var previousPass = passes[0];

        foreach (var pass in passes)
        {
            if (IsTollFreeDate(pass))
            {
                previousPass = pass;
                continue;
            }

            var passFee = CalculateTollFee(pass);

            if (PreviousPassLessThan60MinutesAgo(pass, previousPass)) passFee = Math.Max(passFee, CalculateTollFee(previousPass));

            totalFee += passFee;

            if (totalFee >= maxFee) break;

            previousPass = pass;
        }
        return Math.Min(totalFee, maxFee);
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        foreach (var tollFreeVehicle in Enum.GetValues(typeof(TollFreeVehicles)))
        {
            if ((int)tollFreeVehicle == (int)vehicle.GetVehicleType()) return true;
        }
        return false;
    }

    public int CalculateTollFee(DateTime currentPass)
    {
        var hour = currentPass.Hour;
        var minute = currentPass.Minute;

        if (hour >= 6 && (hour <= 18 && minute < 30))
        {
            //06:00–06:29     8 kr
            if (hour == 6 || minute < 30) return 8;
            //06:30–06:59     13 kr
            if (hour < 7) return 13;
            //07:00–07:59     18 kr
            if (hour < 8) return 18;
            //08:00–08:29     13 kr
            if (hour == 8 && minute < 30) return 13;
            //08:30–14:59     8 kr
            if (hour < 15) return 8;
            //15:00–15:29     13 kr
            if (hour == 15 && minute < 30) return 13;
            //15:30–16:59     18 kr
            if (hour < 17) return 18;
            //17:00–17:59     13 kr
            if (hour < 18) return 13;
            //18:00–18:29     8 kr
            if (hour == 18 && minute < 30) return 8;
        }
        return 0;
    }

    private Boolean IsTollFreeDate(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;
        int day = date.Day;

        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

        if (year == 2013)
        {
            if (month == 1 && day == 1 ||
                month == 3 && (day == 28 || day == 29) ||
                month == 4 && (day == 1 || day == 30) ||
                month == 5 && (day == 1 || day == 8 || day == 9) ||
                month == 6 && (day == 5 || day == 6 || day == 21) ||
                month == 7 ||
                month == 11 && day == 1 ||
                month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
            {
                return true;
            }
        }
        return false;
    }

    public bool PreviousPassLessThan60MinutesAgo(DateTime currentPass, DateTime lastPassed)
    {
        return (currentPass - lastPassed).TotalMinutes < 60;
    }

    private enum TollFreeVehicles
    {
        Motorbike = 0,
        Tractor = 1,
        Emergency = 2,
        Diplomat = 3,
        Foreign = 4,
        Military = 5
    }
}