namespace FlatFiles.TypeMapping
{
    internal interface ITypeMapperInjector
    {
        ITypeMatcherContext SetMatcher(object entity);
    }
}
