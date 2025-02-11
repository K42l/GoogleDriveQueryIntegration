//using Google.Drive.Query.Integration;
//using Google.Drive.Query.Integration.Interface;
//using Google.Drive.Query.Integration.Query.File;
//using static Google.Drive.Query.Integration.Query.TermsBase.AndOrEnum;

//var appName = "test";
//var credentialsPath = Directory.GetCurrentDirectory() + "\\my-drive-integration.json";

//IIntegration integration = new Integration(appName, credentialsPath);

//FileTerms fileTerms = new FileTerms
//{
//    Name = new(),
//    Parents = new()
//};

//fileTerms.Name.Add(new Name { Value = "foo", Operator = Name.OperatorEnum.Contains, EncapsulateWithNext = true, AndOr = And });
//fileTerms.Parents.Add(new Parents { Value = "bar", Operator = Parents.OperatorEnum.In, AndOr = Or });

//fileTerms.Name.Add(new Name { Value = "fubar", Operator = Name.OperatorEnum.Equal, AndOr = And, EncapsulateWithNext = true });
//fileTerms.Parents.Add(new Parents { Value = "baz", Operator = Parents.OperatorEnum.In, AndOr = And });

//fileTerms.IsFolder = false;

//var drives = await integration.GetAllTeamDrivesAsync();
//var files = await integration.ListAsync(fileTerms);
//var file = await integration.ListAsync("");

//return 0;