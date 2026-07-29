namespace UITests
{
    [TestClass]
    public class Test1 : PageTest
    {
        [TestMethod]
        public async Task HomepageHasTitleAndButtonToManageEvents()
        {
            await Page.GotoAsync("http://localhost:5087");

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("LOTUS Planning App"));

            // create a locator
            var manageEvents = Page.Locator("text=Opdrachten beheren");

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
            await Page.GotoAsync("http://localhost:5087");

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("LOTUS Planning App"));

            // create a locator for the dashboard button
            var viewDashboard = Page.Locator("text=Bekijk dashboard");

            // Expect an attribute "to be strictly equal" to the value.
            await Expect(viewDashboard).ToHaveAttributeAsync("href", "/dashboard");

            // Click the dashboard link.
            await viewDashboard.ClickAsync();

            // Expects the URL to contain dashboard.
            await Expect(Page).ToHaveURLAsync(new Regex(".*dashboard"));
        }
    }
}
