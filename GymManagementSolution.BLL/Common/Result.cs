using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Common
{
    public sealed record Result(bool IsSuccess, string? Error=null, ResultType kind= ResultType.Ok)
    {
        public static Result Ok() => new(true);
        public static Result Fail(string error , ResultType kind =ResultType.Conflict) => new(false,error,kind);
        public static Result NotFound(string error="Not Found" ) => new(false, error, ResultType.NotFound);

        public static Result Validation(string error) => new(false, error, ResultType.ValidationFailed);

    }
    public sealed record Result<T>(bool IsSuccess, T? Value,string? Error = null, ResultType kind = ResultType.Ok)
    {
        public static Result<T> Ok(T value) => new(true,value);
        public static Result<T> Fail(string error, ResultType kind = ResultType.Conflict) => new(false, default,error, kind);
        public static Result<T> NotFound(string error = "Not Found") => new(false, default,error, ResultType.NotFound);

        public static Result<T> Validation(string error) => new(false,default, error, ResultType.ValidationFailed);


    }
}
