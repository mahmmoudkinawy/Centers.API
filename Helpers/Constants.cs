﻿namespace Centers.API.Helpers;
public static class Constants
{
    public const string TokenKey = "Token:Key";

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
