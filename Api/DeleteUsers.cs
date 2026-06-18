using Microsoft.EntityFrameworkCore;
using TaskFour.Types;

namespace TaskFour.Api
{
	[ApiRequest("DELETE")]
	public class DeleteUsers : AdminApiRequest, IApiRequest
	{
		public class Request
		{
			public List<int> Ids { get; set; } = null!;
			public bool? OnlyNotVerified { get; set; }
		}

		protected override async Task<(int?, object?)> GetStatusCode(HttpContext httpContext)
		{
			Request? request = await httpContext.Request.ReadFromJsonAsync<Request>();
			string? response = request is null || (request.Ids.Count == 0 && request.OnlyNotVerified is null) ? "Not enough users' ID to delete"
				: request.Ids.Count > 0 && request.OnlyNotVerified is bool onlyNotVerified && onlyNotVerified ? "Cannot delete users by IDs and only not verified at one time" : null;
			if (response is not null || request is null)
			{
				return (400, response);
			}
			
			if (request.Ids.Count == 0)
			{
				await Task4Context.Instance.Users.Where((Db.User user) => user.Status == 0).ForEachAsync((Db.User user) =>
				{
					request.Ids.Add(user.Id);
				});
			}

			int? count = null;
			using var transaction = Task4Context.Instance.Database.BeginTransaction();
			try
			{
				Task4Context.Instance.VerifyGuids.RemoveRange(Task4Context.Instance.VerifyGuids.Where((Db.VerifyGuid verifyGuid) => request.Ids.Contains(verifyGuid.UserId)));
				Task4Context.Instance.SignInTimestamps.RemoveRange(Task4Context.Instance.SignInTimestamps.Where((Db.SignInTimestamp timestamp) => request.Ids.Contains(timestamp.UserId)));
				Task4Context.Instance.Users.RemoveRange(Task4Context.Instance.Users.Where((Db.User user) => request.Ids.Contains(user.Id)));

				Task4Context.Instance.SaveChanges();
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}

			return (200, count);
		}
	}
}