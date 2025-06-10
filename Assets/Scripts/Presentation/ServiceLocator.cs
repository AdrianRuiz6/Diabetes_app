// Código basado en un tutorial de ThePowerUps
// Fuente: https://thepowerups-learning.com/descargar-ejemplo-servicelocator/
// Modificado para ajustarse a las necesidades del proyecto

using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public class ServiceLocator
{
    private static ServiceLocator _instance;
    public static ServiceLocator Instance => _instance ?? (_instance = new ServiceLocator());

    private readonly Dictionary<Type, object> _services;

    public ServiceLocator()
    {
        _services = new Dictionary<Type, object>();
    }

    public void RegisterService<T>(T service)
    {
        var type = typeof(T);
        Assert.IsFalse(
            condition: _services.ContainsKey(type),
            message: $"Service {type} already registered"
            );

        _services.Add(type, service);
    }

    public T GetService<T>()
    {
        var type = typeof (T);
        if (!_services.TryGetValue(type, out var service))
        {
            throw new Exception(message: $"Service {type} not found");
        }

        return (T)service;
    }
}
