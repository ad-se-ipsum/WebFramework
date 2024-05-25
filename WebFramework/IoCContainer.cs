namespace WebFramework;

public interface IIoCContainer
{
    void Register<TContract, TImplementation>() where TImplementation : TContract;
    void Register<TImplementation>() where TImplementation : class;
    TContract Resolve<TContract>();
    object Resolve(Type contractType);
}

public class IoCContainer : IIoCContainer
{
    private Dictionary<Type, Type> _container = new Dictionary<Type, Type>(); 
        
    public void Register<TContract, TImplementation>() where TImplementation : TContract
    {
        if (_container.TryGetValue(typeof(TContract), out var value))
        {
            throw new InvalidOperationException($"Contract {nameof(TContract)} with Implementation {nameof(TImplementation)} already was added.");
        }

        _container[typeof(TContract)] = typeof(TImplementation);
    }

    public void Register<TImplementation>() where TImplementation : class
    {
        if (_container.TryGetValue(typeof(TImplementation), out var value))
        {
            throw new InvalidOperationException($"Implementation {nameof(TImplementation)} already was added.");
        }

        _container[typeof(TImplementation)] = typeof(TImplementation);
    }

    public object Resolve(Type contractType)
    {
        return ResolveByType(contractType);
    }

    public TContract Resolve<TContract>()
    {
        return (TContract)ResolveByType(typeof(TContract));
    }

    private object ResolveByType(Type contractType)
    {
        if (!_container.TryGetValue(contractType, out var implementation))
        {
            throw new InvalidOperationException($"Implementation for Contract {contractType} did not registered.");
        }

        var constructor = implementation.GetConstructors().Where(c => c.IsPublic && !c.IsStatic).ToList()[0];
        if (constructor == null)
        {
            throw new InvalidOperationException($"Implementation {nameof(implementation)} does not have valid constructor");
        }

        var constructorParameters = constructor.GetParameters();

        var parameterInstances = new object[constructorParameters.Length];
        for (var i = 0; i < constructorParameters.Length; i++)
        {
            parameterInstances[i] = Resolve(constructorParameters[i].ParameterType);
        }

        return constructor.Invoke(parameterInstances);
    }
}