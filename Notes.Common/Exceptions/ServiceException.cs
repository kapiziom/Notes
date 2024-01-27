using System.Collections;
using System.Reflection;

namespace Notes.Common.Exceptions
{
    public class ServiceException : Exception
    {
        public readonly ExceptionEnum Type;

        protected ServiceException(string message, ExceptionEnum type = ExceptionEnum.BadRequest) : base(message)
        {
            Type = type;
        }

        public override IDictionary Data =>
            GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .ToDictionary(o => o.Name, o => o.GetValue(this));
    }
}