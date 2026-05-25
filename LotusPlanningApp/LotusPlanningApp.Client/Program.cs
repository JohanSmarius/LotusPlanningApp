using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var dutchCulture = new CultureInfo("nl-NL");
dutchCulture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
dutchCulture.DateTimeFormat.LongDatePattern = "d";
dutchCulture.DateTimeFormat.ShortTimePattern = "t";
dutchCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
dutchCulture.DateTimeFormat.MonthDayPattern = "dd-MM";
CultureInfo.DefaultThreadCurrentCulture = dutchCulture;
CultureInfo.DefaultThreadCurrentUICulture = dutchCulture;

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

await builder.Build().RunAsync();
