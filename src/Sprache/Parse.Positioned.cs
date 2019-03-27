
namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Construct a parser that will set the position to the position-aware
        /// T on succsessful match.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<T> Positioned<T>(this Parser<T> parser) where T : IPositionAware<T>
        {
            return new Parser<T>(i =>
            {
                var r = parser.TryParse(i);

                if (r.WasSuccessful)
                {
                    return Result.Success(r.Value.SetPos(Position.FromInput(i), r.Remainder.Position - i.Position),
                        r.Remainder);
                }

                return r;
            });
        }
    }
}
