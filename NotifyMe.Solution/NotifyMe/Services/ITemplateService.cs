
using System.Collections.Generic;
using NotifyMe.Common;

namespace NotifyMe.Services
{
    public interface ITemplateService
    {
        IEnumerable<IBaseTemplate> Templates { get; set; }
        void Load();
        IBaseTemplate GetTemplate(string name);
    }
}