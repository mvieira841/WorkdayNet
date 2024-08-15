namespace WorkdayNet
{
    public class WorkdayCalendar : IWorkdayCalendar
    {
        private readonly HashSet<DateTime> _holidays = new HashSet<DateTime>();
        private readonly HashSet<(int Month, int Day)> _recurringholidays = new HashSet<(int, int)>();
        private TimeSpan _workdayStart;
        private TimeSpan _workdayEnd;
        private double _totalMinutesInWorkday;

        public void SetHoliday(DateTime date)
        {
            _holidays.Add(date.Date);
        }

        public void SetRecurringHoliday(int month, int day)
        {
            _recurringholidays.Add((month, day));
        }

        public void SetWorkdayStartAndStop(int startHours, int startMinutes, int stopHours, int stopMinutes)
        {
            _workdayStart = new TimeSpan(startHours, startMinutes, 0);
            _workdayEnd = new TimeSpan(stopHours, stopMinutes, 0);
            _totalMinutesInWorkday = (double)(_workdayEnd - _workdayStart).TotalMinutes;
        }

        public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
        {
            DateTime currentDate = AdjustToWorkday(startDate);
            double remainingWorkdays = Convert.ToDouble(incrementInWorkdays);

            while (remainingWorkdays != 0)
            {
                if (remainingWorkdays > 0)
                {
                    double workdayFraction = GetRemainingWorkdayFractionForAddition(currentDate);
                    if (workdayFraction >= remainingWorkdays)
                    {
                        currentDate = AddWorkdayFraction(currentDate, remainingWorkdays);
                        remainingWorkdays = 0;
                    }
                    else
                    {
                        remainingWorkdays -= workdayFraction;
                        currentDate = MoveToNextWorkday(currentDate.Date.AddDays(1));
                    }
                }
                else
                {
                    double workdayFraction = GetRemainingWorkdayFractionForSubstraction(currentDate);
                    if (workdayFraction >= -remainingWorkdays)
                    {
                        currentDate = SubtractWorkdayFraction(currentDate, -remainingWorkdays);
                        remainingWorkdays = 0;
                    }
                    else
                    {
                        remainingWorkdays += workdayFraction;
                        currentDate = MoveToPreviousWorkday(currentDate.Date.AddDays(-1));
                    }
                }
            }

            return currentDate;
        }

        private DateTime AdjustToWorkday(DateTime dateTime)
        {
            while (IsWeekendOrHoliday(dateTime))
            {
                dateTime = dateTime.AddDays(1);
            }

            TimeSpan time = dateTime.TimeOfDay;
            if (time < _workdayStart)
            {
                return dateTime.Date + _workdayStart;
            }
            if (time > _workdayEnd)
            {
                return dateTime.Date + _workdayEnd;
            }

            return dateTime;
        }

        private bool IsWeekendOrHoliday(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ||
                                                          _holidays.Contains(date.Date) || _recurringholidays.Contains((date.Month, date.Day));

        private DateTime MoveToNextWorkday(DateTime date)
        {
            while (IsWeekendOrHoliday(date))
            {
                date = date.AddDays(1);
            }

            return date.Date + _workdayStart;
        }

        private DateTime MoveToPreviousWorkday(DateTime date)
        {
            while (IsWeekendOrHoliday(date))
            {
                date = date.AddDays(-1);
            }

            return date.Date + _workdayEnd;
        }

        private double GetRemainingWorkdayFractionForAddition(DateTime date)
        {
            TimeSpan timeOfDay = date.TimeOfDay;
            if (timeOfDay < _workdayStart)
            {
                return 1;
            }

            if (timeOfDay >= _workdayEnd)
            {
                return 0;
            }

            double minutesRemaining = (double)(_workdayEnd - timeOfDay).TotalMinutes;
            return minutesRemaining / _totalMinutesInWorkday;
        }

        private double GetRemainingWorkdayFractionForSubstraction(DateTime date)
        {
            TimeSpan timeOfDay = date.TimeOfDay;
            if (timeOfDay < _workdayStart)
            {
                return 0;
            }
            if (timeOfDay >= _workdayEnd)
            {
                return 1;
            }
            double minutesRemaining = (double)(timeOfDay - _workdayStart).TotalMinutes;
            return minutesRemaining / _totalMinutesInWorkday;
        }

        private DateTime AddWorkdayFraction(DateTime date, double fractionOfWorkday)
        {
            double minutesToAdd = GetWorkdayFractionRoundOff(date, fractionOfWorkday);
            return date.AddMinutes((double)minutesToAdd);
        }

        private DateTime SubtractWorkdayFraction(DateTime date, double fractionOfWorkday)
        {
            double minutesToSubtract = GetWorkdayFractionRoundOff(date, fractionOfWorkday);
            return date.AddMinutes(-(double)minutesToSubtract);
        }

        private double GetWorkdayFractionRoundOff(DateTime date, double fractionOfWorkday)
        {
            double minutes = fractionOfWorkday * _totalMinutesInWorkday;
            double hours = Math.Floor(minutes / 60);
            double hourMinutes = hours * 60;
            minutes = Math.Floor(minutes - hours * 60);
            return hourMinutes + minutes;
        }
    }
}