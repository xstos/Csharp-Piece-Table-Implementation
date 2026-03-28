namespace CsharpPieceTableImplementation
{
    public sealed class StringSpanPool<T>
    {
        private readonly Dictionary<Span, List<T>> _cache = new();

        public List<T>? GetStringFromCache(Span span)
        {
            if (_cache.TryGetValue(span, out List<T>? result))
            {
                return result;
            }

            return null;
        }

        public void Cache(Span span, List<T> entry)
        {
            _cache.TryAdd(span, entry);
        }

        public void Reset()
        {
            _cache.Clear();
        }
    }
}
