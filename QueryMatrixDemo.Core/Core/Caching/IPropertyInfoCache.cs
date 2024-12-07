using System.Reflection;

namespace QueryMatrixDemo.Core.Core.Caching;

public interface IPropertyInfoCache
{
    PropertyInfo GetProperty<T>(string propertyName);
}