using Microsoft.EntityFrameworkCore;
using TaskFour.Types;

namespace TaskFour.Api
{
	[ApiRequest("PATCH")]
	public class SetUsersStatus : AdminApiRequest, IApiRequest
	{
		public class Request
		{
			public int Status { get; set; }
			public int[] Ids { get; set; } = null!;
		}

		protected override async Task<(int?, object?)> GetStatusCode(HttpContext httpContext)
		{
			Request? request = await httpContext.Request.ReadFromJsonAsync<Request>();
			string? response = request is null || request.Ids.Length == 0 ? $"Not enough users' ID to set status" : null;
			if (response is not null || request is null)
			{
				return (400, response);
			}

			using var transaction = Task4Context.Instance.Database.BeginTransaction();
			try
			{
				await Task4Context.Instance.Users.Where((Db.User user) => request.Ids.Contains(user.Id)).ForEachAsync((Db.User user) =>
				{
					user.Status = request.Status;
				});

				Task4Context.Instance.SaveChanges();
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}

			return (200, null);
		}
	}
}