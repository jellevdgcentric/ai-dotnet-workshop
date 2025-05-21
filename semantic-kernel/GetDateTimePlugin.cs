using System.ComponentModel;
using Microsoft.SemanticKernel;

public class GetDateTimePlugin
{
    [KernelFunction("get_datetime")]
    [Description("Gets the current date and time in the format yyyy-MM-DD HH:mm, For example: 2024-10-25 16:45")]
    public string GetDateTime()
    {
        //return datetime.now in format yyyy-MM-DD HH:mm
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}
