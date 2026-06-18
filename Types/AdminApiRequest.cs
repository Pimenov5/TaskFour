namespace TaskFour.Types
{
	public abstract class AdminApiRequest
	{
		protected virtual bool CanAdmin(ISession session, out Db.User? user)
		{
			int? userId = session.GetInt32("userId");
			user = userId is null ? null : Task4Context.Instance.Users.Find(userId);
			return user is not null && user.Status != 2;
		}

		protected abstract Task<(int?, object?)> GetStatusCode(HttpContext httpContext);

		public virtual async Task<(int?, object?)> RespondAsync(HttpContext httpContext)
		{
			if (!this.CanAdmin(httpContext.Session, out Db.User? user))
			{
				throw new ArgumentException((user is null ? "Not authorized" : "Blocked") + " users cannot use admin API", nameof(httpContext));
			}

			(int ?, object ?) response = await this.GetStatusCode(httpContext);
			return response;
		}
	}
}