var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerServices();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddConfigureCors();

builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(_ => _.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

app.UseHttpsRedirection();

app.UseCors(Constants.CorsPolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.ConfigureDatabaseAsync();

await app.RunAsync();
