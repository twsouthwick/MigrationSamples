using System;

namespace eShopWebForms.Models.Infrastructure
{
    internal static class HostingEnvironment
    {
        public static string ApplicationPhysicalPath => AppContext.BaseDirectory;
    }
}