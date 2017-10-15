﻿using System;

namespace App.Domain.Models
{
    [Flags]
    public enum NoteImportance
    {
        Low = 1,
        Medium = 2,
        High = 4,
        All = Low | Medium | High
    }
}