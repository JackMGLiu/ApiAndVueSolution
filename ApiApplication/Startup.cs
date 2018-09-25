using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApiApplication.Aop;
using ApiApplication.AuthHelper;
using ApiApplication.AuthHelper.OverWrite;
using ApiApplication.Common.Redis;
using ApiApplication.IService;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace ApiApplication
{
    /// <summary>
    /// DotnetCore配置类
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 配置类属性
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services) //public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            services.AddScoped<ICaching, MemoryCaching>(); //缓存注入
            services.AddScoped<IRedisCacheManager, RedisCacheManager>(); //Redis缓存注入

            #region Automapper

            services.AddAutoMapper(typeof(Startup)); //这是AutoMapper的2.0新特性

            #endregion

            #region swagger

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1.0.0",
                    Title = "GeekFM.Core API",
                    Description = "框架说明文档",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact
                        {Name = "GeekFM.Core", Email = "125267283@qq.com", Url = "https://home.cnblogs.com/u/slenet/"}
                });

                #region 读取xml信息

                //var basePath = PlatformServices.Default.Application.ApplicationBasePath;  //2.0配置
                //var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath; //2.1配置
                var xmlPath = Path.Combine(basePath, "ApiApplication.xml");//这个就是刚刚配置的xml文件名
                options.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                #endregion

                #region Token绑定到ConfigureServices

                //添加header验证信息
                //c.OperationFilter<SwaggerHeader>();
                var security = new Dictionary<string, IEnumerable<string>> {{"GeekFM.Core", new string[] { }}};
                options.AddSecurityRequirement(security);
                //方案名称“GeekFM.Core”可自定义，上下一致即可
                options.AddSecurityDefinition("GeekFM.Core", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入{token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });

                #endregion
            });

            #endregion

            #region 认证

            //services.AddAuthentication(x =>
            //    {
            //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(o =>
            //    {
            //        o.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidIssuer = "GeekFM.Core",
            //            ValidAudience = "wr",
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtHelper.secretKey)),


            //            RequireSignedTokens = true,
            //            // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
            //            ValidateAudience = false,
            //            ValidateIssuer = true,
            //            ValidateIssuerSigningKey = true,
            //            // 是否要求Token的Claims中必须包含 Expires
            //            RequireExpirationTime = true,
            //            // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
            //            ValidateLifetime = true
            //        };
            //    });

            //登陆，就是验证用户登陆以后，通过个人信息（用户名 + 密码），调取数据库数据，根据权限，生成一个令牌
            //认证，就是根据登陆的时候，生成的令牌，检查其是否合法，这个主要是证明没有被篡改
            //授权，就是根据令牌反向去解析出的用户身份，回应当前http请求的许可，表示可以使用当前接口，或者拒绝访问

            #endregion

            #region Token服务注册

            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("Admin", policy => policy.RequireClaim("AdminType").Build()); //注册权限管理，可以自定义多个
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("AdminOrClient", policy => policy.RequireRole("Admin,Client").Build());
            });

            #endregion

            #region Autofac

            //实例化 AutoFac  容器   
            var builder = new ContainerBuilder();

            builder.RegisterType<AopDemo>();//可以直接替换其他拦截器

            //注册要通过反射创建的组件
            //builder.RegisterType<AdvertisementService>().As<IAdvertisementService>();

            //var assemblysServices = Assembly.Load("ApiApplication.Service");
            //builder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces();//指定已扫描程序集中的类型注册为提供所有其实现的接口。

            //var assemblysRepository = Assembly.Load("ApiApplication.Repository");
            //builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();


            var servicesDllFile = Path.Combine(basePath, "ApiApplication.Service.dll");//获取项目绝对路径
            var assemblysServices = Assembly.LoadFile(servicesDllFile);//直接采用加载文件的方法
            builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                .InterceptedBy(typeof(AopDemo));//可以直接替换拦截器

            var repositoryDllFile = Path.Combine(basePath, "ApiApplication.Repository.dll");
            var assemblysRepository = Assembly.LoadFile(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();

            //将services填充到Autofac容器生成器中
            builder.Populate(services);

            //使用已进行的组件登记创建新容器
            var ApplicationContainer = builder.Build();

            #endregion

            //第三方IOC接管 core内置DI容器
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            #region swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
            });

            #endregion

            //验证中间件
            //app.UseMiddleware<TokenAuth>();
            app.UseMiddleware<JwtTokenAuth>();

            app.UseMvc();
        }
    }
}
