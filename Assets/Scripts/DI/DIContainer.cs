using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DI
{
    public class DIContainer
    {
        private readonly Dictionary<Type, Type> _services = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        // Регистрация зависимостей
        public void Register<TService, TImplementation>()
            where TImplementation : TService
        {
            _services[typeof(TService)] = typeof(TImplementation);
        }

        // Регистрация существующего экземпляра
        public void RegisterInstance<TService>(TService instance)
        {
            _instances[typeof(TService)] = instance;
        }

        // Разрешение зависимостей
        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        private object Resolve(Type serviceType)
        {
            if (_instances.ContainsKey(serviceType))
            {
                return _instances[serviceType];
            }

            if (!_services.ContainsKey(serviceType))
            {
                throw new Exception($"Service of type {serviceType} is not registered");
            }

            var implementationType = _services[serviceType];
            var constructorInfo = implementationType.GetConstructors().FirstOrDefault();
            var parameters = constructorInfo?.GetParameters();
            var parameterImplementations = new List<object>();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var parameterImplementation = Resolve(parameter.ParameterType);
                    parameterImplementations.Add(parameterImplementation);
                }
            }

            var implementation = Activator.CreateInstance(implementationType, parameterImplementations.ToArray());
            _instances[serviceType] = implementation;

            InjectDependencies(implementation);

            return implementation;
        }

        private void InjectDependencies(object instance)
        {
            var type = instance.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // Inject fields
            foreach (var field in fields)
            {
                if (Attribute.IsDefined(field, typeof(InjectAttribute)))
                {
                    var serviceType = field.FieldType;
                    var serviceInstance = Resolve(serviceType);
                    field.SetValue(instance, serviceInstance);
                }
            }

            // Inject properties
            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(InjectAttribute)) && property.CanWrite)
                {
                    var serviceType = property.PropertyType;
                    var serviceInstance = Resolve(serviceType);
                    property.SetValue(instance, serviceInstance);
                }
            }
        }
    }
}
