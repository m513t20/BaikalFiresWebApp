using System.Text.Json;
using Microsoft.VisualBasic;
using SqlManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddMvc();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
var result=new List<FireMetricItem>{
    new FireMetricItem (2021,1,34,"Иркутский район","ewehddihweifhwehf"),
    new FireMetricItem (2021,2,14,"Иркутский район","ewehddihweifhwehf"),
    new FireMetricItem (2021,3,134,"Баяндайский район","ewehddihweifhwewf"),
};
var metircs=new List<FireMetric>(){
    new FireMetric("ewehddihweifhwehf","За период"),
    new FireMetric("ewehddihweifhwewf","За Баяндай")
};

app.UseCors(x => x.AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .SetIsOriginAllowed(origin => true)
                  .WithOrigins("http://localhost:5287")); 



app.MapGet("/metrics",(int metricId)=>
{
    var tst=new SqlService();
    var metr=(tst.GetMetricKeywords(metricId));
    var response=tst.GetMetric(metr);
    var ser=new JsonSerializerOptions{WriteIndented=true};
    var jsres=JsonSerializer.Serialize(response,ser);
    return response;
}).WithName("Metric_choose").WithOpenApi();


app.Run();
record FireMetricItem(int Year, int Month, int Count, string Region,string MetricId);
record FireMetric(string MetricId, string Name);
