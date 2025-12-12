// xUnit test for AboutBentoViewComponent
// To enable this test, install required packages:
// dotnet add package xunit
// dotnet add package xunit.runner.visualstudio
// dotnet add package Moq

/*
using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MarutiMakwana.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace MarutiMakwana.Tests.ViewComponents;

public class AboutBentoViewComponentTests
{
    [Fact]
    public async Task InvokeAsync_WhenApiFailsOrUnavailable_FallsBackToFixture()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "PUBLIC_API_BASE_URL", "http://localhost:5000" }
            })
            .Build();

        var component = new AboutBentoViewComponent(mockFactory.Object, config);

        // Act
        var result = await component.InvokeAsync();

        // Assert
        Assert.IsType<ViewViewComponentResult>(result);
        var viewResult = result as ViewViewComponentResult;
        Assert.NotNull(viewResult?.ViewData?.Model);
    }
}
*/

namespace MarutiMakwana.Tests.ViewComponents
{
    // Placeholder class to prevent compilation errors
    public class AboutBentoViewComponentTests
    {
    }
}
