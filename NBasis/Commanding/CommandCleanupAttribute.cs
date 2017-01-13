using System;

namespace NBasis.Commanding
{
    [Flags]
    public enum CleanupFlags
    {
        None = 0,
        TrimString = 1,
        LowerString = 2
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class CommandCleanupAttribute : Attribute
    {
        readonly CleanupFlags _level;

        public CommandCleanupAttribute(CleanupFlags level = CleanupFlags.TrimString)
        {
            _level = level;
        }

        public CleanupFlags Levels
        {
            get { return _level; }
        }
    }
}
