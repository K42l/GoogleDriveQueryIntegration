using Google.Drive.Query.Integration.Util;

namespace Google.Drive.Query.Integration.Query.File
{
    public class FileTerms
    {
        /// <summary>
        /// Name of the file. 
        /// Surround with single quotes ('). 
        /// Escape single quotes in queries with \', such as 'Valentine\'s Day'.
        /// </summary>
        public List<Name>? Name { get; set; }

        /// <summary>
        /// Whether the name, description, indexableText properties, or text in the file's content or metadata of the file matches. <br/>
        /// Surround with single quotes ('). 
        /// Escape single quotes in queries with \', such as 'Valentine\'s Day'.
        /// </summary>
        public List<FullText>? FullText { get; set; }

        /// <summary>
        /// MIME type of the file. Surround with single quotes ('). 
        /// Escape single quotes in queries with \', such as 'Valentine\'s Day'. 
        /// For further information on MIME types, see Google Workspace and Google Drive supported MIME types.
        /// </summary>
        public List<MimeType>? MimeType
        {
            get { return this.mimeType; }
            set
            {
                if (IsFolder != null)
                {
                    if ((bool)IsFolder)
                    {
                        this.mimeType = new List<MimeType>
                        {
                           new MimeType {Value = File.MimeType.MimeTypeEnum.Folder, Operator = File.MimeType.OperatorEnum.Equal }
                        };
                    }
                    else
                    {
                        this.mimeType = new List<MimeType>
						{
							new MimeType {Value = File.MimeType.MimeTypeEnum.Folder, Operator = File.MimeType.OperatorEnum.Different }
                        };
                    }
                }
                else
                {
					this.mimeType = value;
                }
            }
        }
        private List<MimeType>? mimeType;

        /// <summary>
        /// Short hand to set the MimeType to folder or file.
        /// </summary>
        public bool? IsFolder 
        {
            get { return this.isFolder; }
            set
            {
                this.isFolder = value;
                if (value != null)
                {
                    if ((bool)value)
                    {
                        this.mimeType = new List<MimeType>
                        {
                            new MimeType {Value = File.MimeType.MimeTypeEnum.Folder, Operator = File.MimeType.OperatorEnum.Equal }
                        };
					}
                    else
                    {
						this.mimeType = new List<MimeType>
						{
                            new MimeType {Value = File.MimeType.MimeTypeEnum.Folder, Operator = File.MimeType.OperatorEnum.Different }
                        };
                    }
                }
            }
        }
        private bool? isFolder;

        /// <summary>
        /// Date of the last file modification. 
        /// RFC 3339 format, default time zone is UTC, such as 2012-06-04T12:00:00-08:00. 
        /// Fields of type date are not comparable to each other, only to constant dates.
        /// </summary>
        public List<ModifiedTime>? ModifiedTime { get; set; }
        /// <summary>
        /// Date that the user last viewed a file. 
        /// RFC 3339 format, default time zone is UTC, such as 2012-06-04T12:00:00-08:00. 
        /// Fields of type date are not comparable to each other, only to constant dates.
        /// </summary>
        public List<ViewedByMeTime>? ViewedByMeTime { get; set; }


        /// <summary>
        /// Whether the file is in the trash or not. 
        /// Can be either true or false.
        /// </summary>
        public List<Trashed>? Trashed { get; set; }

        /// <summary>
        /// Whether the file is starred or not. 
        /// Can be either true or false.
        /// </summary>
        public List<Starred>? Starred { get; set; }

        /// <summary>
        /// Whether the parents collection contains the specified ID.
        /// </summary>
        public List<Parents>? Parents { get; set; }

        /// <summary>
        /// Users who own the file.
        /// </summary>
        public List<Owners>? Owners { get; set; }

        /// <summary>
        /// Users or groups who have permission to modify the file.
        /// See the permissions resource reference.
        /// </summary>
        public List<Writers>? Writers { get; set; }

        /// <summary>
        /// Users or groups who have permission to read the file. 
        /// See the permissions resource reference.
        /// </summary>
        public List<Readers>? Readers { get; set; }

        /// <summary>
        /// Files that are in the user's "Shared with me" collection. 
        /// All file users are in the file's Access Control List (ACL). 
        /// Can be either true or false.
        /// </summary>
        public List<SharedWithMe>? SharedWithMe { get; set; }

        /// <summary>
        /// Date when the shared drive was created. 
        /// Use RFC 3339 format, default time zone is UTC, such as 2012-06-04T12:00:00-08:00.
        /// </summary>
        public List<CreatedTime>? CreatedTime { get; set; }

        /// <summary>
        /// Public custom file properties.
        /// </summary>
        public List<Properties>? Properties { get; set; }

        /// <summary>
        /// Private custom file properties.
        /// </summary>
        public List<AppProperties>? AppProperties { get; set; }

        /// <summary>
        /// The visibility level of the file. 
        /// Valid values are anyoneCanFind, anyoneWithLink, domainCanFind, domainWithLink, and limited. 
        /// Surround with single quotes (').
        /// </summary>
        public List<Visibility>? Visibility { get; set; }

        /// <summary>
        /// The ID of the item the shortcut points to.
        /// </summary>
        public List<ShortcutDetailsTargetId>? ShortcutDetailsTargetId { get; set; }
    }

    public class Name : TermsBase
    {
        public enum OperatorEnum
        {
            [StringValue("=")]
            Equal,

            [StringValue("contains")]
            Contains,

            [StringValue("!=")]
            Different
        }
        public virtual OperatorEnum Operator { get; set; }
        public virtual string? Value { get; set; }
    }

    public class FullText : TermsBase
    {
        public enum OperatorEnum
        {
            [StringValue("contains")]
            Contains
        }
        public virtual OperatorEnum Operator { get; set; }
        public virtual string? Value { get; set; }
    }

    public class MimeType : TermsBase
    {
        public enum OperatorEnum
        {
            [StringValue("=")]
            Equal,

            [StringValue("contains")]
            Contains,

            [StringValue("!=")]
            Different
        }
        public virtual OperatorEnum Operator { get; set; }

        public enum MimeTypeEnum
        {
            [StringValue("application/vnd.google-apps.file")]
            File,

            [StringValue("application/vnd.google-apps.folder")]
            Folder,

            [StringValue("application/vnd.google-apps.audio")]
            Audio,

            [StringValue("application/vnd.google-apps.document")]
            Document,

            [StringValue("application/vnd.google-apps.drive-sdk")]
            DriveSdk,

            [StringValue("application/vnd.google-apps.drawing")]
            Drawing,

            [StringValue("application/vnd.google-apps.form")]
            Form,

            [StringValue("application/vnd.google-apps.fusiontable")]
            Fusiontable,

            [StringValue("application/vnd.google-apps.jam")]
            Jam,

            [StringValue("application/vnd.google-apps.mail-layout")]
            MailLayout,

            [StringValue("application/vnd.google-apps.map")]
            Map,

            [StringValue("application/vnd.google-apps.photo")]
            Photo,

            [StringValue("application/vnd.google-apps.presentation")]
            Presentation,

            [StringValue("application/vnd.google-apps.script")]
            Script,

            [StringValue("application/vnd.google-apps.shortcut")]
            Shortcut,

            [StringValue("application/vnd.google-apps.site")]
            Site,

            [StringValue("application/vnd.google-apps.spreadsheet")]
            Spreadsheet,

            [StringValue("application/vnd.google-apps.unknown")]
            Unknown,

            [StringValue("application/vnd.google-apps.vid")]
            Vid,

            [StringValue("application/vnd.google-apps.video")]
            Video
        }

        public MimeTypeEnum Value { get; set; }
    }

    public class ModifiedTime : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("=")]
            Equal,

            [StringValue("!=")]
            Different,

            [StringValue(">")]
            Greater,

            [StringValue(">=")]
            GreaterOrEqual,

            [StringValue("<")]
            Less,

            [StringValue("<=")]
            LessOrEqual

        };
        public virtual OperatorEnum Operator { get; set; }
        public virtual DateTime? Value { get; set; }
    }

    public class ViewedByMeTime : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("=")]
            Equal,

            [StringValue("!=")]
            Different,

            [StringValue(">")]
            Greater,

            [StringValue(">=")]
            GreaterOrEqual,

            [StringValue("<")]
            Less,

            [StringValue("<=")]
            LessOrEqual

        };
        public virtual OperatorEnum Operator { get; set; }
        public virtual DateTime? Value { get; set; }
    }

    public class Trashed : TermsBase
    {
        public enum OperatorEnum
        {
            [StringValue("=")]
            Equal,

            [StringValue("!=")]
            Different
        };
        public virtual OperatorEnum Operator { get; set; }
        public bool Value { get; set; }
    }

    public class Starred : TermsBase
    {
        public enum OperatorEnum
        {
            [StringValue("=")]
            Equal,

            [StringValue("!=")]
            Different
        };
        public virtual OperatorEnum Operator { get; set; }
        public bool Value { get; set; }
    }

    public class Parents : TermsBase
    {
        public enum OperatorEnum
        {
            [StringValue("in")]
            In
        };
        public virtual OperatorEnum Operator { get; set; }
        public virtual string? Value { get; set; }
    }

    public class Owners : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("in")]
            In
        };
        public virtual OperatorEnum Operator { get; set; }
        public virtual string? Value { get; set; }
    }

    public class Writers : TermsBase
    {
        public enum OperatorEnum
        {
            [StringValue("in")]
            In
        };
        public virtual OperatorEnum Operator { get; set; }

        public virtual string? Value { get; set; }
    }

    public class Readers : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("in")]
            In
        };
        public virtual OperatorEnum Operator { get; set; }
        public virtual string? Value { get; set; }
    }

    public class SharedWithMe : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("=")]
            Equal,

            [StringValue("!=")]
            Different
        };
        public virtual OperatorEnum Operator { get; set; }
        public bool Value { get; set; }
    }

    public class CreatedTime : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("=")]
            Equal,

            [StringValue("!=")]
            Different,

            [StringValue(">")]
            Greater,

            [StringValue(">=")]
            GreaterOrEqual,

            [StringValue("<")]
            Less,

            [StringValue("<=")]
            LessOrEqual

        };
        public virtual OperatorEnum Operator { get; set; }

        public virtual string? Value { get; set; }
    }

    public class Properties : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("has")]
            Has
        };
        public virtual OperatorEnum Operator { get; set; }
        public virtual string? Key { get; set; }

        public virtual string? Value { get; set; }
    }

    public class AppProperties : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("has")]
            Has
        };
        public virtual OperatorEnum Operator { get; set; }

        public virtual string? Value { get; set; }
    }

    public class Visibility : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("=")]
            Equal,

            [StringValue("!=")]
            Different,

            [StringValue(">")]
            Greater,

            [StringValue(">=")]
            GreaterOrEqual,

            [StringValue("<")]
            Less,

            [StringValue("<=")]
            LessOrEqual

        };
        public virtual OperatorEnum Operator { get; set; }

        public enum ValueEnum
        {
            [StringValue("anyoneCanFind")]
            AnyoneCanFind,

            [StringValue("anyoneWithLink")]
            AnyoneWithLink,

            [StringValue("domainCanFind")]
            DomainCanFind,

            [StringValue("domainWithLink")]
            DomainWithLink,

            [StringValue("limited")]
            Limited
        }
        public ValueEnum Value { get; set; }
    }

    public class ShortcutDetailsTargetId : TermsBase
    {
        public enum OperatorEnum
        {

            [StringValue("=")]
            Equal,

            [StringValue("!=")]
            Different

        };
        public virtual OperatorEnum Operator { get; set; }

        public virtual string? Value { get; set; }
    }
}
