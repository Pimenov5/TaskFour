namespace TaskFour.Types
{
	public interface IApiRequest
	{
		public Task<(int?, object?)> RespondAsync(HttpContext httpContext);
	}
}