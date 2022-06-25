// todo los metodos en el program solo corren una vez y no se puede ejecutar metodos asincronos
//para ejecutar metodos asincronos se tienes que agregar wait al llamado como en el metodo SeeData
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shoping.Data;
using Shoping.Data.Entities;
using Shoping.Helpers;
using Vereyon.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//inyectamos la base de datos
builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//Aqui se indica como se crearan los usuarios, se indica que se usara nuestra clase user
//y como seran las contraseñas
//TODO: hacer mas fuerte la contraseña
builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    //generador de tokens se puede poner otro sino dejar el por defecto
    cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    cfg.SignIn.RequireConfirmedEmail = true; //respuesta de confirmacion de correo
    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 0;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequireUppercase = false;
    cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    cfg.Lockout.MaxFailedAccessAttempts = 3;
    cfg.Lockout.AllowedForNewUsers = true;

}).AddDefaultTokenProviders() //agregar para usar tokens
  .AddEntityFrameworkStores<DataContext>();

//configuracion de las cookies, cuando hay problemas de login o acceso denegado
//envia a la visata noauthorized
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/NotAuthorized";
    options.AccessDeniedPath = "/Account/NotAuthorized";
});

// hay 3 tipos de inyeccion trasient, scope y singlenton
// trasient se usa una sola vez y se destruye de memoria
//scope se usa a peticion y se destruye despues que se llama
//singlenton siempre queda en memoria
//inyectamos el alimentador

//en la inyeccion se puede pasar la interface y la clase para poder usar pruebas unitarias
//sino se realizaran se puede pasar solo la clase
builder.Services.AddTransient<SeedDb>();
builder.Services.AddScoped<IBlobHelper, BlobHelper>();
builder.Services.AddScoped<IMailHelper, MailHelper>();
builder.Services.AddScoped<ICombosHelper,CombosHelper>();
builder.Services.AddScoped<IUserHelper,UserHelper>();
builder.Services.AddScoped<IOrdersHelper, OrdersHelper>();
builder.Services.AddFlashMessage();

var app = builder.Build();

SeedData();

void SeedData()
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope? scope = scopedFactory.CreateScope())
    {
        SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
        //se usa wait para ejecutar el asincrono en un contexto que no es asincrono, sirve para todos los casos
        service.SeedAsync().Wait();
    }

}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
//cada vez que haya un error que nos envia a error en el home controller 
app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//indica que se usara autenticacion de usuarios
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
