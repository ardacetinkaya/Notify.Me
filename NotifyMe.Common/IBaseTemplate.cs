using System;

namespace NotifyMe.Common
{
    public interface IBaseTemplate
    {
        string Create(string message, string from, string image);
    }
}
