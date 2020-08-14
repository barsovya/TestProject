//The class does not contain a full implementation of components, just an example

using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Steamworks.Extensions.Lobby
{
    public static class ExtensionsLobby
    {
        public static string GetNameLobby(this Steamworks.Data.Lobby lobby)
        {
            return lobby.Data.ToList().FirstOrDefault(name => name.Key == "name").Value;
        }

        public static string GetInfoLobbyByKey(this Steamworks.Data.Lobby lobby, string key)
        {
            return lobby.Data.ToList().FirstOrDefault(name => name.Key == key).Value;
        }
    }
}

namespace Steamworks.Extensions.Server
{
    public static class ExtensionsServer
    {
        public static IntPtr ToIntPtr(this byte[] data)
        {
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, unmanagedPointer, data.Length);
            Marshal.FreeHGlobal(unmanagedPointer);
            return unmanagedPointer;
        }

        public static byte[] ToByte(this IntPtr data, int size)
        {
            byte[] response = new byte[size];
            Marshal.Copy(data, response, 0, size);
            return response;
        }

        public static void DebugOutput(NetDebugOutput type, string text)
        {
            Debug.Log($"[<color=white>NET</color>:{type}]\t\t{text}");
        }

        public static (IntPtr data, int size) ConvertDataToIntPtr(byte[] data)
        {
            IntPtr conversionToIntPtr = data.ToIntPtr();
            return (conversionToIntPtr, Marshal.SizeOf(conversionToIntPtr));
        }
    }
}

