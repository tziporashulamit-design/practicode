using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using TodoApi; 

var builder = WebApplication.CreateBuilder(args);

// --- 1. הגדרת שירותים (Services) ---

builder.Services.AddCors(options => options.AddPolicy("AllowAll", 
    p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddDbContext<ToDoDbContext>();

// מפתח סודי - ודאי שהוא תואם למה שמוגדר אצלך
var jwtKey = "YourSecretSuperLongKeyHere12345678"; 
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 2. Middleware ---

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication(); 
app.UseAuthorization();

// --- 3. נתיבי API (Routes) ---

// הרשמה
app.MapPost("/register", async (User user, ToDoDbContext db) => {
    try {
        Console.WriteLine($"Register attempt: Username={user.Username}, Password={user.Password}");
        
        // בדיקה אם המשתמש כבר קיים
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (existingUser != null)
        {
            return Results.BadRequest(new { message = "Username already exists" });
        }
        
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "User registered successfully" });
    } catch (Exception ex) {
        Console.WriteLine($"Register error: {ex.Message}");
        return Results.Problem(ex.Message);
    }
});

// התחברות
app.MapPost("/login", async (User loginData, ToDoDbContext db) => {
    var user = await db.Users.FirstOrDefaultAsync(u => 
        u.Username == loginData.Username && u.Password == loginData.Password);
        
    if (user is null) return Results.Unauthorized();

    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor {
        Subject = new ClaimsIdentity(new[] { 
            new Claim("id", user.Id.ToString()), 
            new Claim(ClaimTypes.Name, user.Username) 
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Results.Ok(new { token = tokenHandler.WriteToken(token) });
});

// שליפת משימות
app.MapGet("/items", async (ToDoDbContext db, ClaimsPrincipal user) => {
    var userIdClaim = user.FindFirst("id")?.Value;
    if (string.IsNullOrEmpty(userIdClaim)) return Results.Unauthorized();
    
    int userId = int.Parse(userIdClaim);
    return Results.Ok(await db.Items.Where(i => i.UserId == userId).ToListAsync());
}).RequireAuthorization();

// הוספת משימה
app.MapPost("/items", async (TodoApi.Item item, ToDoDbContext db, ClaimsPrincipal user) => {
    var userIdClaim = user.FindFirst("id")?.Value;
    if (string.IsNullOrEmpty(userIdClaim)) return Results.Unauthorized();
    
    item.UserId = int.Parse(userIdClaim); 

    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
}).RequireAuthorization();

// עדכון משימה - תיקנתי כאן ל-TodoApi.Item
app.MapPut("/items/{id}", async (int id, TodoApi.Item inputItem, ToDoDbContext db) => {
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    
    item.IsComplete = inputItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

// מחיקת משימה
app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) => {
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    
    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok(item);
}).RequireAuthorization();

app.Run();