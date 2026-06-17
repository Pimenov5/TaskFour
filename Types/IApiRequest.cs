namespace TaskFour.Types
{
	public interface IApiRequest
	{
		public Task RespondAsync(HttpContext httpContext);
	}
}
