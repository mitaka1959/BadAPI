using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadAPI.Services
{
    public enum ResultStatus
    {
        Success,
        Invalid,
        NotFound,
        Conflict
    }
    public class Result
    {
        public ResultStatus Status { get; }
        public string? Error { get; }
        public bool IsSuccess => Status == ResultStatus.Success;

        protected Result(ResultStatus status, string? error)
        {
            Status = status;
            Error = error;
        }

        public static Result Success() => new(ResultStatus.Success, null);
        public static Result Invalid(string error) => new(ResultStatus.Invalid, error);
        public static Result NotFound(string error) => new(ResultStatus.NotFound, error);
        public static Result Conflict(string error) => new(ResultStatus.Conflict, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(ResultStatus status, T? value, string? error)
            : base(status, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(ResultStatus.Success, value, null);
        public static new Result<T> Invalid(string error) => new(ResultStatus.Invalid, default, error);
        public static new Result<T> NotFound(string error) => new(ResultStatus.NotFound, default, error);
        public static new Result<T> Conflict(string error) => new(ResultStatus.Conflict, default, error);
    }
}
