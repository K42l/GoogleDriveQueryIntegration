# Google.Drive.Integration

### A class library with some helpful methods and a way of setting the query, for listing files, with a class instead of doing it manually

# Methods

- CreateFolderAsync
- UploadFileAsync
- ListAsync
- GetSharedDrivesAsync
- GetAllTeamDrivesAsync
- DownloadFileAsync
- GetFileInfoAsync

Note: I used the ActionResult from MVC.Core on these methods because if the response status code, in the Google.Apis.Drive ExecuteAsync method, is anything but 200, it will throw a exception

# Class for the query (FileTerms)

I created a class with all of the terms for the files.list query. And a boolean called IsFolder as shorthand for the MimeType.
Each Term's class has:
- A OperatorsEnum with the operators supported for that term
- A Value property that can be a String, Boolean, Enum or DateTime
- A Enum called 'AndOrEnum', to set and/or on the string for the next term
- A Boolean called 'EncapsulateWithNext' (false as default). If true will open a '(' before the term and will only close if the next term's 'EncapsulateWithNext' is false or at the last term
- A Boolean called negate that will add a 'not' at the begining of the term

# Usage
The ListAsync(FileTerms terms, ...) uses the class CustomListRequest that extends from the ListRequest class.<br>
The difference is that the CustomListRequest constructor needs the service to be passed as a parameter. 

To use the method ListAsync(FileTerms terms, ...) you just need to create a instance of the FileTerms with the terms you want to search. Example:
```
FileTerms fileTerms = new FileTerms
{
    Name = new(),
    Parents = new()
};

fileTerms.Name.Add(new Name { Value = "foo", Operator = Name.OperatorEnum.Contains, EncapsulateWithNext = true, AndOr = And });
fileTerms.Parents.Add(new Parents { Value = "bar", Operator = Parents.OperatorEnum.In, AndOr = Or });

fileTerms.Name.Add(new Name { Value = "fubar", Operator = Name.OperatorEnum.Equal, AndOr = And, EncapsulateWithNext = true });
fileTerms.Parents.Add(new Parents { Value = "baz", Operator = Parents.OperatorEnum.In, AndOr = And });

fileTerms.IsFolder = false;

var files = await integration.ListAsync(fileTerms);
```

The above fileTerms will generate the following query:
```
(name contains 'foo' and 'bar' in parents) or (name = 'fubar' and 'baz' in parents) and mimeType != 'application/vnd.google-apps.folder'
```

# Disclaimer

I used reflection to get the non null terms from the FileTerms class, in order of instantiation, to set the query.<br>
It is my first time using reflection in this way. I Haven't fully tested all the options and how the reflection would behave with multiple requests.
Even though I'm setting the Terms as null after the query is set, there may be some unexpected behaviors.

This would probably work better with a method that takes a instance of the FileTerms and create the query from there, especially where performance is concerned.

My goal here was simply to see if I could create the query with only one class, without having to pass it through a method, and a minimum amount of code to use it.

# Note

The CreateConnection from the ConnectionServiceAccount class takes the Service Account JSON directly from the application folder, THIS IS NOT A GOOD PRACTICE AND IT'S ONLY HERE TO SIMPLIFY THE TESTING OF THIS LIBRARY.