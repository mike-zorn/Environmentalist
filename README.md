Environmentalist
================

Configure your .Net application via environment variables in a manner where
you're not calling ``Environment.GetEnvironmentVariable`` all over the place.

Usage
=====

Define an interface
-------------------
Define your configuration via an interface. E.g.,
```c#
public interface ITestConfig
{
    [FromEnvironment("COOL_PROPERTY")]
    string CoolProperty { get; }
}
```
Here, we're using the ``FromEnvironment`` attribute to tell Environmentalist
which environment variable to use with this property.

Create an instance
------------------
Create an instance of your configuration via
```c#
var config = Environmentalist.Create<ITestConfig>(new
    {
        CoolProperty = "foo"
    }
);
```

The supplied object serves as a set of defaults if Environmentalist can't find
a given variable in the environment

???
---
Environmentalist doesn't assume anything about how you use your configuration. You can just pop the returned object into your IoC container of choice or wrap it in a singleton or static class if you want to use an API closer to the standard way of doing config.
