using Microsoft.EntityFrameworkCore;
using MiniValidation;
using UpxApi.Data;
using UpxApi.Models;


var builder = WebApplication.CreateBuilder(args);


#region config

var AllowAll = "_allowAll";
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ContextDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAll, builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();
app.UseCors(AllowAll);


if (app.Environment.IsDevelopment())
{
    
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


#endregion

RegionMapActions(app);
SpotMapActions(app);
StudentMapActions(app);

app.Run();

#region regionActions
void RegionMapActions(WebApplication app)
{
    app.MapGet("/region", async
    (ContextDb context) =>
     await context.Regions.ToListAsync())
    .WithName("GetAllRegions")
    .WithTags("Regions");

    app.MapPost("/region", async (
        ContextDb context,
        Region region) =>
    {
        if (!MiniValidator.TryValidate(region, out var errors))
            return Results.ValidationProblem(errors);

        context.Regions.Add(region);

        var result = await context.SaveChangesAsync();

        return result > 0
                ? Results.Created($"/region/{region.RegionId}", region)
                : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces<Spot>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostRegion")
    .WithTags("Regions");

    app.MapPut("/region/{id}", async (
         int id,
         ContextDb context,
         Region region) =>
    {
        if (!MiniValidator.TryValidate(region, out var errors))
            return Results.ValidationProblem(errors);

        var notModifiedRegion = await context.Regions.AsNoTracking<Region>().FirstOrDefaultAsync(f => f.RegionId == id);
        if (notModifiedRegion == null)
            return Results.NotFound();

        context.Regions.Update(region);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutRegion")
    .WithTags("Regions");

    app.MapDelete("/region/{id}", async (
        int id,
        ContextDb context) =>
    {
        var region = await context.Regions.FindAsync(id);
        if (region == null)
            return Results.NotFound();

        context.Regions.Remove(region);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteRegion")
    .WithTags("Regions");
}
#endregion

#region spotActions
void SpotMapActions(WebApplication app)
{
    app.MapGet("/spot", async
    (ContextDb context) =>
     await context.Spots.ToListAsync())
    .WithName("GetAllSpots")
    .WithTags("Spots");

    app.MapGet("/spot/{id}", async
        (int id,
         ContextDb context) =>
        await context.Spots.FindAsync(id)
            is Spot spot
                ? Results.Ok(spot)
                : Results.NotFound())
        .Produces<Spot>(StatusCodes.Status200OK)
        .Produces<Spot>(StatusCodes.Status404NotFound)
        .WithName("GetSpotById")
        .WithTags("Spots");

    app.MapPost("/spot", async
        (ContextDb context,
         Spot spot) =>
    {
        if (!MiniValidator.TryValidate(spot, out var errors))
            return Results.ValidationProblem(errors);

        context.Spots.Add(spot);

        var result = await context.SaveChangesAsync();

        return result > 0
                ? Results.Created($"/spot/{spot.SpotId}", spot)
                : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces<Spot>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostSpot")
    .WithTags("Spots");

    app.MapPut("/spot/{id}", async (
         int id,
         ContextDb context,
         Spot spot) =>
    {
        if (!MiniValidator.TryValidate(spot, out var errors))
            return Results.ValidationProblem(errors);

        var notModifiedSpot = await context.Spots.AsNoTracking<Spot>().FirstOrDefaultAsync(f => f.SpotId == id);
        if (notModifiedSpot == null)
            return Results.NotFound();

        context.Spots.Update(spot);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutSpot")
    .WithTags("Spots");

    app.MapDelete("/spot/{id}", async (
        int id,
        ContextDb context) =>
    {
        var spot = await context.Spots.FindAsync(id);
        if (spot == null)
            return Results.NotFound();

        context.Spots.Remove(spot);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteSpot")
    .WithTags("Spots");



    app.MapGet("/spotregion/{region}", async (
        string region, ContextDb context) =>
        await context.Spots.Where(p => p.Region == region).ToListAsync()
            is List<Spot> spot
                ? (spot.Count > 0
                    ? Results.Ok(spot)
                    : Results.NotFound())
                : Results.NotFound())
    .Produces<Spot>(StatusCodes.Status200OK)
    .Produces<Spot>(StatusCodes.Status404NotFound)
    .WithName("GetSpotByRegion")
    .WithTags("SpotRegion");
}
#endregion

#region studentActions
void StudentMapActions(WebApplication app)
{
    app.MapGet("/student", async
      (ContextDb context) =>
       await context.Students.ToListAsync())
      .WithName("GetAllStudents")
      .WithTags("Students");

    app.MapPost("/student", async (ContextDb context,
         Student student) =>
    {
        if (!MiniValidator.TryValidate(student, out var errors))
            return Results.ValidationProblem(errors);

        context.Students.Add(student);

        var result = await context.SaveChangesAsync();

        return result > 0
                ? Results.Created($"/student/{student.Id}", student)
                : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces<Spot>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostStudent")
    .WithTags("Students");

    app.MapGet("/student/{id}", async (
        int id,
        ContextDb context) =>
        await context.Students.FindAsync(id)
            is Student student
                ? Results.Ok(student)
                : Results.NotFound())
    .Produces<Spot>(StatusCodes.Status200OK)
    .Produces<Spot>(StatusCodes.Status404NotFound)
    .WithName("GetStudentById")
    .WithTags("Students");

    app.MapGet("/student/{id}/spot", async (
        int id,
        ContextDb context) =>
        await context.Students.FindAsync(id)
            is Student student
                ? Results.Ok(context.Spots.FindAsync(student.SpotId))
                : Results.NotFound())
    .Produces<Spot>(StatusCodes.Status200OK)
    .Produces<Spot>(StatusCodes.Status404NotFound)
    .WithName("GetStudentSpotById")
    .WithTags("Students");

    app.MapPut("/student/{id}", async (
         int id,
         ContextDb context,
         Student student) =>
    {
        if (!MiniValidator.TryValidate(student, out var errors))
            return Results.ValidationProblem(errors);

        var notModifiedStudent = await context.Students.AsNoTracking<Student>().FirstOrDefaultAsync(f => f.Id == id);
        if (notModifiedStudent == null)
            return Results.NotFound();

        if (notModifiedStudent.SpotId != student.SpotId)
        {
            var oldSpot = await context.Spots.FindAsync(notModifiedStudent.SpotId);
            if (oldSpot != null)
            {
                oldSpot.Occupied = false;
                context.Spots.Update(oldSpot);
            }

            var newSpot = await context.Spots.FindAsync(student.SpotId);
            if (newSpot != null)
            {
                newSpot.Occupied = true;
                context.Spots.Update(newSpot);
            }
        }

        context.Students.Update(student);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutStudent")
    .WithTags("Students");

    app.MapDelete("/student/{id}", async (
        int id,
        ContextDb context) =>
    {
        var student = await context.Students.FindAsync(id);
        if (student == null)
            return Results.NotFound();

        var spot = await context.Spots.FindAsync(student.SpotId);
        if (spot != null)
        {
            spot.Occupied = false;
            context.Spots.Update(spot);
        }

        context.Students.Remove(student);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was a problem saving the record");
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteStudent")
    .WithTags("Students");
}
#endregion