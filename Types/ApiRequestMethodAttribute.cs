namespace TaskFour.Types
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ApiRequestMethodAttribute(string httpMethod) : Attribute
	{
		public string HttpMethod = httpMethod;
	}
}
