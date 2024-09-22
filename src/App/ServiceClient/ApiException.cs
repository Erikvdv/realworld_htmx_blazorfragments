namespace RealworldBlazorHtmx.App.ServiceClient;

public class ApiException : Exception
{
    public ApiException(Dictionary<string, string[]> errorList)
    {
        ErrorList = errorList;
    }

    public Dictionary<string, string[]> ErrorList { get; }
}