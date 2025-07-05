using Incidenten.Domain;

namespace IncidentenApp.Tests;

public class DomainTests
{
    [Fact]
    public void IncidentImageEntity_DefaultImageUrlProvided_WhenIncidentImageClassIsInstantiated()
    {
        var incidentImage = new IncidentImage();
        Assert.Equal("http://localhost:5000/images/NO_IMAGE_PLACEHOLDER.png", incidentImage.ImageUrl);
    }
}