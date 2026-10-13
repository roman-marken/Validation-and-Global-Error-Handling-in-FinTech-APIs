using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NeoBankApi.Data;
using NeoBankApi.Exceptions;
using NeoBankApi.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<NeoBankDbContext>(options =>
{
    options.UseInMemoryDatabase("NeoBankDb");
    options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountDtoValidator>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();

app.Run();
