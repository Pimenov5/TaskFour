using TaskFour.Types;

namespace TaskFour.Api
{
	[ApiRequest("GET")]
	public class CanAdmin : AdminApiRequest, IApiRequest
	{
		protected override Task<(int?, object?)> GetStatusCode(HttpContext context) => throw new NotImplementedException();

		public class Response
		{
			public int Id { get; set; }
			public string Name { get; set; } = null!;
			public string Email { get; set; } = null!;
		}

		public override async Task<(int?, object?)> RespondAsync(HttpContext httpContext)
		{
			if (this.CanAdmin(httpContext.Session, out Db.User? user) && user is not null)
			{
				Response response = new() { Id = user.Id, Name = user.Name, Email = user.Email };
				return (200, response);
			}

			return (401, null);
		}
	}
}