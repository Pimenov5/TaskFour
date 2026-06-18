namespace TaskFour.Types
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ApiRequestAttribute(string httpMethod) : Attribute
	{
		public string HttpMethod = httpMethod;
	}
}