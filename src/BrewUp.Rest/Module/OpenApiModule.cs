using System.Globalization;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace BrewUp.Rest.Module;

/// <summary>
/// OpenApiModule is responsible for configuring OpenAPI (Swagger) documentation and related services for the application.
/// </summary>
public class OpenApiModule : IModule
{
  /// <summary>
  /// Indicates whether the module is enabled and should be registered in the application.
  /// </summary>
  public bool IsEnabled => true;
  /// <summary>
  /// Set the order in which the module should be registered in the application.
  /// Modules with lower order values will be registered before those with higher values.
  /// </summary>
  public int Order => 0;
    
  /// <summary>
  /// Registers the module's services and dependencies in the application's service collection.
  /// This method is called during the application startup process to configure the module's services.
  /// </summary>
  /// <param name="builder"></param>
  /// <returns></returns>
  public IServiceCollection Register(WebApplicationBuilder builder)
  {
    builder.Services.AddOpenApi(options =>
    {
      options.AddDocumentTransformer((document, _, _) =>
      {
        document.Servers = [new OpenApiServer { Url = "/" }];
        document.Info = new OpenApiInfo
        {
          Title = "BrewUp API",
          Version = "v1.0",
          Description = "BrewUp API",
          Contact = new OpenApiContact
          {
            Name = "BrewUp Team",
          }
        };

        return Task.CompletedTask;
      });
    });
    
    builder.Services.AddValidation();
    builder.Services.AddProblemDetails(options =>
    {
      options.CustomizeProblemDetails = context =>
      {
        if (context.ProblemDetails is HttpValidationProblemDetails validationProblemDetails)
        {
          context.ProblemDetails.Detail =
            $"Error(s) occurred: {validationProblemDetails.Errors.Values.Sum(x => x.Length)}";
        }

        context.ProblemDetails.Extensions.TryAdd("timestamp",
          DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
      };
    });
    
    return builder.Services;
  }

  /// <summary>
  /// Configures the module's middleware and request pipeline in the application.
  /// This method is called during the application startup process to set up the module's middleware and request handling logic.
  /// </summary>
  /// <param name="app"></param>
  /// <returns></returns>
  public WebApplication Configure(WebApplication app)
  {
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
      options.WithTitle("BrewUp API")
        .WithTheme(ScalarTheme.None);
    });

    return app;
  }
}