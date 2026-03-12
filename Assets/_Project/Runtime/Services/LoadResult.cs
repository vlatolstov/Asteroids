namespace _Project.Runtime.Services
{
    public readonly struct LoadResult<T> where T : class
    {
        public bool Found { get; }
        public T Data { get; }

        private LoadResult(bool found, T data)
        {
            Found = found;
            Data = data;
        }

        public static LoadResult<T> Success(T data)
        {
            return new LoadResult<T>(found: true, data);
        }

        public static LoadResult<T> NotFound()
        {
            return new LoadResult<T>(found: false, null);
        }
    }
}
