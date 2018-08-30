using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Auctus.Util
{
    public class FileTypeMatcher
    {
        private static readonly IEnumerable<FileType> KnownTypes = new FileType[]
            {
                new FileType("PNG", new FileTypeMatcher(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })),
                new FileType("JPG", new FileTypeMatcher(new byte[] { 0xFF, 0xD8 }))
                //new FileType("GIF", new FileTypeMatcher(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, null)), 
                //new FileType("GIF", new FileTypeMatcher(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, null)),
                //new FileType("PDF", new FileTypeMatcher(new byte[] { 0x25, 0x50, 0x44, 0x46 }))
            };

        private readonly IEnumerable<byte> StartBytes;

        private FileTypeMatcher(IEnumerable<byte> startBytes)
        {
            StartBytes = startBytes;
        }

        public static IEnumerable<string> GetValidFileExtensions()
        {
            return new string[] { "JPG", "JPEG", "PNG" };
        }

        public static string GetFileExtension(Stream stream)
        {
            return KnownTypes.Where(c => c.Matcher.Matches(stream)).FirstOrDefault()?.Extension;
        }

        private bool Matches(Stream stream)
        {
            if (stream == null)
                throw new ArgumentException("Invalid stream.");
            if (!stream.CanRead || (stream.Position != 0 && !stream.CanSeek))
                throw new ArgumentException("File contents must be a readable stream.");

            var result = MatchesPrivate(stream);
            if (stream.Position != 0)
                stream.Seek(0, SeekOrigin.Begin);

            return result;
        }

        private bool MatchesPrivate(Stream stream)
        {
            if (stream.Position != 0)
                stream.Seek(0, SeekOrigin.Begin);

            foreach (var b in StartBytes)
            {
                if (stream.ReadByte() != b)
                    return false;
            }
            return true;
        }

        private class FileType
        {
            public string Extension { get; set; }
            public FileTypeMatcher Matcher { get; set; }

            public FileType(string extension, FileTypeMatcher matcher)
            {
                Extension = extension;
                Matcher = matcher;
            }
        }
    }
}
