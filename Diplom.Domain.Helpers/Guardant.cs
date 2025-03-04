namespace Diplom.Domain.Helpers;

public sealed class Guardant
{
    public static Guardant Instance { get; } = new ();

    // private Guardant()
    // {
    //     
    // }
    
    public Guardant ThrowIfEmpty<T>(
        IEnumerable<T> value)
    {
        //return ThrowIf(value, passedValue => !passedValue.Any())
        if (!value.Any())
        {
            throw new GuardantException("Value is empty enumerable.");
        }
        
        return this;
    }
    
    public Guardant ThrowIfAny<T>(
        IEnumerable<T?> value,
        Predicate<T?> predicate,
        string exceptionMessage)
    {
        if (value.Any(new Func<T?, bool>(predicate)))
        {
            throw new GuardantException(exceptionMessage);
        }

        return this;
    }

    public Guardant ThrowIfEqual<T>(
        T initialValue,
        T valueToCompareWith)
        where T : IEquatable<T>
    {
        if (initialValue.Equals(valueToCompareWith))
        {
            throw new GuardantException("Initial value is equal to other value by inner equality comparison.");
        }

        return this;
    }

    
    
    public Guardant ThrowIf<T>(
        T? value,
        Predicate<T?> predicate,
        string exceptionMessage)
    {
        if (predicate(value))
        {
            throw new GuardantException(exceptionMessage);
        }
    
        return this;
    }
    
    public Guardant ThrowIfNull<T>(
        T? value)
        where T : class
    {
        return ThrowIf(value, passedValue => passedValue is null, "Value is null.");
    }
    
    public Guardant ThrowIfNullOrEmpty<T>(
        IEnumerable<T>? value)
    {
        return ThrowIfNull(value).ThrowIfEmpty(value!);
    }
    public Guardant ThrowIfNotEqual<T>(
        T initialValue,
        T valueToCompareWith)
        where T : IEquatable<T>
    {
        if (!initialValue.Equals(valueToCompareWith))
        {
            throw new GuardantException("Initial value is not equal to other value by inner equality comparison.");
        }
        
        return this;
    }
    
    public Guardant ThrowIfLowerThanOrEqualTo<T>(
        T initialValue,
        T valueToCompareWith)
        where T: IComparable<T>
    {
        if (initialValue.CompareTo(valueToCompareWith) <= 0)
        {
            throw new GuardantException("Initial value is LT or EQ to other value by inner comparison.");
        }

        return this;
    }
    
    public Guardant ThrowIfGreaterThan<T>(
        T initialValue,
        T valueToCompareWith)
        where T: IComparable<T>
    {
        if (initialValue.CompareTo(valueToCompareWith) > 0)
        {
            throw new GuardantException("Initial value is GT other value by inner comparison.");
        }

        return this;
    }
    public Guardant ThrowIfLowerThan<T>(
        T initialValue,
        T valueToCompareWith)
        where T : IComparable<T>
    {
        if (initialValue.CompareTo(valueToCompareWith) < 0)
        {
            throw new GuardantException("Initial value is LT other value by inner comparison.");
        }

        return this;
    }
    
    public Guardant ThrowIfGreaterThanOrEqualTo<T>(
        T initialValue,
        T valueToCompareWith)
        where T: IComparable<T>
    {
        if (initialValue.CompareTo(valueToCompareWith) >= 0)
        {
            throw new GuardantException("Initial value is GT or EQ to other value by inner comparison.");
        }
        
        return this;
    }
    
    public Guardant ThrowIfGreaterThanOrEqualTo<T>(
        T initialValue,
        T valueToCompareWith,
        IComparer<T> comparer)
    {
        if (comparer.Compare(initialValue, valueToCompareWith) >= 0)
        {
            throw new GuardantException("Initial value is GT or EQ to other value by outer comparison.");
        }

        return this;
    }
    
    

}