// Guids.cs
// MUST match guids.h
using System;

namespace ArunMahapatra.Pester_Adapter_Package
{
    static class GuidList
    {
        public const string guidPester_Adapter_PackagePkgString = "0aab56e1-3b62-47c1-bb03-7e9afc30ca71";
        public const string guidPester_Adapter_PackageCmdSetString = "7c06f7b8-db0b-4e51-b093-24bdb6c7c964";

        public static readonly Guid guidPester_Adapter_PackageCmdSet = new Guid(guidPester_Adapter_PackageCmdSetString);
    };
}