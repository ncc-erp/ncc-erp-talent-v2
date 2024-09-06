using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TalentV2.Entities;

namespace TalentV2.Constants.Const
{
    class CvAnnotationValue
    {
        public static int Phone = typeof(CV).GetProperty("Phone").GetCustomAttribute<MaxLengthAttribute>().Length;
        public static int Address = typeof(CV).GetProperty("Address").GetCustomAttribute<MaxLengthAttribute>().Length;
        public static int Name = typeof(CV).GetProperty("Name").GetCustomAttribute<MaxLengthAttribute>().Length;
    }
}
