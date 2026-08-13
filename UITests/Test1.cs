namespace UITests
{
    [TestClass]
    public class Test1 : PageTest
    {
        // Fallback matches the repository's current local UI test host used in CI/task runs.
        private const string DefaultBaseUrl = "http://localhost:5087";

        /// <summary>
        /// Gets the base URL for UI tests from the UITEST_BASE_URL environment variable,
        /// defaulting to the local development server.
        /// </summary>
        private static string BaseUrl => Environment.GetEnvironmentVariable("UITEST_BASE_URL") ?? DefaultBaseUrl;

        [TestMethod]
        public async Task HomepageHasTitleAndButtonToManageEvents()
        {
            await Page.GotoAsync(BaseUrl);

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("LOTUS Planning App"));

            // create a locator
            var manageEvents = Page.Locator("a[href='/events']").First;

            // Expect an attribute "to be strictly equal" to the value.
            await Expect(manageEvents).ToHaveAttributeAsync("href", "/events");

            // Click the manage events link.
            await manageEvents.ClickAsync();

            // Expects the URL to contain events.
            await Expect(Page).ToHaveURLAsync(new Regex(".*events"));
        }

        [TestMethod]
        public async Task HomepageHasTitleAndButtonToViewDashboard()
        {
            await Page.GotoAsync(BaseUrl);

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("LOTUS Planning App"));

            // create a locator for the dashboard button
            var viewDashboard = Page.Locator("a[href='/dashboard']").First;

            // Expect an attribute "to be strictly equal" to the value.
            await Expect(viewDashboard).ToHaveAttributeAsync("href", "/dashboard");

            // Click the dashboard link.
            await viewDashboard.ClickAsync();

            // Expects the URL to contain dashboard.
            await Expect(Page).ToHaveURLAsync(new Regex(".*dashboard"));
        }
    }
}
