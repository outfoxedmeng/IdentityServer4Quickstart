using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;

namespace MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies")//��ӿ��Դ���cookies�Ĵ������
                .AddOpenIdConnect("oidc", options =>
                 {
                     options.Authority = "https://localhost:5001";

                     options.ClientId = "mvc-client";
                     options.ClientSecret = "mvc-secret";
                     options.ResponseType = "code";

                     //Getting claims from the UserInfo endpoint
                     options.Scope.Clear();
                     options.Scope.Add("openid");
                     options.Scope.Add("profile");

                     //�Ǳ�׼IdentityResource
                     options.Scope.Add("myIdentityResource1");
                     
                     //ע�⣬����Ĳ�����ָ���ķǱ�׼ClaimType��������IdneityResource������
                     options.ClaimActions.MapUniqueJsonKey("myclaim1", "myclaim1");


                     // keeps id_token smaller
                     options.GetClaimsFromUserInfoEndpoint = true;

                     options.SaveTokens = true;
                 });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                .RequireAuthorization(); //�������������ʣ�Ҳ����ѡ���Ե�ʹ��[Authorize]��

            });
        }
    }
}
