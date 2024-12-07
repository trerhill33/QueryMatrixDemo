namespace QueryMatrixDemo.Core.Core.Services.Interfaces;

public interface IValueConverter
{
    object? ConvertValue(object? value, Type targetType);
}
