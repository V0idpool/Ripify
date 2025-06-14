﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ripify.Helpers
{
    static class SaveFiles
    {
        public static void SaveToDisk(string resourceName, string fileName)
        {
            var assy = Assembly.GetExecutingAssembly();

            foreach (string resource in assy.GetManifestResourceNames())
            {
                if (resource.ToLower().IndexOf(resourceName.ToLower()) != -1)
                {
                    using (var resourceStream = assy.GetManifestResourceStream(resource))
                    {
                        if (resourceStream is not null)
                        {
                            using (var reader = new BinaryReader(resourceStream))
                            {
                                byte[] buffer = reader.ReadBytes((int)resourceStream.Length);
                                using (var outputStream = new FileStream(fileName, FileMode.Create))
                                {
                                    using (var writer = new BinaryWriter(outputStream))
                                    {
                                        writer.Write(buffer);
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
}
