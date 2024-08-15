using WorkdayNet;

namespace WorkdayNetTests;

public class WorkdayCalendarTests
{
    [Theory]
    [ClassData(typeof(WorkdayCalendarTestData))]
    public void ShouldReturnCorrectIncrementedDate(DateTime start, decimal increment, DateTime expectedIncrementedDate)
    {
        // Arrange
        IWorkdayCalendar calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        // Act
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        // Assert
        Assert.Equal(incrementedDate.ToString(WorkdayCalendarTestData.Format), expectedIncrementedDate.ToString(WorkdayCalendarTestData.Format));
    }
}