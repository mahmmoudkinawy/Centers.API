﻿namespace Centers.API.Helpers;
public sealed class SubjectParams : PaginationParams
{
    public string? Keyword { get; set; }

    // Will add more filters on that.
    // public string? OrderBy { get; set; } = "Name";
}
