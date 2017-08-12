namespace Urlshortener.Tests.Controllers
{
    using System.Threading.Tasks;
    using Urlshortener.Functions;
    using Urlshortener.Models;
    using Xunit;

    public class ShortenUrlFunctionsTests
    {
        [Fact]
        public void ShortenGoogleUrlShouldReturn_c7e4()
        {
            var result = ShortenUrlFunctions.ShortenUrl("http://google.com/");

            Assert.Equal("c7e4", result);
        }

        [Fact]
        public void ShortenUrlShouldCallSaveFunctionWhenShortUrlNotFound()
        {
            var originalUrl = "http://google.com/";
            var calledSave = false;

            var result = new ShortUrlResponse(
                (shortUrl) =>
                {
                    calledSave = true;
                    return Task.Run(() => true);
                },
                (url) => url,
                (h) => Task.Run(() => null as string),
                new ShortUrlRequest { OriginalUrl = originalUrl });

            Assert.True(calledSave);
        }

        [Fact]
        public void ShortenUrlShouldNotCallSaveFunctionWhenShortUrlFound()
        {
            var originalUrl = "http://google.com/";
            var calledSave = false;

            var result = new ShortUrlResponse(
                (shortUrl) =>
                {
                    calledSave = true;
                    return Task.Run(() => true);
                },
                (url) => url,
                (h) => Task.Run(() => h),
                new ShortUrlRequest { OriginalUrl = originalUrl });

            Assert.False(calledSave);
        }
    }
}