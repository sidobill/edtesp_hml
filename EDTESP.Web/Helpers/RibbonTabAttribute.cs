using System;

namespace EDTESP.Web.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RibbonTabAttribute : Attribute
    {
        public string Tab { get; set; }
    }
}