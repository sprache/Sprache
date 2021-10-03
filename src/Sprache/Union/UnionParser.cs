using System;

namespace Sprache.Union
{
    public class UnionParser<T> : IUnionParser<T>
    {
        private Func<IInput, IState, UnionResult<T>> _parse;

        public string Name { get; set; }

        public string GetFullName(IState state)
        {
            return string.Join("/", state.Path) + Name;
        }

        public Func<IInput, IState, UnionResult<T>> Parse
        {
            get
            {
                return _parse;
            }

            set
            {
                _parse = (input, state) =>
                {
                    var fullName = GetFullName(state);
                    if (state.HasValue(input.Position, fullName))
                    {
                        return state.GetValue<UnionResult<T>>(input.Position, fullName);
                    }

                    var result = value(input, state);

                    state.Store(input.Position, fullName, result);

                    return result;
                };
            }
        }
    }
}
