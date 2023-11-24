var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(silo => silo.UseLocalhostClustering())
    .UseConsoleLifetime();

var app = builder.Build();

app.MapGet("/simple", async () =>
{
    await Dal.UpdateRow();
    return await Dal.GetRow();
});

app.MapGet("/orleans", async (IClusterClient client) =>
{
    var grain = client.GetGrain<IUpdateRowGrain>(0);
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