using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;
using TaskFour.Types;

namespace TaskFour
{
    public class Program
    {
        public static void Main(string[] args)
		{
			Console.WriteLine("DB connection string: " + Task4Context.Instance.Database.GetConnectionString() + Environment.NewLine);

			var builder = WebApplication.CreateBuilder(args); 
            builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession();

			var app = builder.Build();
            app.UseSession();
            app.UseStaticFiles();

            app.MapGet("/", async (HttpContext httpContext) =>
            {
                if (httpContext.Session.IsAvailable && httpContext.Session.GetInt32("userId") is int userId && httpContext.Session.GetString("userEmail") is string userEmail
                    && httpContext.Session.GetString("userPassword") is string userPassword)
				{
                    if (Task4Context.Instance.Users.Find(userId) is Db.User user && user.Email == userEmail && user.Password == userPassword)
                    {
                        httpContext.Response.Redirect("/admin.html");
                        return;
                    }                    
				}

				httpContext.Response.Redirect("/sign-in.html");
			});

            IEnumerable<Type> types = typeof(Program).Assembly.GetTypes().Where((Type type) => type.GetInterface(nameof(IApiRequest)) is not null);
            foreach (Type type in types)
            {
                ApiRequestAttribute attribute = type.GetCustomAttribute<ApiRequestAttribute>() ?? throw new NullReferenceException();

                app.MapMethods($"/api/{type.Name.ToLower()}", [attribute.HttpMethod.ToUpper()], async(HttpContext httpContext) =>
				{
					ConstructorInfo constructorInfo = type.GetConstructor([]) ?? throw new NullReferenceException();
					object apiRequestObject = constructorInfo.Invoke([]) ?? throw new NullReferenceException();

                    (int?, object?) response = (null, null);
					try
                    {
						response = await ((IApiRequest)apiRequestObject).RespondAsync(httpContext);
                    }
                    catch (Exception ex)
                    {
                        response.Item1 = 500;
                        response.Item2 = (ex.InnerException is Exception inner ? inner : ex).Message;
                    }

                    httpContext.Response.StatusCode = response.Item1 ?? httpContext.Response.StatusCode;
                    if (response.Item2 is not null)
                        await httpContext.Response.WriteAsJsonAsync(response.Item2);
				});
            }

            app.Run();
        }
    }
}