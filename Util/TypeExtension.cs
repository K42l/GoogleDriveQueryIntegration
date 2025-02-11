namespace Google.Drive.Query.Integration.Util
{
    public static class TypeExtension
    {
        public static bool In<T>(this T obj, params T[] args)
        {
            return args.Contains(obj);
        }
    }
}
