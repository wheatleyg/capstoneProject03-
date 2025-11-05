# Software Development Capstone Backend Template
README last updated: October 31, 2025 :jack_o_lantern:

---
## Overview
This repository is built to give students at FCHS a good starting point for
their first semester projects. It has a few things that will make life a
little easier, and to provide a template for some things that might be hard
to figure out without examples.

### CapstoneBackend.Auth
This project contains a basic authentication system so that you can readily
assign user ids to items in your database without having to write it all
yourself. You don't ***NEED*** to use this, but it's there if you want.
There's a script in the `DatabaseScripts` folder that creates the table for
you, so run that before you start trying to implement auth functionality in
your codebase. The auth solution is pretty weird compared to other code
you've likely seen, so it has its own README file to help expalain some of
what's in there and how to use it. You shouldn't have to add anything here.

### CapstoneBackend.Core
This project is where most of your code will go (if you'd like). You can
create a new project if you'd like, but this is already set up for you to
start adding controllers, services, and repositories. There are a couple
things that are worth pointing out:
- `appsettings.json` and `appsettings.Development.json` are configuration files
that our program will pull values from such as our database connection
strings. You'll want to update these to work with your local setup. The
Development file is the main one you'll care about as we start the project.
The other file is for if you want to deploy your code onto a server.
- `Program.cs` and `Startup.cs` are what get called when you run your program.
`Program.cs` is intentionally pretty thin, and most of the actual project
configuration happens in the `Startup.cs` file. If you want to add dependency
injection (you do), that's where you'll want to register your services. I don't
believe you'll need to do much else with these files though.

### CapstoneBackend.Test
This project is where your unit tests go. If you are interested, the unit testing
framework we're using is called [xUnit](https://xunit.net/). There are other
frameworks for .NET out there (mainly NUnit and MSTest), and you can explore
those if you'd like. xUnit is just what I chose because I've used it more than
the others. We're also using [moq](https://github.com/devlooped/moq) to remove
dependencies for the sake of testing. We can mock interfaces easily with this
library and even define custom behavior for our methods if we need. Unit testing
can be really frustrating when you get started (sometimes because of how your
code is structured, sometimes because of the exact syntax needed for unit tests).
That is expected and there's nothing wrong with you.

### CapstoneBackend.Utilities
This project exists to hold functionality that might be needed in multiple
other projects. For example, the `EnvironmentVariables.cs` file is needed
by the `Core` project *and* the `Auth` project. To help manage that
project dependency, I've created this third project. If you're interested in
learning more, you'll want to read up on circular dependencies. The 'short'
version is that it *would* be fine to put the `EnvironmentVariables` class in
the `Core` project *except* the `Auth` project has to get compiled first. The
alternative solution here was to put the file in the `Auth` project, but it
doesn't have anything to do with authentication or authorization, so logically
that doesn't make sense. More often than not, `Utility` projects end up being
the 'junk drawer' of a solution.

### BrunoCollection
This folder holds a basic collection of REST calls for Bruno. You can open the
collection to get a quicker start rather than creating your own. It also has
some test calls that you can use to make sure your application is configured
correctly before you start coding. There's also examples of how to use the
auth endpoints, and the `login` endpoint will actually store the token in an
variable for you to use in other calls.

### DatabaseScripts
It is what it says it is. Organize is as you see fit.