using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ios_Android_Project.Utilities
{
    [AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class TestAttribute : TestMethodAttribute
    {
        public UserMode UserMode;
    }
    public enum UserMode
    {
        Normal,
        SuperAdmin,
        Admin,
        SystemAdmin
    }
}
