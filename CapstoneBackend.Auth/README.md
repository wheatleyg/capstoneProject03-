# Auth Package Documentation

## Overview
Authentication can be a bit of a pain to implement if you haven't
done it before, so I've built out a basic authorization system that
will let you register users and login. By doing so, if you use the
`00-Users.sql` script I put in the `DatabaseScripts` folder, you'll
be able to link other objects to specific users using a foreign key.

This might be useful if, for example, you only want users to be able
to see things that belong to them. All you need to do is update your
scripts that get a collection of objects like this:
```mysql
SELECT *
FROM MyTable
```
becomes
```mysql
SELECT *
FROM MyTable
WHERE userId = @userId
```
## Abstraction
In general, the rest of your code should not care about the implementation
details of the auth system. To help enforce this, there are only 3
classes that are fully public (and can therefore be readily used
in other projects.) Some classes, such as `AuthRepository` have a public
constructor, but that is mostly to enable dependency injection. Since
the class itself is internal, you shouldn't be able to instantiate one
in other projects.

That said, here are the 3 files you will care about:
### AuthService.cs / IAuthService.cs
This gives you access to the `Register` and `Login` methods so you can use
this auth system in the first place. `Register` will create a user and add
it to the database for you. `Login` will validate that the username and
password combination provided match one found in the database. `Login` also
provides an auth token back to you that can be used with future api calls
to validate who a user is and what they have access to.

### AuthSetup.cs
This gets called during program startup. You just need to call `AuthSetup.AddAuth()`
somewhere in your initialization process.

### UserContext.cs / IUserContext.cs
You can use this to get a user id anywhere in your code if you add `IUserContext`
as a dependency. So for example, if you want to add/override the UserId for
an object before inserting it into the database, You would update your code like this:
```csharp
public class MyService 
{
    private readonly IMyRepo _repo;
    private readonly IUserContext _userContext;
    
    public MyService(IMyRepo repo, IUserContext userContext) 
    {
        repo = _repo;
        _userContext = userContext;
    }
    
    ...
        
    public MyObject InsertObject(MyObject newObject) {
        newObject.UserId = _userContext.GetUserId();
        return _repo.InsertObject(newObject);
    }
}
```
You can also use this functionality to add a where clause to your sql queries
as described in the overview.

That's really all you need to know. This is a first attempt at an auth solution
and as of October 30th, 2025, it is probably not best practice for what you might
write for a production system. No attempts were made to integrate an active directory
or to store keys in user secrets or anything like that. This is just basic
functionality that will enable students to quickly integrate user id's into their
projects.