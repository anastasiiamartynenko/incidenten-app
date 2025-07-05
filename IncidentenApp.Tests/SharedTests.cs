using Incidenten.Shared.Utils;

namespace IncidentenApp.Tests;

public class SharedTests
{
    [Fact]
    public void IsValidEmail_ReturnsTrue_WhenEmailIsValid()
    {
        var validationHelper = new ValidationHelper();
        Assert.True(validationHelper.IsValidEmail("test@test.com"));
    }

    [Fact]
    public void IsValidEmail_ReturnsFalse_WhenEmailIsNotValid()
    {
        var validationHelper = new ValidationHelper();
        Assert.False(validationHelper.IsValidEmail("test"));
    }
}
