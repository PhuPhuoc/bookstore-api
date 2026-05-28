var builder = WebApplication.CreateBuilder(args);
{
  builder.Services
    .AddSwaggerGen()
    .AddControllers();
}
var app = builder.Build();
{
  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger().UseSwaggerUI();
  }

  app.UseHttpsRedirection();
  app.MapControllers();

  app.MapGet("/", () => Results.Redirect("/swagger"));

  app.Run();
}
