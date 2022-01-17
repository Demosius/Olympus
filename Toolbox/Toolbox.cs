using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Olympus
{
    public class FileDoesNotExistException : Exception
    {
        public FileDoesNotExistException() : base() { }
        public FileDoesNotExistException(string message) : base(message) { }
    }

    public class WrongFileExtensionException : Exception
    {
        public WrongFileExtensionException() : base() { }
        public WrongFileExtensionException(string message) : base(message) { }
    }

    /// <summary>
    /// General use toolbox class.
    /// </summary>
    public static class Toolbox
    {
        
    }     
}
