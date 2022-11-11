#if !NETFRAMEWORK

namespace System.Web.Hosting;

internal static class HostingEnvironment
{
    public static string ApplicationPhysicalPath => AppContext.BaseDirectory;
}

#endif