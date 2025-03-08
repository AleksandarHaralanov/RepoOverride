using RepoOverride;
using System.Reflection;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// PRODUCT INFORMATION
//-----------------------------------------------------------------------------
[assembly: AssemblyTitle(Hax.NAME)]
[assembly: AssemblyProduct(Hax.NAME)]
[assembly: AssemblyCompany(Hax.COMPANY)]

//-----------------------------------------------------------------------------
// VERSION INFORMATION
//-----------------------------------------------------------------------------
[assembly: AssemblyVersion(Hax.VERSION)]
[assembly: AssemblyFileVersion(Hax.VERSION)]

//-----------------------------------------------------------------------------
// LEGAL & TRADEMARK INFORMATION
//-----------------------------------------------------------------------------
[assembly: AssemblyCopyright("Copyright © 2025")]
[assembly: AssemblyTrademark(Hax.COMPANY)]

//-----------------------------------------------------------------------------
// CONFIGURATION & CULTURE
//-----------------------------------------------------------------------------
[assembly: AssemblyCulture("en-US")]

//-----------------------------------------------------------------------------
// INTEROP & SECURITY SETTINGS
//-----------------------------------------------------------------------------
[assembly: ComVisible(false)]
[assembly: Guid("48e39b4f-6850-4c6c-8e9d-9bf4e6729687")]
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

//-----------------------------------------------------------------------------
// OBFUSCATION SETTINGS
//-----------------------------------------------------------------------------
// [assembly: ObfuscateAssembly(true)]

//-----------------------------------------------------------------------------
// METADATA
//-----------------------------------------------------------------------------
[assembly: AssemblyMetadata("BuildDate", Hax.BUILD_DATE)]