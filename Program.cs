using DoughTempPredictor.RegressionCalculator;
using System.Text.Json;
using project.ApiCommunication;
using DoughTempPredictor;

namespace DoughTempPredictor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReact", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseCors("AllowReact");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapPost("/send", (TemperatureRequest data) =>
            {
                try
                {
                    double[] inputs = new double[] {  data.oil, data.water, data.flour, data.x4 };
                    PredictionEnhancements.asdf(inputs);
                    //(double IceAmount, double? OilAmount) = PredictionEnhancements.CheckAndSendFeature(inputs, 120);

                    //var response = new PredictionResponse
                    //{
                    //    IceAmount = IceAmount,
                    //    OilAmount = OilAmount,
                    //    ErrorMessage = ""
                    //};
                    return Results.NoContent();
                    //return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    return Results.Ok(new PredictionResponse
                    {
                        ErrorMessage = ex.Message
                    });
                }
            });

            app.MapPost("/manual-predict", async (HttpContext context) =>
            {
                try
                {
                    Console.WriteLine("📦 התחיל חיזוי חודשי לפי נתוני בצק");

                    LinearRegression.PerformGradientDescent();

                    predict.PredictAndPrintFromCsv();

                    // מחזירים תגובה ריקה עם סטטוס 204 (No Content)
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    Console.WriteLine( ex.Message);
                    return Results.StatusCode(500); // Internal Server Error
                }
            });


            app.Run();
        }
    }
}
