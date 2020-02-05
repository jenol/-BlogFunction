using System;
using System.Threading.Tasks;

namespace Guyabano.Validation
{
    public class AbstractValidator<T>
    {
        protected IRule<TK> RuleFor<TK>(Func<T, TK> a)
        {
            return null;
        }
    }
}
