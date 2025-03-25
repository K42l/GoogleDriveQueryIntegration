using Google.Apis.Services;
using Google.Drive.Query.Integration.Query.File;
using Google.Drive.Query.Integration.Util;
using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;

namespace Google.Drive.Query.Integration.Request
{
    public class CustomListRequest : Apis.Drive.v3.FilesResource.ListRequest
    {
        public CustomListRequest(IClientService service) : base(service)
        {
            InitParameters();
        }

        /// <summary>
        /// Class with the terms for list.files that will set the query for filtering the results
        /// </summary>
        public FileTerms? Terms
        {
            get { return this.terms; }
            set
            {
				this.terms = value;
				if (value != null) 
                    this.Q = null;
            }
        }
        private FileTerms? terms;

        /// <summary>
        /// Selector specifying which fields to include in a partial response.<br />
        /// Default: "nextPageToken, files(id, name, size, mimeType, parents, createdTime, modifiedTime)"
        /// </summary>
        [Google.Apis.Util.RequestParameterAttribute("fields", Google.Apis.Util.RequestParameterType.Query)]
        public override string? Fields
        {
            get { return this.fields; }
            set
            {
                if (value == null)
                {
                    this.fields = "nextPageToken, files(id, name, size, mimeType, parents, createdTime, modifiedTime, trashed, version, driveId)";
                }
                else
                {
                    this.fields = value;
                }  
            }
        }
        private string? fields;

        /// <summary>
        /// A query for filtering the results.
        /// Use only if you want to set the Query manually.
        /// </summary>
        [Google.Apis.Util.RequestParameterAttribute("q", Google.Apis.Util.RequestParameterType.Query)]
        public override string? Q
        {
            get { return this.q; }
            set
            {
                if (this.Terms != null)
                {
                    var properties = this.Terms.GetType()
                      .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                      .AsParallel()
                      .Where(p => p.GetValue(Terms) != null && !p.GetMethod.IsPrivate && !p.SetMethod.IsPrivate && p.PropertyType.IsClass)
                      .ToList();

                    List<ValidValue> validValues = new();
                    for (int i = 0; i < properties.Count(); i++)
                    {                        
                        if (properties[i].PropertyType.Name != "List`1")
                        {
                            var propType = properties[i].GetType();

                            string name = "";
                            if (!String.IsNullOrEmpty(propType.Name))
                                name = Char.ToLower(propType.Name[0]) + propType.Name.Substring(1);

                            var val = propType.GetProperty("Value").GetValue(properties[i]);
                            var op = propType.GetProperty("Operator").GetValue(properties[i]) as Enum;
                            var andOr = propType.GetProperty("AndOr").GetValue(properties[i]) as Enum;
                            var negateSearchQuery = (propType.GetProperty("NegateSearchQuery").GetValue(properties[i]) as bool?).Value;
                            var encapsulateWithNext = (propType.GetProperty("EncapsulateWithNext").GetValue(properties[i]) as bool?).Value;
                            var createdTime = (propType.GetProperty("CreatedTime").GetValue(properties[i]) as DateTime?).Value;

                            string? key = null;
                            if (propType.GetProperty("Key") != null)
                                key = (string?)propType.GetProperty("Key").GetValue(properties[i]);

                            if (val.GetType().IsEnum)
                                val = (val as Enum).GetStringValue();

                            if(propType.GetProperty("Value").PropertyType == typeof(DateTime))
                                val = (val as DateTime?).Value.ToUniversalTime(); 

                            ValidValue validValue = new()
                            {
                                Name = name,
                                Value = val,
                                Key = key,
                                Operator = op.GetStringValue(),
                                AndOr = andOr.GetStringValue(),
                                EncapsulateWithNext = encapsulateWithNext,
                                CreatedTime = createdTime
                            };
                            validValues.Add(validValue);
                        }
                        else
                        {
                            var objectArray = (IList)Terms.GetType().GetProperty(properties[i].Name).GetValue(Terms);
                            if (objectArray == null) continue;
                            for (int j = 0; j < objectArray.Count; j++)
                            {
                                var propType = objectArray[j].GetType();

                                string name = "";
                                if (!String.IsNullOrEmpty(propType.Name))
                                    name = Char.ToLower(propType.Name[0]) + propType.Name.Substring(1);

                                var val = propType.GetProperty("Value").GetValue(objectArray[j]);
                                var op = propType.GetProperty("Operator").GetValue(objectArray[j]) as Enum;
                                var andOr = propType.GetProperty("AndOr").GetValue(objectArray[j]) as Enum;
                                var negateSearchQuery = (propType.GetProperty("NegateSearchQuery").GetValue(objectArray[j]) as bool?).Value;
                                var encapsulateWithNext = (propType.GetProperty("EncapsulateWithNext").GetValue(objectArray[j]) as bool?).Value;
                                var createdTime = (propType.GetProperty("CreatedTime").GetValue(objectArray[j]) as DateTime?).Value;
                                
                                string? key = null;
                                if (propType.GetProperty("Key") != null)
                                    key = (string?)propType.GetProperty("Key").GetValue(objectArray[j]);

                                if (val.GetType().IsEnum)
                                    val = (val as Enum).GetStringValue();

                                if (propType.GetProperty("Value").PropertyType == typeof(DateTime))
                                    val = (val as DateTime?).Value.ToUniversalTime();

                                ValidValue validValue = new()
                                {
                                    Name = name,
                                    Value = val,
                                    Key = key,
                                    Operator = op.GetStringValue(),
                                    AndOr = andOr.GetStringValue(),
                                    EncapsulateWithNext = encapsulateWithNext,
                                    CreatedTime = createdTime
                                };
                                validValues.Add(validValue);
                            }
                        }
                    }

                    validValues.Sort((x, y) => (x.CreatedTime).CompareTo(y.CreatedTime));
                    bool isEncapsulated = false;
                    bool isLast = false;
                    for (int i = 0; i < validValues.Count; i++)
                    {
                        if (i == validValues.Count() - 1) isLast = true;
                        this.q += SetQuerySring(validValues[i].Name, 
                                                validValues[i].Value, 
                                                validValues[i].Operator, 
                                                validValues[i].AndOr, 
                                                validValues[i].EncapsulateWithNext,
                                                validValues[i].NegateSearchQuery,
                                                validValues[i].Key, 
                                                isLast, 
                                                isEncapsulated, 
                                                out isEncapsulated);
                    }
                    this.Terms = null;
                }
            }
        }
        private string? q;

        private class ValidValue
        {
            public required string Name { get; set; }
            public required object Value { get; set; }
            public string? Key { get; set; }
            public required string Operator { get; set; }
            public string? AndOr { get; set; }
            public bool EncapsulateWithNext { get; set; }
            public bool NegateSearchQuery { get; set; }
            public DateTime CreatedTime { get; set; }
        }
        private List<ValidValue> ValidValues { get; set; }

        private string SetQuerySring(string name, 
                              object val, 
                              string op,
                              string andOr,
                              bool encapsulateWithNext,
                              bool negateSearchQuery,
                              string key,
                              bool isLast,
                              bool isEncapsulated,
                              out bool isEncapsulatedOut
                             )
        {
            isEncapsulatedOut = isEncapsulated;
            string? str = null;
            string? queryString = null;

            if (negateSearchQuery)
                str = "not ";

            switch (name)
            {
                case "parents":
                case "owners":
                case "writers":
                    str += $"'{val}' {op} {name}";
                    break;
                case "properties":
                case "appProperties":
                    str += $"{name} {op} {{ key='{key}' and value='{val}' }}";
                    break;
                case "trashed":
                case "starred":
                    str += $"{name} {op} {val}";
                    break;
                default:
                    str += $"{name} {op} '{val}'";
                    break;
            }

            if (isLast)
            {
                if (isEncapsulated)
                {
                    queryString = $"{str})";
                }
                else
                {
                    queryString = str;
                }
                return queryString;
            }

            if (!isEncapsulated)
            {
                if (encapsulateWithNext)
                {
                    isEncapsulatedOut = true;
                    queryString = "(";
                }
                queryString += $"{str} {andOr} ";
                return queryString;
            }
            else
            {
                if (!encapsulateWithNext)
                {
                    isEncapsulatedOut = false;
                    queryString = $"{str}) {andOr} ";
                }
                else
                {
                    queryString = $"{str} {andOr} ";
                }
                return queryString;
            }
        }
    }
}
