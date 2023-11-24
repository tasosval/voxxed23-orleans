var builder = WebApplication.CreateBuilder(args);

// Setup the orleans runtime to run for this web application
builder.Host.UseOrleans(silo => silo.UseLocalhostClustering())
    .UseConsoleLifetime();

var app = builder.Build();

// Setup the routes
app.MapGet("/simple", async () =>
{
    await Dal.UpdateRow();
    return await Dal.GetRow();
});

app.MapGet("/orleans", async (IClusterClient client) =>
{
    // Get the reference to grain 0
    var grain = client.GetGrain<IUpdateRowGrain>(0);
    // Send the call to this grain
    return await grain.UpdateAndGetRow();
});

app.MapGet("/setup", () =>
{
    Dal.SetupDb();
    return "ok";
});

app.Run();

public class UpdateRowGrain : Grain, IUpdateRowGrain
{
    public async Task<long> UpdateAndGetRow()
    {
        await Dal.UpdateRow();
        return await Dal.GetRow();
    }
}


public interface IUpdateRowGrain : IGrainWithIntegerKey
{
    Task<long> UpdateAndGetRow();
}