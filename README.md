DotArgs
=======

Helper library for parsing, validating and processing command line arguments for .NET

Features
=======
- Simple to install (just add one file to your existing project)
- Simple to use (just tell the library what arguments you want to support and the rest ist magic)
- Automatic help page generation based on the arguments you support
- Comes with often used argument types like flags, options, collections and sets
- Yet it can easily be extended by adding new, exotic argument types

Installation
=======
Simply add the CommandLineArgs.cs file to your project. Now you can use the library.
Of course you can add a reference to the generated assembly to your project if you really want to.

Examples
=======
TODO

Why one file?
=======
Having all the code in one big file may be a huge wtf but there is a reason for this:
Having to add an assembly to your project just to be able to parse command line arguments is - in my opinion - the bigger wtf.
So in order to use this library in a project all you have to do is add one single file to your project and you are good to go.

ToDos
=======
- [x] General parsing and validating of command lines
- [x] Auto help page generation
- [ ] Examples
- [ ] Release as nuget "Package"
- [ ] Auto populate object based on attributes
