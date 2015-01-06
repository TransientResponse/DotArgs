# DotArgs

Helper library for parsing, validating and processing command line arguments for .NET

# Features
- Simple to install (just add one file to your existing project)
- Simple to use (just tell the library what arguments you want to support and the rest ist magic)
- Automatic help page generation based on the arguments you support
- Comes with often used argument types like flags, options, collections and sets
- Yet it can easily be extended by adding new, exotic argument types

# Installation
You can install DotArgs using Nuget:
> Install-Package DotArgs

This will add a file "CommandLineArgs.cs" to your project and you are good to go.

If you want to install the library by hand simply add the CommandLineArgs.cs file to your project. Now you can use the library.
Of course you can add a reference to the generated assembly to your project if you really want to.

# Examples
We'll start simple. Let's suppose you have a nice tool that takes a file and applies some operation to it.
Additionally there are two options that further define this operation.
The command line you expect would look something like this:
`your-tool filename /option1=some-value /option2=some-other-value`
With `option1` and `option2` being optional arguments.

To get this workin with DotArgs all you need to do would be the following snippet:
```cs
static void Main( string[] args )
{
  CommandLineArgs cmd = new CommandLineArgs();

  // Register the available arguments so they can be parsed from the command line
  cmd.RegisterArgument( "filename", new OptionArgument( null, true ) );
  cmd.RegisterArgument( "option1", new OptionArgument( "default1" ) );
  cmd.RegisterArgument( "option2", new OptionArgument( "some-value" ) );

  // The argument that was registered using the name "filename" will be the default argument
  // The default argument does not need to be set using the argument's name
  cmd.SetDefaultArgument( "filename" );

  // Validate the command line. This will parse it, set values for registered arguments and validate
  // whether everything has correct values.
  if( !cmd.Validate(args) )
  {
    // The passed arguments contained an error or no value for "filename" was given
    // We'll come back to this later
    return;
  }

  // Finally read the values the user provided
  string filename = cmd.GetValue<string>( "filename" );
  string option1 = cmd.GetValue<string>( "option1" );
  string option2 = cmd.GetValue<string>( "option2" );
}
```
Basically this is everything you have to setup to get things running.

## Flags
Now let's say that one of your options is not an option but a flag.
Checking if the value the user has entered for this option is yes/no/true/false/1/0 or whatever would be possible but there is a better solution:
Simply replace the `OptionArgument` type of the option with `FlagArgument`:
```cs
cmd.RegisterArgument( "option1", new FlagArgument( true ) );
// default value of true (will have this value if not specified via command line)
```

Then you can check the value given by using:
```cs
bool isFlagSet = cmd.GetValue<bool>( "option1");
```

## Sets
Now what if the only valid values for `option2` are `one`, `two` and `three`?
Yes manually checking is still an option but you don't have to ;)

There is a `SetArgument` that takes a list of all valid values and does all the validation and processing for you.
So you simply replace the registration of `option2` with the following:
```cs
cmd.RegisterArgument( "option2", new SetArgument( new[]{"one","two","three"}, "two" ) );
```

And that's it. The validation will now be done when the command line is being validated.
Reading the value is still the same call since all these values are still strings.

## Collections
What if you want to process multiple files at once? Add some more optional arguments for the second and third file?
Now what if the user wants to process four files?
Again there is a solution for this: `CollectionArgument`

When you register an argument as a CollectionArgument the user can specify multiple values for the same option:
> /input=file1 /input=file2 ... /input=fileN

Reading the values in your code would be as simple as reading the value for a normal option:
```cs
string[] files = cmd.GetValue<string[]>( "option" );
// files now contains {"file1", "file2", ..., "fileN"}
```

# Formats for the command line
DotArgs supports a wide range of possible format that can be used to specify values for arguments in the command line.

### OptionArgument, SetArgument, CollectionArgument
- /option=value
- /option:value
- /option value
- -option=value
- -option:value
- -option value
- --option=value
- --option value
- --option:value

### FlagArgument
- -flag
- --flag
- /flag

# Why one file?
Having all the code in one big file may be a huge wtf but there is a reason for this:
Having to add an assembly to your project just to be able to parse command line arguments is - in my opinion - the bigger wtf.
So in order to use this library in a project all you have to do is add one single file to your project and you are good to go.
