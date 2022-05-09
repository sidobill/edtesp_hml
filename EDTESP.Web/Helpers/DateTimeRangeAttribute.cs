using System;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.Helpers
{
    public class DateTimeRangeAttribute : RangeAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }

        public DateTimeRangeAttribute() : base(typeof(DateTime), DateTime.MinValue.ToLongDateString(), DateTime.MaxValue.ToLongDateString())
        {
            
        }
    }
}