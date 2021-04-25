using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReadBay.DataAccess.Data;
using ReadBay.DataAccess.Repository;
using ReadBay.DataAccess.Repository.IRepository;
using ReadBay.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadBay
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()     //AdddefaultIdentity does not have support for IdentityRole confirm signin required also removed and add default topken providers added
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<EmailOptions>(Configuration);
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));
            services.AddScoped<IUnitOfWork, UnitOfWork>(); //UnitOfWork added as part of dependency injection  
            services.AddControllersWithViews(); //.AddRazorRuntimeCompilation(); Needed for 3.1 and below
            services.AddRazorPages();  //Needed when Scaffolding identity
            services.ConfigureApplicationCookie(options =>
            {   
                // Reflects the correct Area in the WebUrl
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
            // Facebook Login
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "203049561271446";
                options.AppSecret = "db0a86f0a89534f100025573823bd08e";
            });            
            // Google Login
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "994167471286-cjbjtpn5ti8sn4fl8m2vsf37ftk5uujf.apps.googleusercontent.com";
                options.ClientSecret = "TRyy-M4j52BI2N2dIyY34lxR";
            });
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
