﻿namespace Centers.API.Helpers;
public static class Constants
{
    public const string TokenKey = "Token:Key";

    public const string CorsPolicyName = "default";
    public const string CorsOriginSectionKey = "CrossOriginRequests:AllowedOrigins";

    public sealed class Policies
    {
        public const string MustBeTeacher = "MustBeTeacher";
        public const string MustBeSuperAdmin = "MustBeSuperAdmin";
        public const string MustBeCenterAdmin = "MustBeCenterAdmin";
        public const string MustBeReviewer = "MustBeReviewer";
    }

    public sealed class MailgunSettings
    {
        public const string ApiKey = "MailgunSettings:ApiKey";
        public const string ApiUrl = "MailgunSettings:ApiUrl";
        public const string SandBoxDomain = "MailgunSettings:SandBoxDomain";
        public const string From = "MailgunSettings:From";
    }

    public sealed class CloudinarySettings
    {
        public const string CloudName = "CloudinarySettings:CloudName";
        public const string ApiKey = "CloudinarySettings:ApiKey";
        public const string ApiSecret = "CloudinarySettings:ApiSecret";
    }

    public sealed class Roles
    {
        public const string Student = "Student";
        public const string CenterAdmin = "CenterAdmin";
        public const string SuperAdmin = "SuperAdmin";
        public const string Teacher = "Teacher";
        public const string Reviewer = "Reviewer";
    }
}
