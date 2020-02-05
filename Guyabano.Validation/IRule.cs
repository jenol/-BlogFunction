using System;
using System.Threading.Tasks;

namespace Guyabano.Validation
{
    public interface IRule<TK>
    {
        IRule<TK> Must(Predicate<TK> predicate);
        IRule<TK> MustAsync(Func<TK, Task<bool>> predicate);
    }

    public static class A
    {
        public static IRule<TK> PipeOnlyValids<TK>(this IRule<TK> a)
        {
            return a;
        }

        public static IRule<TKK> PipeOnlyValids<TK, TKK>(this IRule<TK> a, Func<TK, TKK> selector)
        {
            return null;
        }

        public static IRule<TK> WithMessage<TK>(this IRule<TK> a, string message)
        {
            return a;
        }

        public static IRule<TK> WithMessage<TK>(this IRule<TK> a, Func<TK, string> message)
        {
            return a;
        }
    }
}
