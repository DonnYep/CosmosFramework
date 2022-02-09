namespace Cosmos
{
    public partial class AOIZone<T>
    {
        public class SkipListNode
        {
            private T value;
            private SkipListNode next;
            private SkipListNode previous;
            private SkipListNode above;
            private SkipListNode below;
            public virtual T Value { get { return value; } set { this.value = value; } }
            public SkipListNode Next { get { return next; } set { next = value; } }
            public SkipListNode Previous { get { return previous; } set { previous = value; } }
            public SkipListNode Above { get { return above; } set { above = value; } }
            public SkipListNode Below { get { return below; } set { below = value; } }
            public SkipListNode(T value)
            {
                this.value = value;
            }
            public void Dispose()
            {
                value = default(T);
                next = null;
                previous = null;
                above = null;
                previous = null;
            }
        }
    }
}