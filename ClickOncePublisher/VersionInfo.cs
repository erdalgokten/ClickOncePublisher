using System;

namespace ClickOncePublisher
{
    public class VersionInfo
    {
        private int major;
        private int minor;
        private int build;
        private int revision;

        public int Major { get { return this.major; } }
        public int Minor { get { return this.minor; } }
        public int Build { get { return this.build; } }
        public int Revision { get { return this.revision; } }

        public VersionInfo(int major, int minor, int build, int revision)
        {
            this.major = major;
            this.minor = minor;
            this.build = build;
            this.revision = revision;
        }
    }
}
