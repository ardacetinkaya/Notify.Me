using System;

namespace NotifyMe.Common
{
    public interface IBaseTemplate
    {
        string Name {get;}
        string Create(string message, string from,string friendlyName, string image, DateTimeOffset date,string to);
    }
}
