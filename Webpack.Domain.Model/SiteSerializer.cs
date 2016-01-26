// <copyright file="SiteSerializer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model
{
    using Ionic.Zip;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Site Serializer
    /// </summary>
    public class SiteSerializer
    {
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="site">site</param>
        /// <param name="path">path</param>
        /// <returns></returns>
        public void Serialize(Site site, string path)
        {
            if (site == null)
            {
                throw new ArgumentException("site");
            }
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("stream");
            }

            var tempPath = Path.GetTempPath() + "site.xml";
            using (var stream = File.Create(tempPath))
            {
                var serializer = new XmlSerializer(typeof(Site));
                serializer.Serialize(stream, site);
            }

            var packagePath = path + Path.DirectorySeparatorChar + site.Name + ".wbp";
            if (File.Exists(packagePath))
            {
                File.Delete(packagePath);

            }
            using (ZipFile package = new ZipFile(packagePath, Console.Out))
            {
                package.AddFile(tempPath, string.Empty);
                package.AddFiles(site.Resources.Select(r => r.TextData).Where(File.Exists), "resources");
                package.Save();
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="extractedPath">extracted Path</param>
        /// <returns></returns>
        public Site Deserialize(string path, out string extractedPath)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("stream");
            }

            extractedPath = Path.GetTempPath() + "umbraco-import" + Path.DirectorySeparatorChar;
            using (ZipFile package = new ZipFile(path))
            {
                package.ExtractAll(extractedPath, ExtractExistingFileAction.OverwriteSilently);
            }

            using (var stream = File.OpenRead(extractedPath + "site.xml"))
            {
                var serializer = new XmlSerializer(typeof(Site));
                return (Site)serializer.Deserialize(stream);
            }
        }
    }
}
