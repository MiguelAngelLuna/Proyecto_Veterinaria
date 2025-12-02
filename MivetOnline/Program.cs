using MivetOnline.Data.DAO;
using MivetOnline.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ===== REGISTRAR DAOs CON INYECCIÓN DE DEPENDENCIAS =====
builder.Services.AddScoped<IClienteDAO, ClienteDAO>();
builder.Services.AddScoped<IUsuarioDAO, UsuarioDAO>();
builder.Services.AddScoped<IMascotaDAO, MascotaDAO>();
builder.Services.AddScoped<IPagoDAO, PagoDAO>();
builder.Services.AddScoped<ICitaDAO, CitaDAO>();

// Configurar sesiones (para el login)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar sesiones
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
